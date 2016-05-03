using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.OwinMVC.Startup.Static
{
    public static class ServiceLauncher
    {
        public static void Launch()
        {
            WinService service = new WinService();
            System.ServiceProcess.ServiceBase.Run(service);
        }
    }
}
