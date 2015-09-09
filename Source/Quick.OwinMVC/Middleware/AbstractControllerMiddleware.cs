using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin;
using System.Threading.Tasks;
using System.Reflection;
using Quick.OwinMVC.Routing;

namespace Quick.OwinMVC.Middleware
{
    public abstract class AbstractControllerMiddleware<T> : AbstractPluginPathMiddleware, ITypeHunter
        where T : IPluginController
    {
        protected Encoding encoding = new UTF8Encoding(false);
        private IDictionary<String, T> controllerDict = new Dictionary<String, T>();

        internal void RegisterController(string plugin, string path, T controller)
        {
            controllerDict[$"{plugin}:{path}"] = controller;
        }

        public AbstractControllerMiddleware(OwinMiddleware next) : base(next) { }


        public void Hunt(Assembly assembly, Type type)
        {
            String pluginName = assembly.GetName().Name;
            foreach (RouteAttribute attr in type.GetCustomAttributes<RouteAttribute>())
            {
                if (typeof(T).IsAssignableFrom(type))
                {

                    T controller = (T)Activator.CreateInstance(type);
                    controller.Init(Server.Instance.properties);
                    RegisterController(pluginName, attr.Path, controller);
                }
            }
        }

        public override Task Invoke(IOwinContext context, string plugin, string path)
        {
            //先替换别名
            if (plugin != null && pluginAliasDict.ContainsKey(plugin))
                plugin = pluginAliasDict[plugin];
            //然后查找控制器
            if (!controllerDict.ContainsKey($"{plugin}:{path}"))
                return Next.Invoke(context);
            T controller = controllerDict[$"{plugin}:{path}"];
            return Task.Factory.StartNew(() => ExecuteController(controller, context, plugin, path));
        }

        public abstract void ExecuteController(T controller, IOwinContext context, string plugin, string path);
    }
}
