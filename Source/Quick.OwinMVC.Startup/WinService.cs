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
using Quick.OwinMVC.Plugin;
using Quick.OwinMVC.Service;
using System.IO.Pipes;

namespace Quick.OwinMVC.Startup
{
    public partial class WinService : ServiceBase
    {
        private IPluginActivator[] pluginActivators = null;

        public WinService()
        {
            InitializeComponent();
            //添加WEB服务器服务
            ServiceManager.Instance.Register(new WebServerService());
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }

        private NamedPipeServerStream createNewNamedPipedServerStream(String pipeName)
        {
            return new NamedPipeServerStream(
                    pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);
        }

        private void ensureOnlyOne()
        {
            var serviceName = new WinServiceInstaller().ServiceName;
            var pipeName = $"{this.GetType().FullName}.{serviceName}";
            try
            {
                var serverStream = createNewNamedPipedServerStream(pipeName);
                AsyncCallback ac = null;
                ac = ar =>
                {
                    serverStream.Close();
                    serverStream = createNewNamedPipedServerStream(pipeName);
                    serverStream.BeginWaitForConnection(ac, null);
                };
                serverStream.BeginWaitForConnection(ac, null);
            }
            catch
            {
                try
                {
                    var clientStream = new NamedPipeClientStream(pipeName);
                    clientStream.Connect();
                    clientStream.Close();
                }
                finally
                {
                    Console.WriteLine("程序已经启动，将在10秒后退出。");
                    Task.Delay(10 * 1000).Wait();
                    Environment.Exit(0);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            ensureOnlyOne();
            if (!ProgramUtils.IsMonoRuntime()
                && ProgramUtils.IsRuningOnWindows()
                && Environment.Version < Version.Parse("4.0.30319.17929"))
                throw new ApplicationException("需要安装4.5或更新版本的Microsoft .NET Framework才能运行此程序！");

            if (Entrance.Parameter.OnServiceStarting != null)
                Entrance.Parameter.OnServiceStarting.Invoke();

            if (Entrance.Parameter.LoadAllPlugins)
            {
                pluginActivators = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(ass => ass.GetTypes().FirstOrDefault(t =>
                      !t.IsAbstract && !t.IsNotPublic && t.IsClass
                      && typeof(IPluginActivator).IsAssignableFrom(t)
                    )).Where(t => t != null)
                    .Select(t => (IPluginActivator)Activator.CreateInstance(t)).ToArray();

                //启动所有插件
                foreach (var activator in pluginActivators)
                    activator.Start();
            }

            //启动所有服务
            foreach (var service in ServiceManager.Instance.GetItems())
            {
                Console.Write($"服务[{service.Name}]");
                HunterUtils.TryHunt(service, Entrance.Parameter.Properties);
                Console.Write($"->启动中");
                service.Start();
                Console.WriteLine($"->完成");
            }

            if (Entrance.Parameter.OnServiceStarted != null)
                Entrance.Parameter.OnServiceStarted.Invoke();
        }

        protected override void OnStop()
        {
            if (Entrance.Parameter.OnServiceStoping != null)
                Entrance.Parameter.OnServiceStoping.Invoke();

            if (Entrance.Parameter.LoadAllPlugins)
            {
                //停止所有插件
                foreach (var activator in pluginActivators)
                    activator.Stop();
            }

            //停止所有服务
            foreach (var service in ServiceManager.Instance.GetItems())
            {
                service.Stop();
            }

            if (Entrance.Parameter.OnServiceStoped != null)
                Entrance.Parameter.OnServiceStoped.Invoke();

            Environment.Exit(0);
        }
    }
}
