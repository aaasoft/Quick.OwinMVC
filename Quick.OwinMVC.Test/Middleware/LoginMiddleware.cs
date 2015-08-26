using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC.Controller;

namespace Quick.OwinMVC.Test.Middleware
{
    public class LoginMiddleware : OwinMiddleware
    {
        public LoginMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            return Task.Factory.StartNew(() =>
                {
                    var session = context.GetSession();
                    context.Response.Write("I'm a middleware.");
                });
        }
    }
}
