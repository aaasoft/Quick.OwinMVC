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
                ["Quick.OwinMVC.VIEWRENDER_CLASS"] = "Quick.OwinMVC.View.NVelocity.ViewRender; Quick.OwinMVC.View.NVelocity",

                //NVelocity配置部分
                ["resource.loader"] = "class",
                ["class.resource.loader.class"] = "Quick.OwinMVC.View.NVelocity.ResourceLoaders.EmbedResourceLoader; Quick.OwinMVC.View.NVelocity",
                ["velocimacro.library"] = "Quick.OwinMVC.Test:vm_global_library",
            };

            Server server = new Server(properties, 2001, "localhost");
            server.OutputErrorToResponse = true;
            //注册中间件
            server.RegisterMiddleware<LoginMiddleware>();
            //注册重定向和重写
            server.RegisterRedirect("/", "/test/view/index");
            server.RegisterRewrite("/favicon.ico", "/test/resource/favicon.ico");
            //启动服务
            server.Start();
            Console.WriteLine($"WebServer is started as {server.GetUrl()}.");
            Console.ReadKey();
        }
    }
}
