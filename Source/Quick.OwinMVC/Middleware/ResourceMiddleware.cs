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
        //默认一天
        private double resourceExpires = 86400;
        private Boolean useMd5ETag = false;
        private ResourceWebRequestFactory resourceWebRequestFactory;
        private String StaticFileFolder
        {
            set { resourceWebRequestFactory.StaticFileFolder = value; }
        }

        public ResourceMiddleware(OwinMiddleware next) : base(next)
        {
            var properties = Server.Instance.properties;
            if (properties.ContainsKey("Quick.OwinMVC.resourceExpires"))
                resourceExpires = double.Parse(properties["Quick.OwinMVC.useMd5ETag"]);
            if (properties.ContainsKey("Quick.OwinMVC.resourceExpires"))
                useMd5ETag = Boolean.Parse(properties["Quick.OwinMVC.useMd5ETag"]);

            resourceWebRequestFactory = new ResourceWebRequestFactory();
            resourceWebRequestFactory.PluginAliasMap = base.pluginAliasDict;
            resourceWebRequestFactory.AssemblyMap = new Dictionary<String, Assembly>();
            //注册resource:前缀URI处理程序
            WebRequest.RegisterPrefix("resource:", resourceWebRequestFactory);
        }

        public override string GetRouteMiddle()
        {
            return "resource";
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
            return handleResource(context, stream, resourceResponse);
        }

        public override Task Invoke(IOwinContext context, string plugin, string path)
        {
            Stream stream;
            ResourceWebResponse resourceResponse;

            stream = getUrlStream($"resource://{plugin}/resource/{path}", out resourceResponse);
            if (stream == null)
                if (plugin != null && pluginAliasDict.ContainsKey(plugin))
                {
                    plugin = pluginAliasDict[plugin];
                    stream = getUrlStream($"resource://{plugin}/resource/{path}", out resourceResponse);
                }
            if (stream == null)
                return this.InvokeNotMatch(context);
            return handleResource(context, stream, resourceResponse);
        }

        private Task handleResource(IOwinContext context, Stream stream, ResourceWebResponse resourceResponse)
        {
            return Task.Factory.StartNew(() =>
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
                            return;
                        }
                    }
                    //===================
                    //然后验证ETag
                    //===================
                    //ETag设置判断部分
                    String serverETag = null;
                    if (useMd5ETag)
                        serverETag = HashUtils.ComputeETagByMd5(stream);
                    else
                        serverETag = resourceResponse.LastModified.Ticks.ToString();
                    var clientETag = req.Headers.Get("If-None-Match");
                    //如果客户端的ETag值与服务端相同，则返回304，表示资源未修改
                    if (serverETag == clientETag)
                    {
                        rep.StatusCode = 304;
                        return;
                    }
                    rep.ETag = serverETag;
                    stream.Position = 0;
                }
                //设置MIME类型
                var mime = MimeUtils.GetMime(resourceResponse.Uri.LocalPath);
                if (mime != null)
                    rep.ContentType = mime;
                rep.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(resourceExpires));
                rep.Headers["Cache-Control"] = $"max-age={resourceExpires}";
                rep.Headers["Last-Modified"] = resourceResponse.LastModified.ToUniversalTime().ToString("R");
                Output(context, stream);
            });
        }


        public override void Hunt(string key, string value)
        {
            switch (key)
            {
                case nameof(StaticFileFolder):
                    resourceWebRequestFactory.StaticFileFolder = value;
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
