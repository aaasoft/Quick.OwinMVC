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
            Server server = new Server("http://*:2001", new Quick.OwinMVC.View.NVelocity.ViewRender());
            server.Start();
            Console.WriteLine("WebServer is started.");
            Console.ReadKey();
        }
    }
}
