using Quick.OwinMVC.Test.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            IDictionary<String, String> properties = new Dictionary<String, String>
            {
                //Quick.OwinMVC Server配置部分
                [Server.VIEWRENDER_CLASS] = "Quick.OwinMVC.View.NVelocity.ViewRender; Quick.OwinMVC.View.NVelocity",

                //NVelocity配置部分
                ["resource.loader"] = "class",
                ["class.resource.loader.class"] = "Quick.OwinMVC.View.NVelocity.ResourceLoaders.EmbedResourceLoader; Quick.OwinMVC.View.NVelocity",
                ["velocimacro.library"] = "Quick.OwinMVC.Test:vm_global_library",
            };

            Server server = new Server("http://*:20001", properties);
            //注册中间件
            //server.RegisterMiddleware<LoginMiddleware>();
            server.Start();
            Console.WriteLine("WebServer is started.");
            Console.ReadKey();
        }
    }
}
