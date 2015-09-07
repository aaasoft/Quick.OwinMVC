using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;
using System.Diagnostics;

namespace SvnManage.Controller
{
    [Route("index")]
    public class IndexController : IViewController, IApiController
    {
        private String refreshInterval;

        private Microsoft.VisualBasic.Devices.Computer computer;
        private PerformanceCounter cpuCounter;

        public void Init(IDictionary<string, string> properties)
        {
            refreshInterval = properties["SvnManage.Controller.IndexController.refreshInterval"];

            computer = new Microsoft.VisualBasic.Devices.Computer();

            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            cpuCounter.NextValue();
        }

        public string Service(IOwinContext context, IDictionary<String, Object> data)
        {
            data["refreshInterval"] = refreshInterval;
            return "index";
        }

        public object Service(IOwinContext context)
        {
            switch (context.Request.Query["type"])
            {
                case "info":
                    return new
                    {
                        time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        basic = new
                        {
                            computer_name = computer.Name,
                            os_name = computer.Info.OSFullName,
                            process_run_time = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss")
                        },
                        cpu = new
                        {
                            used = cpuCounter.NextValue()
                        },
                        memory = new
                        {
                            total = computer.Info.TotalPhysicalMemory,
                            free = computer.Info.AvailablePhysicalMemory,
                            used = computer.Info.TotalPhysicalMemory - computer.Info.AvailablePhysicalMemory
                        }
                    };
            }
            return null;
        }
    }
}
