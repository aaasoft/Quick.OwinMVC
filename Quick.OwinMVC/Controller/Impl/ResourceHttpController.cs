using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using System.Net;
using Quick.OwinMVC.Utils;
using Quick.OwinMVC.Routing;
using Quick.OwinMVC.Middleware;
using Quick.OwinMVC.Resource;

namespace Quick.OwinMVC.Controller.Impl
{
    [Route("/:" + MvcMiddleware.QOMVC_PLUGIN_KEY + "/resource/:" + MvcMiddleware.QOMVC_PATH_KEY)]
    internal class ResourceHttpController : HttpController
    {
        //默认一天
        private double resourceExpires = 86400;
        private Boolean useMd5ETag = false;

        public override void Init(IDictionary<string, string> properties)
        {
            base.Init(properties);
            if (properties.ContainsKey("Quick.OwinMVC.resourceExpires"))
                resourceExpires = double.Parse(properties["Quick.OwinMVC.useMd5ETag"]);
            if (properties.ContainsKey("Quick.OwinMVC.resourceExpires"))
                useMd5ETag = Boolean.Parse(properties["Quick.OwinMVC.useMd5ETag"]);
        }

        public override void DoGet(IOwinContext context, string plugin, string path)
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
            {
                rep.StatusCode = 404;
                rep.Write($"Resource '{path}' in plugin '{plugin}' not found!");
                return;
            }
            if (useMd5ETag)
            {
                var serverETag = HashUtils.ComputeETagByMd5(stream);
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
            var mime = MimeUtils.GetMime(path);
            if (mime != null)
                rep.ContentType = mime;
            rep.ContentLength = stream.Length;
            rep.Expires = new DateTimeOffset(DateTime.Now.AddSeconds(resourceExpires));
            rep.Headers["Cache-Control"] = $"max-age={resourceExpires}";
            rep.Headers["Last-Modified"] = resourceResponse.LastModified.ToUniversalTime().ToString("R");
            stream.CopyTo(rep.Body);
        }
    }
}
