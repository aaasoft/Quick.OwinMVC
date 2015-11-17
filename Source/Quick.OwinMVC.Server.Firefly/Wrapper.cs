using Firefly.Http;
using Quick.OwinMVC.WebServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.Builder;

namespace Quick.OwinMVC.Server.Firefly
{
    public class Wrapper : IWebServer
    {
        private IDisposable webApp;
        public bool IsRuning { get { return webApp != null; } }

        public void Dispose()
        {
            if (webApp != null)
                webApp.Dispose();
            webApp = null;
        }

        public void Start(Action<IAppBuilder> app, IPEndPoint endpoint)
        {
            AppBuilder appBuilder = new AppBuilder();
            app(appBuilder);
            webApp = new ServerFactory().Create(appBuilder.Build(), endpoint);
        }

        public void Stop()
        {
            Dispose();
        }
    }
}
