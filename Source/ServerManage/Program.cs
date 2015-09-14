using ServerManage.Static;
using System;
using System.Threading;

namespace ServerManage
{
    class Program
    {

        static void Main(string[] args)
        {
            AssemblyAutoSearcher.Init();
            Startup.Start();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                Console.ReadLine();
            else
                Thread.CurrentThread.Join();
        }
    }
}
