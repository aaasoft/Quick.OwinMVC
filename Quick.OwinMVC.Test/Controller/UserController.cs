using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Routing;

namespace Quick.OwinMVC.Test.Controller
{
    [Route("User")]
    public class UserController : IApiController
    {
        public object Service(IOwinContext context)
        {
            return new { FirstName = "Hello", LastName = "Quick,OwinMVC" };
        }
    }
}
