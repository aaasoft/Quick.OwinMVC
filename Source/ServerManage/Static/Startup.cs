using Quick.OwinMVC;
using Quick.OwinMVC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerManage.Static
{
    public class Startup
    {
        public static void Start()
        {
            IDictionary<String, String> properties = PropertyUtils.LoadFile("Quick.OwinMVC.properties");
            String httpUrl = properties["http.url"];
            Server server = new Server(properties, new Uri(httpUrl));

            //server.SetCertificate(new System.Security.Cryptography.X509Certificates.X509Certificate2("device.scbeta.com.pfx", "1qaz2wsx1qaz"));
            //启动服务
            server.Start();
            Console.WriteLine($"ServerManage WebServer is started on {httpUrl}.");
        }
    }
}
