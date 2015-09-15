using Quick.OwinMVC.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin;

namespace Quick.OwinMVC.Controller
{
    [Route("error")]
    public class ErrorController : IViewController
    {
        public string Service(IOwinContext context, IDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }
    }
}
