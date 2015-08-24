using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.Hosting;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Controller.Impl;
using System.Reflection;
using Quick.OwinMVC.Routing;
using Quick.OwinMVC.View;
using Quick.OwinMVC.Resource;
using System.Net;

namespace Quick.OwinMVC
{
    public class Server
    {
        public const String PLUGIN_ALIAS_DICT_KEY = "Quick.OwinMVC.Server.PluginAliasDict";

        private String url;
        private IViewRender viewRender;
        private IDisposable webApp;

        static Server()
        {
            //注册embed:前缀URI处理程序
            WebRequest.RegisterPrefix("embed:", new EmbedWebRequestFactory());
            //注册resource:前缀URI处理程序
            WebRequest.RegisterPrefix("resource:", new ResourceWebRequestFactory());
        }

        public Server(String url, IViewRender viewRender)
        {
            this.url = url;
            this.viewRender = viewRender;
        }

        public void Start()
        {
            webApp = WebApp.Start(url, app =>
            {
#if DEBUG
                app.UseErrorPage();
#endif
                app.Use<Controller.Middleware>(viewRender);
            }
            );
        }

        public void Stop()
        {
            webApp.Dispose();
        }
    }
}
