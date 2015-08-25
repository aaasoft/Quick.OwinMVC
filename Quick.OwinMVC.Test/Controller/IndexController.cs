using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;

[assembly: Route("test")]

namespace Quick.OwinMVC.Test.Controller
{
    [Route("/")]
    [Route("index")]
    public class IndexController : IMvcController
    {
        public string Service(IOwinContext context, IDictionary<String, Object> data)
        {
            data["currentTime"] = DateTime.Now;
            return "index";
        }
    }
}
