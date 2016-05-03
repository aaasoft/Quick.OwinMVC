using Quick.OwinMVC.Startup.Service;
using Quick.OwinMVC.Startup.Service.Impl;
using Quick.OwinMVC.Startup.Static;
using Quick.OwinMVC.Startup.Utils;
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

namespace Quick.OwinMVC.Startup
{
    public partial class WinService : ServiceBase
    {
        private List<IService> serviceList = new List<IService>();

        public WinService()
        {
            InitializeComponent();
            //添加WEB服务器服务
            serviceList.Add(new WebServerService());
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }

        protected override void OnStart(string[] args)
        {
            if (ProgramUtils.IsRuningOnWindows() && Environment.Version < Version.Parse("4.0.30319.17929"))
                throw new ApplicationException("需要安装4.5或更新版本的Microsoft .NET Framework才能运行此程序！");

            var properties = PropertyUtils.LoadFile(Entrance.ConfigFilePath);
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
            //停止所有服务
            foreach (var service in serviceList)
            {
                service.Stop();
            }
            Environment.Exit(0);
        }
    }
}
