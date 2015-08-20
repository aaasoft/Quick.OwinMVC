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
                Controller.MvcHttpController mvcHttpController = new Controller.MvcHttpController();
                String pluginKey = Controller.Middleware.QOMVC_PLUGIN_KEY;
                String pathKey = Controller.Middleware.QOMVC_PATH_KEY;
                app.Use<Controller.Middleware>(new Dictionary<String, Controller.IHttpController>
                {
                    ["/"] = mvcHttpController,
                    [$"/:{pluginKey}/view/:{pathKey}"] = mvcHttpController,
                    [$"/:{pluginKey}/resource/:{pathKey}"] = new Controller.ResourceHttpController(),
                    [$"/:{pluginKey}/api/:{pathKey}"] = new Controller.ApiHttpController()
                });
            }
            );
        }

        public void Stop()
        {
            webApp.Dispose();
        }
    }
}
