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
                ["file.resource.loader.path"] = "Views",
                ["resource.loader"] = "class",
                ["class.resource.loader.class"] = "Quick.OwinMVC.View.NVelocity.ResourceLoaders.EmbedResourceLoader; Quick.OwinMVC.View.NVelocity",
                ["velocimacro.library"] = "Quick.OwinMVC.Test:vm_global_library",
            };

            Server server = new Server("http://*:2001", properties);
            server.Start();
            Console.WriteLine("WebServer is started.");
            Console.ReadKey();
        }
    }
}
