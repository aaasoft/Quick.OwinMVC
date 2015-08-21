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

namespace Quick.OwinMVC
{
    public class Server
    {
        private String url;
        private IDisposable webApp;

        private ApiHttpController apiController;
        private MvcHttpController mvcHttpController;

        public Server(String url, IViewRender viewRender)
        {
            this.url = url;
            apiController = new ApiHttpController();
            mvcHttpController = new MvcHttpController(viewRender);
        }

        public void RegisterMvcController(String plugin, String path, IMvcController controller)
        {

        }

        public void RegisterApiController(String plugin, String path, IApiController controller)
        {
            apiController.RegisterController(plugin, path, controller);
        }

        private void scanController()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                String pluginName = assembly.GetName().Name;

                foreach (Type type in assembly.GetTypes())
                {
                    foreach (RouteAttribute attr in type.GetCustomAttributes<RouteAttribute>())
                    {
                        if (typeof(IApiController).IsAssignableFrom(type))
                            RegisterApiController(pluginName, attr.Path, (IApiController)Activator.CreateInstance(type));
                        else
                            RegisterMvcController(pluginName, attr.Path, (IMvcController)Activator.CreateInstance(type));
                    }
                }
            }
        }

        public void Start()
        {
            scanController();
            webApp = WebApp.Start(url, app =>
            {
#if DEBUG
                app.UseErrorPage();
#endif
                String pluginKey = Controller.Middleware.QOMVC_PLUGIN_KEY;
                String pathKey = Controller.Middleware.QOMVC_PATH_KEY;
                app.Use<Controller.Middleware>(new Dictionary<String, Controller.IHttpController>
                {
                    ["/"] = mvcHttpController,
                    [$"/:{pluginKey}/view/:{pathKey}"] = mvcHttpController,
                    [$"/:{pluginKey}/resource/:{pathKey}"] = new ResourceHttpController(),
                    [$"/:{pluginKey}/api/:{pathKey}"] = apiController
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
