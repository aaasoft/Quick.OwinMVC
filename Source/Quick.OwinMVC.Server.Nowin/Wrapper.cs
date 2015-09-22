using Quick.OwinMVC.WebServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Nowin;

namespace Quick.OwinMVC.Server.Nowin
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
            var builder = ServerBuilder.New().SetEndPoint(endpoint).SetOwinApp(app);
            //builder.SetCertificate(new X509Certificate2("certificate.pfx", "password"));
            webApp = builder.Start();
        }

        public void Stop()
        {
            Dispose();
        }
    }
}
