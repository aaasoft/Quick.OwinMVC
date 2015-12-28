using System;
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
using System.IO.Compression;
using Quick.OwinMVC.Hunter;

namespace Quick.OwinMVC.Middleware
{
    public class ResourceMiddleware : AbstractPluginPathMiddleware, IPropertyHunter, IAssemblyHunter
    {
        //(资源的缓存过期时间，单位：秒)默认一天
        private double Expires = 86400;
        //资源的ETag是否使用MD5值
        private Boolean UseMd5ETag = false;

        private ResourceWebRequestFactory resourceWebRequestFactory;
        private String StaticFileFolder
        {
            set { resourceWebRequestFactory.StaticFileFolder = value; }
        }

        public ResourceMiddleware(OwinMiddleware next) : base(next)
        {
            resourceWebRequestFactory = new ResourceWebRequestFactory();
            resourceWebRequestFactory.PluginAliasMap = base.pluginAliasDict;
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
            var stream = response?.GetResponseStream();
            return stream;
        }

        public override Task InvokeNotMatch(IOwinContext context)
        {
            ResourceWebResponse resourceResponse;
            var stream = getUrlStream($"resource://0{context.Request.Path}", out resourceResponse);
            if (stream == null)
                return base.InvokeNotMatch(context);
            return handleResource(context, stream, resourceResponse, Expires);
        }

        public override Task Invoke(IOwinContext context, string plugin, string path)
        {
            return InvokeFinal(context, Route, null, plugin, path, t => InvokeNotMatch(context), Expires);
        }

        public Task InvokeFinal(IOwinContext context, string prefix, string suffix, string plugin, string path, Func<IOwinContext, Task> noMatchHandler, double expires)
        {
            //加前缀
            if (!String.IsNullOrEmpty(prefix))
                path = $"{prefix}/{path}";
            //加后缀
            if (!String.IsNullOrEmpty(suffix))
                path = $"{path}{suffix}";
            Stream stream;
            ResourceWebResponse resourceResponse;

            stream = getUrlStream($"resource://{plugin}/{path}", out resourceResponse);
            if (stream == null)
                return noMatchHandler(context);
            return handleResource(context, stream, resourceResponse, expires);
        }

        private Task handleResource(IOwinContext context, Stream stream, ResourceWebResponse resourceResponse, double expires)
        {
            var req = context.Request;
            var rep = context.Response;
            //验证缓存有效
            {
                //===================
                //先验证最后修改时间
                //===================
                var resourceLastModified = resourceResponse.LastModified;
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
                    serverETag = HashUtils.ComputeETagByMd5(stream);
                else
                    serverETag = resourceResponse.LastModified.Ticks.ToString();
                var clientETag = req.Headers.Get("If-None-Match");
                //如果客户端的ETag值与服务端相同，则返回304，表示资源未修改
                if (serverETag == clientETag)
                {
                    rep.StatusCode = 304;
                    return Task.Run(() => stream.Dispose());
                }
                rep.ETag = serverETag;
                stream.Position = 0;
            }
            //设置MIME类型
            var mime = MimeUtils.GetMime(resourceResponse.Uri.LocalPath);
            if (mime != null)
                rep.ContentType = mime;
            rep.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(expires));
            rep.Headers["Cache-Control"] = $"max-age={expires}";
            rep.Headers["Last-Modified"] = resourceResponse.LastModified.ToUniversalTime().ToString("R");
            return Output(context, stream)
                .ContinueWith(t =>
                {
                    stream.Dispose();
                });
        }

        public override void Hunt(string key, string value)
        {
            switch (key)
            {
                case nameof(StaticFileFolder):
                    resourceWebRequestFactory.StaticFileFolder = value;
                    break;
                case nameof(Expires):
                    Expires = double.Parse(value);
                    break;
                case nameof(UseMd5ETag):
                    UseMd5ETag = Boolean.Parse(value);
                    break;
            }
            base.Hunt(key, value);
        }

        public override void Hunt(Assembly assembly)
        {
            resourceWebRequestFactory.AssemblyMap[assembly.GetName().Name.ToLower()] = assembly;
            base.Hunt(assembly);
        }
    }
}
