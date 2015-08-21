using Microsoft.Owin;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    public class Middleware : OwinMiddleware
    {
        public const String QOMVC_PLUGIN_KEY = "QOMVC_Plugin";
        public const String QOMVC_PATH_KEY = "QOMVC_Path";

        public IDictionary<String, IHttpController> routes;
        public Middleware(OwinMiddleware next, IDictionary<String, IHttpController> route) : base(next)
        {
            this.routes = route;
        }

        public override Task Invoke(IOwinContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                String path = context.Environment["owin.RequestPath"].ToString();
                IHttpController controller = null;
                foreach (String reoute in routes.Keys)
                {
                    var regex = RouteBuilder.RouteToRegex(reoute);
                    if (regex.IsMatch(path))
                    {
                        var groups = regex.Match(path).Groups;
                        var dic = regex.GetGroupNames().ToDictionary(name => name, name => groups[name].Value);
                        foreach (var key in dic.Keys.Where(t => t != "0"))
                            context.Environment.Add(key, dic[key]);
                        controller = routes[reoute];
                        break;
                    }
                }
                if (controller == null)
                    return Next.Invoke(context);
                controller.Service(context, context.Get<String>(QOMVC_PLUGIN_KEY), context.Get<String>(QOMVC_PATH_KEY));
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
