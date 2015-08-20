using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.Hosting;

namespace Quick.OwinMVC
{
    public class Server
    {
        private String url;
        private IDisposable webApp;

        public Server(String url)
        {
            this.url = url;
        }

        public void Start()
        {
            webApp = WebApp.Start(url, app =>
            {
#if DEBUG
                app.UseErrorPage();
#endif
                app.UseWelcomePage();
            }
            );
        }

        public void Stop()
        {
            webApp.Dispose();
        }
    }
}
