using System;
using System.IO;
using System.Collections.Generic;
using Quick.OwinMVC.Startup.Buttons;

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
                GetControlConfigFunc = () => new KeyValuePair<string, object>[]
                 {
                    new KeyValuePair<string, object>("版本号",CommonButtons.Instance.VersionLabel),
                    new KeyValuePair<string, object>("服务状态",ServiceButtons.Instance.StatusLabel),
                    new KeyValuePair<string, object>("启动",ServiceButtons.Instance.StartButton),
                    new KeyValuePair<string, object>("停止",ServiceButtons.Instance.StopButton),
                    new KeyValuePair<string, object>("服务配置",null),
                    new KeyValuePair<string, object>("安装",ServiceButtons.Instance.InstallButton),
                    new KeyValuePair<string, object>("卸载",ServiceButtons.Instance.UninstallButton),
                    new KeyValuePair<string, object>(null,null),
                    new KeyValuePair<string, object>("常用工具",null),
                    new KeyValuePair<string, object>("设置",CommonButtons.Instance.Action_Setting),
                    new KeyValuePair<string, object>("调试运行",CommonButtons.Instance.Action_Debug),
                    new KeyValuePair<string, object>(null,null),
                    new KeyValuePair<string, object>("退出",CommonButtons.Instance.Action_Exit)
                 }
            };
#if DEBUG
            //修改调试的WEB服务端口为8094
            var webServerUriKey = string.Format("{0}.{1}", typeof(Startup.Service.Impl.WebServerService).FullName, "WebServerUri");
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
            Console.WriteLine(string.Format(@"发生严重未处理异常.
操作系统:{0}
CLR版本:{1}
是否64位系统:{2}
是否64位进程:{3}
异常:{4}{5}",
                Environment.OSVersion,
                Environment.Version,
                Environment.Is64BitOperatingSystem,
                Environment.Is64BitProcess,
                Environment.NewLine,
                e.ExceptionObject));
        }
    }
}
