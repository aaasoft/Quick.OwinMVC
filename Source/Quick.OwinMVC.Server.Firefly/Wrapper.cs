using Firefly.Http;
using Quick.OwinMVC.WebServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

        public void Start(Func<IDictionary<string, object>, Task> app, IPEndPoint endpoint)
        {
            webApp = new ServerFactory().Create(app, endpoint);
        }

        public void Stop()
        {
            Dispose();
        }
    }
}
