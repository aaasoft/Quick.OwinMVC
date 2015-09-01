using Microsoft.Owin;
using Quick.OwinMVC.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Quick.OwinMVC.Controller;

namespace Quick.OwinMVC.Middleware
{
    public abstract class AbstractPluginPathMiddleware : OwinMiddleware
    {
        public const String VIEWRENDER_CLASS = "Quick.OwinMVC.VIEWRENDER_CLASS";
        public const String QOMVC_PLUGIN_KEY = "QOMVC_PLUGIN_KEY";
        public const String QOMVC_PATH_KEY = "QOMVC_PATH_KEY";

        private static IDictionary<String, String> pluginAliasDict;
        private Regex route;

        static AbstractPluginPathMiddleware()
        {
            pluginAliasDict = new Dictionary<String, String>();
            scanController();
        }

        public AbstractPluginPathMiddleware(OwinMiddleware next) : base(next)
        {
            String fullRoute = $"/:{QOMVC_PLUGIN_KEY}/{GetRouteMiddle()}/:{QOMVC_PATH_KEY}";
            route = RouteBuilder.RouteToRegex(fullRoute);
        }

        public override Task Invoke(IOwinContext context)
        {
            String path = context.Get<String>("owin.RequestPath");
            if (!route.IsMatch(path))
                return Next.Invoke(context);

            //如果还没有解析插件名称和路径
            if (context.Get<String>(QOMVC_PLUGIN_KEY) == null)
            {
                var groups = route.Match(path).Groups;
                var dic = route.GetGroupNames().ToDictionary(name => name, name => groups[name].Value);
                foreach (var key in dic.Keys.Where(t => t != "0"))
                    context.Environment.Add(key, dic[key]);

                //替换别名为完整的程序集名
                String pluginName = context.Get<String>(QOMVC_PLUGIN_KEY);
                if (pluginName != null && pluginAliasDict.ContainsKey(pluginName))
                {
                    pluginName = pluginAliasDict[pluginName];
                    context.Set<String>(QOMVC_PLUGIN_KEY, pluginName);
                }
            }
            //交给派生的Middleware
            return Invoke(context, context.Get<String>(QOMVC_PLUGIN_KEY), context.Get<String>(QOMVC_PATH_KEY));
        }

        public abstract String GetRouteMiddle();
        public abstract Task Invoke(IOwinContext context, String plugin, String path);


        private static void scanController()
        {
            List<Action> registerControllerActionList = new List<Action>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                String pluginName = assembly.GetName().Name;
                foreach (RouteAttribute attr in assembly.GetCustomAttributes<RouteAttribute>())
                {
                    pluginAliasDict[attr.Path] = pluginName;
                }
            }
        }
    }
}
