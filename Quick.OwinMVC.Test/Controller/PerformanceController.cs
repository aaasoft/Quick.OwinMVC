using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Routing;
using System.Diagnostics;
using System.IO;

namespace Quick.OwinMVC.Test.Controller
{
    [Route("performance")]
    public class PerformanceController : IApiController
    {
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;

        public PerformanceController()
        {
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            cpuCounter.NextValue();

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            ramCounter.NextValue();
        }

        public object Service(IOwinContext context)
        {
            switch (context.Request.Query["type"])
            {
                case "cpu":
                    return new { cpu = cpuCounter.NextValue() };
                case "memory":
                    return new { memory = ramCounter.NextValue() };
                case "disk":
                    return DriveInfo.GetDrives().Where(t => t.IsReady).Select(t => new
                    {
                        Name = t.Name,
                        TotalSize = t.TotalSize,
                        TotalFreeSpace = t.TotalFreeSpace,
                        DriveFormat = t.DriveFormat,
                        DriveType = t.DriveType
                    });
            }
            return null;
        }
    }
}
