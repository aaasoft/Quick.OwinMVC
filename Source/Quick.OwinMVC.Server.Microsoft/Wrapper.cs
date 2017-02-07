using Quick.OwinMVC.WebServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Builder;
using Owin;

namespace Quick.OwinMVC.Server.Microsoft
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
            var url = string.Empty;
            if (endpoint.Address == IPAddress.Any)
            {
#if DEBUG
                endpoint.Address = IPAddress.Loopback;
                url = $"http://{endpoint.Address}:{endpoint.Port}";
#else
                url = $"http://*:{endpoint.Port}";
#endif
            }
            else
                url = $"http://{endpoint.Address}:{endpoint.Port}";
            webApp = WebApp.Start(url, startup =>
            {
                app(startup);
            });
        }

        public void Stop()
        {
            Dispose();
        }
    }
}
