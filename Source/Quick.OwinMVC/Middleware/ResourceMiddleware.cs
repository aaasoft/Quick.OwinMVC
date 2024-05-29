﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Resource;
using System.Net;
using Quick.OwinMVC.Utils;
using System.IO;
using System.Reflection;
using Quick.OwinMVC.Hunter;

namespace Quick.OwinMVC.Middleware
{
    public class ResourceMiddleware : AbstractPluginPathMiddleware, IPropertyHunter, IAssemblyHunter
    {
        //(资源的缓存过期时间，单位：秒)默认一天
        public double Expires { get; private set; }
        //资源的ETag是否使用MD5值
        public Boolean UseMd5ETag { get; private set; }
        //使用内存缓存
        public bool UseMemoryCache { get; private set; }

        public static StreamCacheManager StreamCacheManager = new StreamCacheManager();

        private ResourceWebRequestFactory resourceWebRequestFactory;

        /// <summary>
        /// 静态文件目录
        /// </summary>
        public String StaticFileFolder
        {
            get { return resourceWebRequestFactory.StaticFileFolder; }
            private set { resourceWebRequestFactory.StaticFileFolder = value; }
        }

        public ResourceMiddleware(OwinMiddleware next) : base(next)
        {
            Expires = 86400;
            UseMd5ETag = false;
            UseMemoryCache = false;

            resourceWebRequestFactory = new ResourceWebRequestFactory();
            resourceWebRequestFactory.AssemblyMap = new Dictionary<String, Assembly>();
            //注册resource:前缀URI处理程序
            WebRequest.RegisterPrefix("resource:", resourceWebRequestFactory);
        }

        private Stream getUrlStream(String url, out ResourceWebResponse response)
        {
            Uri uri = new Uri(url);
            response = null;
            try { response = WebRequest.Create(uri).GetResponse() as ResourceWebResponse; }
            catch { }
            if(response==null)
                return null;
            var stream = response.GetResponseStream();
            return stream;
        }

        public override Task InvokeNotMatch(IOwinContext context)
        {
            var requestPath = context.Request.Path.ToString();
            var isRootContextPath = Server.Instance.IsRootContextPath;
            var contextPath = Server.Instance.ContextPath;
            if (!isRootContextPath
                && requestPath.StartsWith(contextPath)
                && requestPath.Length > contextPath.Length)
                requestPath = requestPath.Substring(Server.Instance.ContextPath.Length - 1);
            var url = string.Format("resource://0{0}",requestPath);
            if (UseMemoryCache)
            {
                var streamCache = StreamCacheManager.GetCache(url);
                if (streamCache != null)
                {
                    return handleResource(context, streamCache.GetStream(), null, url, Expires, AddonHttpHeaders);
                }
            }
            ResourceWebResponse resourceResponse;
            var stream = getUrlStream(url, out resourceResponse);
            if (stream == null)
                return base.InvokeNotMatch(context);
            if (UseMemoryCache)
            {
                var streamCache = new StreamCache(stream);
                StreamCacheManager.AddCache(url, streamCache);
                stream.Close();
                stream.Dispose();
                stream = streamCache.GetStream();
            }
            return handleResource(context, stream, resourceResponse.LastModified, resourceResponse.Uri.LocalPath, Expires, AddonHttpHeaders);
        }

        public override Task Invoke(IOwinContext context, string plugin, string path)
        {
            return InvokeFinal(context, Route, null, plugin, path, t => InvokeNotMatch(context), Expires, AddonHttpHeaders);
        }

        public Task InvokeFinal(IOwinContext context, string prefix, string suffix, string plugin, string path, Func<IOwinContext, Task> noMatchHandler, double expires, IDictionary<string, string> addonHttpHeaders)
        {
            //加前缀
            if (!String.IsNullOrEmpty(prefix))
                path = string.Format("{0}/{1}",prefix,path);

            List<String> resourceUrlList = new List<string>();
            resourceUrlList.Add(string.Format("resource://{0}/{1}",plugin,path));
            //加后缀
            if (!String.IsNullOrEmpty(suffix))
                path = string.Format("{0}{1}",path,suffix);
            resourceUrlList.Add(string.Format("resource://{0}/{1}",plugin,path));

            //先尝试从缓存中获取
            if (UseMemoryCache)
            {
                foreach (var url in resourceUrlList)
                {
                    var streamCache = StreamCacheManager.GetCache(url);
                    if (streamCache != null)
                    {
                        return handleResource(context, streamCache.GetStream(), null, url, expires, addonHttpHeaders);
                    }
                }
            }

            Stream stream;
            ResourceWebResponse resourceResponse;
            foreach (var url in resourceUrlList)
            {
                stream = getUrlStream(url, out resourceResponse);
                if (stream != null)
                {
                    if (UseMemoryCache)
                    {
                        var streamCache = new StreamCache(stream);
                        StreamCacheManager.AddCache(url, streamCache);
                        stream.Close();
                        stream.Dispose();
                        stream = streamCache.GetStream();
                    }
                    return handleResource(context, stream, resourceResponse.LastModified, resourceResponse.Uri.LocalPath, expires, addonHttpHeaders);
                }
            }
            return noMatchHandler(context);
        }

        private Task handleResource(IOwinContext context, Stream stream, DateTime? lastModified, string path, double expires, IDictionary<string, string> addonHttpHeaders)
        {
            var req = context.Request;
            var rep = context.Response;
            //验证缓存有效
            if (lastModified.HasValue)
            {
                //===================
                //先验证最后修改时间
                //===================
                var resourceLastModified = lastModified.Value;
                //最后修改时间判断部分
                var clientLastModified = req.Headers.Get("If-Modified-Since");
                if (clientLastModified != null)
                {
                    if (clientLastModified == resourceLastModified.ToString("R"))
                    {
                        rep.StatusCode = 304;
                        return Task.Run(() => stream.Dispose());
                    }
                }
                //===================
                //然后验证ETag
                //===================
                //ETag设置判断部分
                String serverETag = null;
                if (UseMd5ETag)
                {
                    serverETag = HashUtils.ComputeETagByMd5(stream);
                    if (stream.CanSeek)
                        stream.Position = 0;
                }
                else
                    serverETag = lastModified.Value.Ticks.ToString();
                var clientETag = req.Headers.Get("If-None-Match");
                //如果客户端的ETag值与服务端相同，则返回304，表示资源未修改
                if (serverETag == clientETag)
                {
                    rep.StatusCode = 304;
                    return Task.Run(() => stream.Dispose());
                }
                rep.ETag = serverETag;
            }
            rep.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(expires));
            rep.Headers["Cache-Control"] = "max-age="+expires;
            if (lastModified.HasValue)
                rep.Headers["Last-Modified"] = lastModified.Value.ToUniversalTime().ToString("R");
            return context.Output(stream, true, EnableCompress, path, addonHttpHeaders);
        }

        public override void Hunt(string key, string value)
        {
            switch (key)
            {
                case "StaticFileFolder":
                    resourceWebRequestFactory.StaticFileFolder = value;
                    break;
                case "Expires":
                    Expires = double.Parse(value);
                    break;
                case "UseMd5ETag":
                    UseMd5ETag = Boolean.Parse(value);
                    break;
                case "UseMemoryCache":
                    UseMemoryCache = Boolean.Parse(value);
                    break;
            }
            base.Hunt(key, value);
        }

        public void Hunt(Assembly assembly)
        {
            resourceWebRequestFactory.AssemblyMap[assembly.GetName().Name.ToLower()] = assembly;
        }
    }
}
