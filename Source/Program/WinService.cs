using Quick.OwinMVC.Program.Service;
using Quick.OwinMVC.Program.Service.Impl;
using Quick.OwinMVC.Program.Static;
using Quick.OwinMVC.Program.Utils;
using Quick.OwinMVC.Hunter;
using Quick.OwinMVC.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Program
{
    public partial class WinService : ServiceBase
    {
        private List<IService> serviceList = new List<IService>();

#if DEBUG
        private FileSystemWatcher sourceFileWatcher = null;
#endif
        public WinService()
        {
            InitializeComponent();
            //添加WEB服务器服务
            serviceList.Add(new WebServerService());
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

        public void Start(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionCallbackFun;
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            if (ProgramUtils.IsRuningOnWindows() && Environment.Version < Version.Parse("4.0.30319.17929"))
                throw new ApplicationException("需要安装4.5或更新版本的Microsoft .NET Framework才能运行此程序！");

            //读取全部配置文件
            var properties = PropertyUtils.LoadFile("Config/app.properties");
            #region 开发调试用代码
#if DEBUG
            DirectoryInfo staticFileFolder = new DirectoryInfo(properties["Quick.OwinMVC.Middleware.ResourceMiddleware.StaticFileFolder"]);
            DirectoryInfo sourceCodeFolder = new DirectoryInfo("../../../Source Codes/Main");
            if (sourceCodeFolder.Exists)
            {
                sourceFileWatcher = new FileSystemWatcher(sourceCodeFolder.FullName);
                List<String> monitorFolderList = new List<string>();
                monitorFolderList.AddRange(PathUtils.SearchFolder(Path.Combine(sourceCodeFolder.FullName, "Quick.OwinMVC.Program.Plugin.*", "view")));
                monitorFolderList.AddRange(PathUtils.SearchFolder(Path.Combine(sourceCodeFolder.FullName, "Quick.OwinMVC.Program.Plugin.*", "resource")));
                sourceFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName;

                sourceFileWatcher.Renamed += (sender, e) =>
                {
                    if (!File.Exists(e.FullPath)
                        || e.Name.Contains("~"))
                        return;

                    bool isInMonitorFolderList = false;
                    foreach (var monitorFolder in monitorFolderList)
                    {
                        if (e.FullPath.StartsWith(monitorFolder))
                        {
                            isInMonitorFolderList = true;
                            break;
                        }
                    }
                    if (!isInMonitorFolderList)
                        return;
                    var absPath = e.FullPath.Substring(sourceCodeFolder.FullName.Length);
                    while (absPath.StartsWith(Path.DirectorySeparatorChar.ToString()))
                        absPath = absPath.Substring(1);

                    String desFilePath = Path.Combine(staticFileFolder.FullName, absPath);
                    Task.Factory.StartNew(() =>
                    {
                        //复制文件重试3次
                        for (var i = 0; i < 3; i++)
                        {
                            try
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(desFilePath));
                                File.Copy(e.FullPath, desFilePath, true);
                                Console.WriteLine("[调试模式]已处理资源文件变更：" + e.FullPath.Substring(sourceCodeFolder.FullName.Length));
                                break;
                            }
                            catch { System.Threading.Thread.Sleep(500); }
                        }
                    });
                };
                sourceFileWatcher.IncludeSubdirectories = true;
                sourceFileWatcher.EnableRaisingEvents = true;
            }
#endif
            #endregion
            
            //启动所有服务
            foreach (var service in serviceList)
            {
                Console.Write($"服务[{service.Name}]");
                HunterUtils.TryHunt(service, properties);
                Console.Write($"->启动中");
                service.Start();
                Console.WriteLine($"->完成");
            }
        }

        protected override void OnStop()
        {
#if DEBUG
            if (sourceFileWatcher != null)
            {
                sourceFileWatcher.EnableRaisingEvents = false;
                sourceFileWatcher.Dispose();
                sourceFileWatcher = null;
            }
#endif
            //停止所有服务
            foreach (var service in serviceList)
            {
                service.Stop();
            }
            Environment.Exit(0);
        }
    }
}
