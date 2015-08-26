using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;
using System.Diagnostics;

[assembly: Route("test")]

namespace Quick.OwinMVC.Test.Controller
{
    [Route("/")]
    [Route("index")]
    public class IndexController : IMvcController
    {
        private Microsoft.VisualBasic.Devices.Computer computer = new Microsoft.VisualBasic.Devices.Computer();

        public string Service(IOwinContext context, IDictionary<String, Object> data)
        {
            data["computerInfo"] = new
            {
                computer_name = computer.Name,
                os_name = computer.Info.OSFullName,
                process_run_time = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss")
            };
            return "index";
        }
    }
}
