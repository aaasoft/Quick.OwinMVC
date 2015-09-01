using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;
using ServerManage.Middleware;

namespace ServerManage.Controller
{
    [Route("logout")]
    public class LogoutController : HttpController
    {
        public override void Service(IOwinContext context, string plugin, string path)
        {
            var rep = context.Response;

            //清除所有Session记录
            context.GetSession().Clear();
            LoginMiddleware.AuthorizationRequired(context).Wait();
        }
    }
}
