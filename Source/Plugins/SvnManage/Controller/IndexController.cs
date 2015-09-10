using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;
using System.Diagnostics;
using SvnManage.Utils;

namespace SvnManage.Controller
{
    [Route("index")]
    public class IndexController : IViewController, IApiController
    {
        private String refreshInterval;

        public void Init(IDictionary<string, string> properties)
        {
            refreshInterval = properties["SvnManage.Controller.IndexController.refreshInterval"];
        }

        string IViewController.Service(IOwinContext context, IDictionary<String, Object> data)
        {
            data["refreshInterval"] = refreshInterval;
            return "index";
        }

        object IApiController.Service(IOwinContext context)
        {
            switch (context.Request.Query["type"])
            {
                case "info":
                    return new
                    {
                        time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        basic = new
                        {
                            computer_name = SystemInfoUtils.GetComputerName(),
                            os_name = SystemInfoUtils.GetOsName(),
                            process_run_time = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss"),
                            server_run_time = new TimeSpan(0, 0, Environment.TickCount / 1000).ToString(@"dd\.hh\:mm\:ss")
                        },
                        cpu = new
                        {
                            used = SystemInfoUtils.GetCpuUsage()
                        },
                        memory = new
                        {
                            total = SystemInfoUtils.GetTotalMemory(),
                            free = SystemInfoUtils.GetFreeMemory(),
                        }
                    };
                case "disk":
                    return System.IO.DriveInfo.GetDrives().Where(t => t.IsReady).Select(t => new
                    {
                        name = t.Name,
                        totalSize = t.TotalSize,
                        totalFreeSpace = t.TotalFreeSpace,
                        driveFormat = t.DriveFormat
                    });
            }
            return null;
        }
    }
}
