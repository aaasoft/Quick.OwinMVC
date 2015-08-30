using Microsoft.Owin;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Controller.Impl;
using Quick.OwinMVC.Routing;
using Quick.OwinMVC.Utils;
using Quick.OwinMVC.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class MvcMiddleware : OwinMiddleware
    {
        public const String VIEWRENDER_CLASS = "Quick.OwinMVC.VIEWRENDER_CLASS";
        public const String QOMVC_PLUGIN_KEY = "QOMVC_PLUGIN_KEY";
        public const String QOMVC_PATH_KEY = "QOMVC_PATH_KEY";

        private IDictionary<String, String> properties;
        private IViewRender viewRender;
        private IDictionary<String, String> pluginAliasDict;
        public IDictionary<Regex, IHttpController> routes;

        public MvcMiddleware(OwinMiddleware next, IDictionary<String, String> properties) : base(next)
        {
            this.properties = properties;
            if (!properties.ContainsKey(VIEWRENDER_CLASS))
                throw new ApplicationException($"Cann't find '{VIEWRENDER_CLASS}' in properties.");

            //创建视图渲染器
            String viewRenderClassName = properties[VIEWRENDER_CLASS];
            this.viewRender = (IViewRender)AssemblyUtils.CreateObject(viewRenderClassName);
            this.viewRender.Init(properties);

            this.pluginAliasDict = new Dictionary<String, String>();
            this.routes = new Dictionary<Regex, IHttpController>();
            scanController();
        }

        public override Task Invoke(IOwinContext context)
        {
            return Task.Factory.StartNew(() =>
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
                //替换别名为完整的程序集名
                String pluginName = context.Get<String>(QOMVC_PLUGIN_KEY);
                if (pluginName != null && pluginAliasDict.ContainsKey(pluginName))
                {
                    pluginName = pluginAliasDict[pluginName];
                    context.Set<String>(QOMVC_PLUGIN_KEY, pluginName);
                }
                //交给HttpController
                controller.Service(context, context.Get<String>(QOMVC_PLUGIN_KEY), context.Get<String>(QOMVC_PATH_KEY));
                return Task.FromResult(0);
            });
        }

        private void scanController()
        {
            List<Action> registerControllerActionList = new List<Action>();
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
                        if (typeof(IHttpController).IsAssignableFrom(type))
                            RegisterController(attr.Path, (IHttpController)Activator.CreateInstance(type));
                        if (typeof(IApiController).IsAssignableFrom(type)
                            || typeof(IMvcController).IsAssignableFrom(type))
                        {
                            IPluginController controller = (IPluginController)Activator.CreateInstance(type);
                            controller.Init(properties);
                            if (typeof(IApiController).IsAssignableFrom(type))
                                registerControllerActionList.Add(() => RegisterController(pluginName, attr.Path, (IApiController)controller));
                            if (typeof(IMvcController).IsAssignableFrom(type))
                                registerControllerActionList.Add(() => RegisterController(pluginName, attr.Path, (IMvcController)controller));
                        }
                    }
                }
            }
            registerControllerActionList.ForEach(t => t.Invoke());
            foreach (MvcHttpController mvcHttpController in routes.Values.Where(t => t is MvcHttpController).Cast<MvcHttpController>())
                mvcHttpController.ViewRender = this.viewRender;
        }

        private void RegisterController(string path, IHttpController httpController)
        {
            httpController.Init(properties);
            routes.Add(RouteBuilder.RouteToRegex(path), httpController);
        }

        public void RegisterController(String plugin, String path, IMvcController controller)
        {
            foreach (MvcHttpController mvcHttpController in routes.Values.Where(t => t is MvcHttpController).Cast<MvcHttpController>())
                mvcHttpController.RegisterController(plugin, path, controller);
        }

        public void RegisterController(String plugin, String path, IApiController controller)
        {
            foreach (ApiHttpController apiController in routes.Values.Where(t => t is ApiHttpController).Cast<ApiHttpController>())
                apiController.RegisterController(plugin, path, controller);
        }
    }
}
