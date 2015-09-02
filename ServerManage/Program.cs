using Quick.OwinMVC;
using Quick.OwinMVC.Routing;
using ServerManage.Middleware;
using ServerManage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Route("core")]

namespace ServerManage
{
    class Program
    {
        static void Main(string[] args)
        {
            IDictionary<String, String> properties = PropertyUtils.LoadFile("Quick.OwinMVC.properties");
            String httpUrl = properties["http.url"];
            Server server = new Server(properties, new Uri(httpUrl));
            //启动服务
            server.Start();

            //var abc = server.GetMiddlewareInstance<Quick.OwinMVC.Middleware.RedirectMiddleware>();
            //abc.RegisterRedirect("/", "http://www.baidu.com");

            Console.WriteLine($"ServerManage WebServer is started on {httpUrl}.");
            Console.ReadKey();
        }
    }
}
