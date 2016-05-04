using Microsoft.Owin;
using Quick.OwinMVC.Manager;
using Quick.OwinMVC.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvnManage
{
    public class Activator : IPluginActivator
    {
        private OwinMiddleware middleware = null;

        public Activator()
        {
            middleware = new Middleware.LoginMiddleware();
        }

        public void Start()
        {
            PreMiddlewareManager.Instance.Register(middleware);
        }

        public void Stop()
        {
            PreMiddlewareManager.Instance.Unregister(middleware);
        }
    }
}
