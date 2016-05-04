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
            Startup.Entrance.Init("Config/Quick.OwinMVC.properties");
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionCallbackFun;
            Startup.Entrance.Start(args);
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
