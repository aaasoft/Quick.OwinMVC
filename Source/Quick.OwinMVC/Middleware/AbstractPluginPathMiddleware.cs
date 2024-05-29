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
    public abstract class AbstractPluginPathMiddleware : OwinMiddleware, IOwinContextCleaner, IPropertyHunter
    {
        public const String QOMVC_PLUGIN_KEY = "QOMVC_PLUGIN_KEY";
        public const String QOMVC_PATH_KEY = "QOMVC_PATH_KEY";

        /// <summary>
        /// 路由
        /// </summary>
        public String Route { get; private set; }
        /// <summary>
        /// 是否启用压缩
        /// </summary>
        protected bool EnableCompress { get; set; }
        /// <summary>
        /// 额外的HTTP头
        /// </summary>
        public IDictionary<string, string> AddonHttpHeaders { get; private set; }

        private Regex route;

        public AbstractPluginPathMiddleware(OwinMiddleware next) : base(next)
        {
            EnableCompress = false;
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

            //添加额外的HTTP头
            if (AddonHttpHeaders != null && AddonHttpHeaders.Count > 0)
                foreach (var header in AddonHttpHeaders)
                    context.Response.Headers[header.Key] = header.Value;

            //交给派生的Middleware
            return Invoke(context, context.Get<String>(QOMVC_PLUGIN_KEY), context.Get<String>(QOMVC_PATH_KEY));
        }

        public abstract Task Invoke(IOwinContext context, String plugin, String path);
        
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
                case "EnableCompress":
                    EnableCompress = bool.Parse(value);
                    break;
                case "Route":
                    Route = value;
                    String fullRoute = string.Format("{0}:{1}/{2}/:{3}", Server.Instance.ContextPath, QOMVC_PLUGIN_KEY, Route, QOMVC_PATH_KEY);
                    route = RouteBuilder.RouteToRegex(fullRoute);
                    break;
                case "AddonHttpHeaders":
                    AddonHttpHeaders = new Dictionary<string, string>();
                    foreach (var headerKeyValue in value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var tmpStrs = headerKeyValue.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tmpStrs.Length < 2)
                            continue;
                        var headerKey = tmpStrs[0].Trim();
                        var headerValue = string.Join(":", tmpStrs.Skip(1));
                        AddonHttpHeaders[headerKey] = headerValue;
                    }
                    break;
            }
        }
    }
}
