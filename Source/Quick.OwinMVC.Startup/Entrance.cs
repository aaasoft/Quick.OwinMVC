using Quick.OwinMVC.Startup.Static;
using Quick.OwinMVC.Startup.Utils;
using Quick.OwinMVC.Utils;
using System;
using System.Collections.Generic;

namespace Quick.OwinMVC.Startup
{
    public static class Entrance
    {
        public static EntranceParameter Parameter { get; private set; }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        public static void Start(EntranceParameter parameter)
        {
            Parameter = parameter;

            //初始化程序集自动搜索器
            AssemblyAutoSearcher.Init(parameter.LoadAllPlugins);

            var args = parameter.StartupArguments;
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
                    DebugLauncher.Launch();
                    return;
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
