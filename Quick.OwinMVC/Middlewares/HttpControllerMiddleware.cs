using Microsoft.Owin;
using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middlewares
{
    public class HttpControllerMiddleware : OwinMiddleware
    {
        public IDictionary<String, IHttpController> route;
        public HttpControllerMiddleware(OwinMiddleware next, IDictionary<String, IHttpController> route) : base(next)
        {
            this.route = route;
        }

        public override Task Invoke(IOwinContext context)
        {
            return Task.Factory.StartNew(() =>
            {

                IHttpController controller = null;
                foreach (String path in route.Keys)
                {
                    if (context.Request.Path.StartsWithSegments(new PathString(path)))
                    {
                        controller = route[path];
                        break;
                    }
                }
                if (controller == null)
                    return Next.Invoke(context);
                controller.Service(context);
                return Task.FromResult(0);

                /*
                String content =
                context.Response.ContentType = "text/plain";
                context.Response.ContentLength = content.Length;
                context.Response.StatusCode = 200;
                context.Response.Expires = DateTimeOffset.Now;
                context.Response.Write(content);

                ViewData data = new ViewData();
                data.ViewBag.Title = "Hello ";
                data.ViewBag.Content = "OWIN!";
                return Task.FromResult(0);
                */
            });
        }
    }
}
