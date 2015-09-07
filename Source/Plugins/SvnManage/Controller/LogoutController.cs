using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin;

namespace SvnManage.Controller
{
    [Route("logout")]
    public class LogoutController : IViewController
    {
        public void Init(IDictionary<string, string> properties)
        {
        }

        public string Service(IOwinContext context, IDictionary<string, object> data)
        {
            var rep = context.Response;
            context.GetSession().Clear();
            rep.Redirect("../../");
            return null;
        }
    }
}
