using Quick.OwinMVC.Startup.Static;
using Quick.OwinMVC.Startup.Utils;
using System;

namespace Quick.OwinMVC.Startup
{
    public static class Entrance
    {
        public static String ConfigFilePath = "Config/app.properties";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        public static void Start(String[] args)
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
                    WinServiceInstaller.Instance.Install();
                    break;
                case "-uninstall":
                    WinServiceInstaller.Instance.Uninstall();
                    break;
                case "-start":
                    WinServiceInstaller.Instance.Start();
                    break;
                case "-stop":
                    WinServiceInstaller.Instance.Stop();
                    break;
            }
        }
    }
}
