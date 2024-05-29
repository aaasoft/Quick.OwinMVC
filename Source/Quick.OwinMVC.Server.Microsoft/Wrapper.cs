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
using System.Net.NetworkInformation;
using System.Security.Principal;

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

        public bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            WindowsIdentity user = null;
            try
            {
                //get the currently logged in user
                user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
            }
            catch (Exception)
            {
                isAdmin = false;
            }
            finally
            {
                if (user != null)
                    user.Dispose();
            }
            return isAdmin;
        }

        public void Start(Action<IAppBuilder> app, IPEndPoint endpoint)
        {
            StartOptions options = new StartOptions();

            if (endpoint.Address == IPAddress.Any)
            {
                if (IsUserAdministrator())
                    options.Urls.Add(string.Format("http://*:{0}", endpoint.Port));
                else
                {
                    endpoint.Address = IPAddress.Loopback;
                    options.Urls.Add(string.Format("http://{0}:{1}", endpoint.Address, endpoint.Port));
                }
            }
            else
                options.Urls.Add(string.Format("http://{0}:{1}", endpoint.Address, endpoint.Port));
            webApp = WebApp.Start(options, startup =>
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
