using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin;
using System.Threading.Tasks;
using System.Reflection;
using Quick.OwinMVC.Routing;
using Quick.OwinMVC.Hunter;

namespace Quick.OwinMVC.Middleware
{
    public abstract class AbstractControllerMiddleware<T> : AbstractPluginPathMiddleware, ITypeHunter, IHungryPropertyHunter
        where T : IPluginController
    {
        protected Encoding encoding = new UTF8Encoding(false);
        private IDictionary<string, string> properties;
        private IDictionary<String, T> controllerDict = new Dictionary<String, T>();

        internal void RegisterController(string plugin, string path, T controller)
        {
            controllerDict[string.Format("{0}:{1}",plugin,path)] = controller;
            HunterUtils.TryHunt(controller, properties);
        }

        public AbstractControllerMiddleware(OwinMiddleware next) : base(next) { }


        public virtual void Hunt(IDictionary<string, string> properties)
        {
            this.properties = properties;
        }

        public void Hunt(Type type)
        {
            String pluginName = type.Assembly.GetName().Name;
            foreach (RouteAttribute attr in type.GetCustomAttributes<RouteAttribute>())
            {
                if (typeof(T).IsAssignableFrom(type))
                {
                    T controller = (T)Activator.CreateInstance(type);
                    RegisterController(pluginName, attr.Path, controller);
                }
            }
        }

        public override Task Invoke(IOwinContext context, string plugin, string path)
        {
            //构造key
            var key = string.Format("{0}:{1}", plugin, path);
            //然后查找控制器
            if (!controllerDict.ContainsKey(key))
                return Next.Invoke(context);
            T controller = controllerDict[key];
            return ExecuteController(controller, context, plugin, path);
        }

        public abstract Task ExecuteController(T controller, IOwinContext context, string plugin, string path);
    }
}
