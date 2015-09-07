using Microsoft.Owin;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class HttpMiddleware : OwinMiddleware, ITypeHunter
    {
        public IDictionary<Regex, IHttpController> routes;

        public HttpMiddleware(OwinMiddleware next) : base(next)
        {
            routes = new Dictionary<Regex, IHttpController>();
        }

        public override Task Invoke(IOwinContext context)
        {
            String path = context.Get<String>("owin.RequestPath");
            IHttpController controller = null;
            foreach (Regex regex in routes.Keys)
            {
                if (regex.IsMatch(path))
                {
                    var groups = regex.Match(path).Groups;
                    var dic = regex.GetGroupNames().ToDictionary(name => name, name => groups[name].Value);
                    foreach (var key in dic.Keys.Where(t => t != "0"))
                        context.Environment.Add(key, dic[key]);
                    controller = routes[regex];
                    break;
                }
            }
            if (controller == null)
                return Next.Invoke(context);
            return Task.Factory.StartNew(() =>
            {
                controller.Service(context);
            });
        }

        private void RegisterController(string path, IHttpController httpController)
        {
            httpController.Init(Server.Instance.properties);
            routes.Add(RouteBuilder.RouteToRegex(path), httpController);
        }

        public void Hunt(Assembly assembly, Type type)
        {
            foreach (RouteAttribute attr in type.GetCustomAttributes<RouteAttribute>())
            {
                if (typeof(IHttpController).IsAssignableFrom(type))
                    RegisterController(attr.Path, (IHttpController)Activator.CreateInstance(type));
            }
        }
    }
}
