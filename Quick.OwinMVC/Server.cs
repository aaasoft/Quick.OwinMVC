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
using Quick.OwinMVC.Utils;

namespace Quick.OwinMVC
{
    public class Server
    {
        public const String VIEWRENDER_CLASS = "Quick.OwinMVC.VIEWRENDER_CLASS";

        private IDictionary<String, String> properties;
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

        public Server(String url, IDictionary<String, String> properties)
        {
            this.properties = properties;
            this.url = url;

            if (!properties.ContainsKey(VIEWRENDER_CLASS))
                throw new ApplicationException($"Cann't find '{VIEWRENDER_CLASS}' in properties.");
            String viewRenderClassName = properties[VIEWRENDER_CLASS];
            this.viewRender = (IViewRender)AssemblyUtils.CreateObject(viewRenderClassName);
            this.viewRender.Init(properties);
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
