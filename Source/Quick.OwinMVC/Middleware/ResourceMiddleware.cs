using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Resource;
using System.Net;
using Quick.OwinMVC.Utils;

namespace Quick.OwinMVC.Middleware
{
    public class ResourceMiddleware : AbstractPluginPathMiddleware
    {
        //默认一天
        private double resourceExpires = 86400;
        private Boolean useMd5ETag = false;

        public ResourceMiddleware(OwinMiddleware next) : base(next)
        {
            var properties = Server.Instance.properties;
            if (properties.ContainsKey("Quick.OwinMVC.resourceExpires"))
                resourceExpires = double.Parse(properties["Quick.OwinMVC.useMd5ETag"]);
            if (properties.ContainsKey("Quick.OwinMVC.resourceExpires"))
                useMd5ETag = Boolean.Parse(properties["Quick.OwinMVC.useMd5ETag"]);
        }

        public override string GetRouteMiddle()
        {
            return "resource";
        }

        public override Task Invoke(IOwinContext context, string plugin, string path)
        {
            var req = context.Request;
            var rep = context.Response;

            Uri uri = new Uri($"resource://{plugin}/resource/{path}");
            ResourceWebResponse resourceResponse = null;
            try
            {
                resourceResponse = WebRequest.Create(uri).GetResponse() as ResourceWebResponse;
            }
            catch { }

            var stream = resourceResponse?.GetResponseStream();
            if (stream == null)
                return Next.Invoke(context);

            return Task.Factory.StartNew(() =>
            {
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
                var mime = MimeUtils.GetMime(path);
                if (mime != null)
                    rep.ContentType = mime;
                rep.ContentLength = stream.Length;
                rep.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(resourceExpires));
                rep.Headers["Cache-Control"] = $"max-age={resourceExpires}";
                rep.Headers["Last-Modified"] = resourceResponse.LastModified.ToUniversalTime().ToString("R");
                stream.CopyTo(rep.Body);
            });
        }
    }
}
