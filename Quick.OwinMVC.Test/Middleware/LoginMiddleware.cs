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
            var session = context.GetSession();
            if (session.ContainsKey("UserId"))
            {
                return Next.Invoke(context);
            }
            session["UserId"] = Guid.NewGuid().ToString();
            context.Response.Write("Login success,please refresh.");
            return Task.FromResult(0);
        }
    }
}
