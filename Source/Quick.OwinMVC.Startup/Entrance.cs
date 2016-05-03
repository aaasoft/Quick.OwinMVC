using Quick.OwinMVC.Startup.Static;
using Quick.OwinMVC.Startup.Utils;
using Quick.OwinMVC.Utils;
using System;
using System.Collections.Generic;

namespace Quick.OwinMVC.Startup
{
    public static class Entrance
    {
        /// <summary>
        /// Quick.OwinMVC配置文件路径
        /// </summary>
        public static String ConfigFilePath { get; private set; }

        public static IDictionary<String, String> Property { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init(String configFilePath)
        {
            Entrance.ConfigFilePath = configFilePath;
            //初始化程序集自动搜索器
            AssemblyAutoSearcher.Init();
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        public static void Start(String[] args)
        {
            Property = PropertyUtils.LoadFile(ConfigFilePath);

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
                    new WinServiceInstaller().Install();
                    break;
                case "-uninstall":
                    new WinServiceInstaller().Uninstall();
                    break;
                case "-start":
                    new WinServiceInstaller().Start();
                    break;
                case "-stop":
                    new WinServiceInstaller().Stop();
                    break;
            }
        }
    }
}
