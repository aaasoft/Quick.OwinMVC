using ServerManage.Static;
using System;

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
