using Quick.OwinMVC;
using ServerManage.Utils;
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
            //启动服务
            server.Start();
            Console.WriteLine($"ServerManage WebServer is started on {httpUrl}.");
        }
    }
}
