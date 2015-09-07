using Quick.OwinMVC;
using Quick.OwinMVC.Routing;
using ServerManage.Static;
using ServerManage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerManage
{
    class Program
    {
        static void Main(string[] args)
        {
            AssemblyAutoSearcher.Init();
            Startup.Start();
            Console.ReadLine();
        }
    }
}
