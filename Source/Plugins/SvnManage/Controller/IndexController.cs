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
using Quick.OwinMVC.Utils;
using Quick.OwinMVC.Hunter;

namespace SvnManage.Controller
{
    [Route("index")]
    public class IndexController : ViewController, IPropertyHunter
    {
        private String refreshInterval;

        void IPropertyHunter.Hunt(string key, string value)
        {
            switch (key)
            {
                case nameof(refreshInterval):
                    refreshInterval = value;
                    break;
            }
        }

        protected override string doGet(IOwinContext context, IDictionary<string, object> data)
        {
            var viewData = new
            {
                serverTime = TimeUtils.GetTime(DateTime.Now),
                refreshInterval = refreshInterval
            };
            data.Add(viewData);
            return base.doGet(context, data);
        }
    }

    [Route("index")]
    public class IndexApiController : ApiController
    {
        private UnitStringConverting storageUnitStringConverting = UnitStringConverting.StorageUnitStringConverting;

        protected override object doGet(IOwinContext context)
        {
            switch (context.Request.Query["type"])
            {
                case "info":
                    return new
                    {
                        time = Convert.ToInt64(TimeUtils.GetTime(DateTime.Now)),
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
                    return System.IO.DriveInfo.GetDrives().Where(t => t.IsReady && t.TotalSize > 0).Select(t => new
                    {
                        name = t.Name,
                        totalSize = t.TotalSize,
                        totalSizeString = storageUnitStringConverting.GetString(t.TotalSize),
                        totalUsed = t.TotalSize - t.TotalFreeSpace,
                        totalUsedString = storageUnitStringConverting.GetString(t.TotalSize - t.TotalFreeSpace),
                        driveFormat = t.DriveFormat
                    });
            }
            return null;
        }
    }
}
