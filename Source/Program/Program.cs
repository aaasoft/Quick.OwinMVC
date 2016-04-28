using Quick.OwinMVC.Program.Static;
using Quick.OwinMVC.Program.Utils;
using System;

namespace Quick.OwinMVC.Program
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(String[] args)
        {
            //初始化程序集自动搜索器
            AssemblyAutoSearcher.Init();

            //如果没有带参数，则启动控制WinForm界面
            if (args == null || args.Length == 0)
            {
                if (ProgramUtils.IsRuningOnWindows())
                {
                    WinFormLauncher.Launch();
                    return;
                }
                else
                {
                    while (true)
                    {
                        Console.WriteLine("Please select: 1:RunAsWinForm  2:RunAsDebug");
                        var inLine = Console.ReadLine();
                        if (inLine == "1")
                        {
                            WinFormLauncher.Launch();
                            return;
                        }
                        else if (inLine == "2")
                        {
                            DebugLauncher.Launch();
                            return;
                        }
                    }
                }
            }

            String firstArg = args[0].ToLower();
            switch (firstArg)
            {
                case "-debug":
                    DebugLauncher.Launch();
                    break;
                case "-service":
                    ServiceLauncher.Launch();
                    break;
                case "-install":
                    WinServiceInstaller.Install();
                    break;
                case "-uninstall":
                    WinServiceInstaller.Uninstall();
                    break;
                case "-start":
                    WinServiceInstaller.Start();
                    break;
                case "-stop":
                    WinServiceInstaller.Stop();
                    break;
            }
        }
    }
}
