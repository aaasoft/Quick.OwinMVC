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
using SvnManage.Middleware;

namespace SvnManage.Controller
{
    [Route("index")]
    public class IndexApiController : ApiController
    {
        private UnitStringConverting storageUnitStringConverting = UnitStringConverting.StorageUnitStringConverting;

        private String computer_name, os_name;
        public IndexApiController()
        {
            computer_name = SystemInfoUtils.GetComputerName();
            os_name = SystemInfoUtils.GetOsName();
        }

        protected override object doGet(IOwinContext context)
        {
            switch (context.Request.Query["type"])
            {
                case "user":
                    return new
                    {
                        user = context.GetSession()[LoginMiddleware.LOGINED_USER_KEY]
                    };
                case "info":
                    return new
                    {
                        time = Convert.ToInt64(TimeUtils.GetTime(DateTime.Now)),
                        basic = new
                        {
                            computer_name,
                            os_name,
                            process_run_time = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss")
                        }
                    };
                case "cpu":
                    return new
                    {
                        time = Convert.ToInt64(TimeUtils.GetTime(DateTime.Now)),
                        cpu = new
                        {
                            used = SystemInfoUtils.GetCpuUsage(),
                            temp = SystemInfoUtils.GetCpuTempature()
                        }
                    };
                case "memory":
                    return new
                    {
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
                case "regex":
                    {
                        var content = "0.233081";
                        var regex = new System.Text.RegularExpressions.Regex(@"^\s*(?'value'.*?)\s*$");
                        var match = regex.Match(content);
                        if (match == null || !match.Success)
                            return null;
                        var value = match.Groups["value"].Value;
                        return new
                        {
                            value
                        };
                    }
                case "process":
                    {
                        var process = new Process();
                        var psi = process.StartInfo;
                        psi.FileName = "bash";
                        psi.Arguments = "-c \"grep 'cpu ' /proc/stat| awk '{value=($2+$4)*100/($2+$4+$5)} END {print value}'\"";

                        psi.UseShellExecute = false;
                        psi.RedirectStandardOutput = true;

                        process.Start();
                        var content = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();

                        return new
                        {
                            content
                        };
                    }
            }
            return null;
        }
    }
}
