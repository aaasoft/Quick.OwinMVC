using Microsoft.Owin;
using Quick.OwinMVC.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class PreMiddleware : OwinMiddleware
    {
        public PreMiddleware(OwinMiddleware next) : base(next)
        {
            PreMiddlewareManager.Instance.TailMiddleware = next;
        }

        public override Task Invoke(IOwinContext context)
        {
            return PreMiddlewareManager.Instance.HeadMiddleware.Invoke(context);
        }
    }
}
