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
    public class LogoutController : ViewController
    {
        public override string Service(IOwinContext context, IDictionary<string, object> data)
        {
            var rep = context.Response;
            context.GetSession().Clear();
            rep.Redirect(context.Get<String>("ContextPath"));
            return null;
        }
    }
}
