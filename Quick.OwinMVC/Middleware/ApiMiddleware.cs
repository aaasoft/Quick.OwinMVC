using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Quick.OwinMVC.Middleware
{
    public class ApiMiddleware : AbstractPluginPathMiddleware
    {
        public ApiMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override string GetRouteMiddle()
        {
            return "api";
        }

        public override Task Invoke(IOwinContext context, string plugin, string path)
        {
            return Next.Invoke(context);
        }
    }
}
