using System;
using System.IO;
using System.Collections.Generic;

namespace Quick.OwinMVC.Program
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(String[] args)
        {
            var parameter = new Startup.EntranceParameter()
            {
                BasePath = Path.GetDirectoryName(typeof(Program).Assembly.Location),
                ConfigFilePath = "Config/Quick.OwinMVC.properties",
                StartupArguments = args,
                LibsPath = "Libs",
                PluginsPath = "Plugins",
                LoadAllPlugins = true,
                ButtonDict = new Dictionary<String, Action>()
                {
                    ["常用工具"] = null,
                    [Startup.EntranceParameter.SettingAction.Key] = Startup.EntranceParameter.SettingAction.Value,
                    [Startup.EntranceParameter.DebugAction.Key] = Startup.EntranceParameter.DebugAction.Value
                }
            };
#if DEBUG
            //修改调试的WEB服务端口为8094
            var webServerUriKey = $"{typeof(Startup.Service.Impl.WebServerService).FullName}.{nameof(Startup.Service.Impl.WebServerService.WebServerUri)}";
            if (parameter.Properties.ContainsKey(webServerUriKey))
                parameter.Properties[webServerUriKey] = "net://0.0.0.0:8094";
#endif
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionCallbackFun;
            Startup.Entrance.Start(parameter);
        }

        /// <summary>
        /// 未处理异常回掉函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnhandledExceptionCallbackFun(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($@"发生严重未处理异常.
操作系统:{Environment.OSVersion.ToString()}
CLR版本:{Environment.Version.ToString()}
是否64位系统:{Environment.Is64BitOperatingSystem}
是否64位进程:{Environment.Is64BitProcess}
异常:{Environment.NewLine}{e.ExceptionObject.ToString()}");
        }
    }
}
