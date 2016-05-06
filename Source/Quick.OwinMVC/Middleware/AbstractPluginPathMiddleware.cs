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
using System.IO;
using System.IO.Compression;
using Quick.OwinMVC.Hunter;

namespace Quick.OwinMVC.Middleware
{
    public abstract class AbstractPluginPathMiddleware : OwinMiddleware, IAssemblyHunter, IOwinContextCleaner, IPropertyHunter
    {
        public const String QOMVC_PLUGIN_KEY = "QOMVC_PLUGIN_KEY";
        public const String QOMVC_PATH_KEY = "QOMVC_PATH_KEY";

        /// <summary>
        /// 路由
        /// </summary>
        protected String Route { get; set; }
        /// <summary>
        /// 是否启用压缩
        /// </summary>
        protected bool EnableCompress { get; set; } = false;

        protected IDictionary<String, String> pluginAliasDict;
        private Regex route;

        public AbstractPluginPathMiddleware(OwinMiddleware next) : base(next)
        {
            pluginAliasDict = new Dictionary<String, String>();
        }

        public virtual Task InvokeNotMatch(IOwinContext context)
        {
            return Next.Invoke(context);
        }

        public override Task Invoke(IOwinContext context)
        {
            String path = context.Get<String>("owin.RequestPath");
            if (!route.IsMatch(path))
                return InvokeNotMatch(context);

            //如果还没有解析插件名称和路径
            if (context.Get<String>(QOMVC_PLUGIN_KEY) == null)
            {
                var groups = route.Match(path).Groups;
                var dic = route.GetGroupNames().ToDictionary(name => name, name => groups[name].Value);
                foreach (var key in dic.Keys.Where(t => t != "0"))
                    context.Environment.Add(key, dic[key]);
            }
            //交给派生的Middleware
            return Invoke(context, context.Get<String>(QOMVC_PLUGIN_KEY), context.Get<String>(QOMVC_PATH_KEY));
        }

        public abstract Task Invoke(IOwinContext context, String plugin, String path);

        public virtual void Hunt(Assembly assembly)
        {
            String pluginName = assembly.GetName().Name;
            foreach (RouteAttribute attr in assembly.GetCustomAttributes<RouteAttribute>())
            {
                pluginAliasDict[attr.Path] = pluginName;
            }
        }

        void IOwinContextCleaner.Clean(IOwinContext context)
        {
            if (context.Environment.ContainsKey(QOMVC_PLUGIN_KEY))
                context.Environment.Remove(QOMVC_PLUGIN_KEY);
            if (context.Environment.ContainsKey(QOMVC_PATH_KEY))
                context.Environment.Remove(QOMVC_PATH_KEY);
        }

        public virtual void Hunt(string key, string value)
        {
            switch (key)
            {
                case nameof(EnableCompress):
                    EnableCompress = bool.Parse(value);
                    break;
                case nameof(Route):
                    Route = value;
                    String fullRoute = $"/:{QOMVC_PLUGIN_KEY}/{Route}/:{QOMVC_PATH_KEY}";
                    route = RouteBuilder.RouteToRegex(fullRoute);
                    break;
            }
        }
    }
}
