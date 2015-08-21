using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Quick.OwinMVC.Controller.Impl
{
    public abstract class ExtendHttpController<T> : HttpController
    {
        protected Encoding encoding = new UTF8Encoding(false);

        private IDictionary<String, T> controllerDict = new Dictionary<String, T>();

        internal void RegisterController(string plugin, string path, T controller)
        {
            controllerDict[$"{plugin}:{path}"] = controller;
        }

        public override void Service(IOwinContext context, string plugin, string path)
        {
            if (!controllerDict.ContainsKey($"{plugin}:{path}"))
            {
                context.Response.StatusCode = 404;
                context.Response.Write($"Controller '{path}' in plugin '{plugin}' not found!");
                return;
            }
            T controller = controllerDict[$"{plugin}:{path}"];
            ExecuteController(controller, context);
        }

        public abstract void ExecuteController(T controller, IOwinContext context);
    }
}
