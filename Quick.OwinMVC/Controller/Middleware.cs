using Microsoft.Owin;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Controller.Impl;
using Quick.OwinMVC.Routing;
using Quick.OwinMVC.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    public class Middleware : OwinMiddleware
    {
        public const String QOMVC_PLUGIN_KEY = "QOMVC_Plugin";
        public const String QOMVC_PATH_KEY = "QOMVC_Path";


        private ApiHttpController apiController;
        private MvcHttpController mvcHttpController;
        private IDictionary<String, String> pluginAliasDict;
        public IDictionary<String, IHttpController> routes;

        public Middleware(OwinMiddleware next, IViewRender viewRender) : base(next)
        {
            this.apiController = new ApiHttpController();
            this.mvcHttpController = new MvcHttpController(viewRender);
            this.pluginAliasDict = new Dictionary<String, String>();
            this.routes = new Dictionary<String, IHttpController>
            {
                ["/"] = mvcHttpController,
                [$"/:{QOMVC_PLUGIN_KEY}/view/:{QOMVC_PATH_KEY}"] = mvcHttpController,
                [$"/:{QOMVC_PLUGIN_KEY}/resource/:{QOMVC_PATH_KEY}"] = new ResourceHttpController(),
                [$"/:{QOMVC_PLUGIN_KEY}/api/:{QOMVC_PATH_KEY}"] = apiController
            };
            scanController();
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
            });
        }

        private void scanController()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                String pluginName = assembly.GetName().Name;

                foreach (RouteAttribute attr in assembly.GetCustomAttributes<RouteAttribute>())
                {
                    pluginAliasDict[attr.Path] = pluginName;
                }
                foreach (Type type in assembly.GetTypes())
                {
                    foreach (RouteAttribute attr in type.GetCustomAttributes<RouteAttribute>())
                    {
                        if (typeof(IApiController).IsAssignableFrom(type))
                            RegisterApiController(pluginName, attr.Path, (IApiController)Activator.CreateInstance(type));
                        else
                            RegisterMvcController(pluginName, attr.Path, (IMvcController)Activator.CreateInstance(type));
                    }
                }
            }
        }

        public void RegisterMvcController(String plugin, String path, IMvcController controller)
        {
            mvcHttpController.RegisterController(plugin, path, controller);
        }

        public void RegisterApiController(String plugin, String path, IApiController controller)
        {
            apiController.RegisterController(plugin, path, controller);
        }
    }
}
