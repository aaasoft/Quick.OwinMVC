using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin;

namespace SvnManage.Controller
{
    [Route("404")]
    public class Error404Controller : IViewController
    {
        public void Init(IDictionary<string, string> properties)
        {
        }

        public string Service(IOwinContext context, IDictionary<string, object> data)
        {
            return "404";
        }
    }
}
