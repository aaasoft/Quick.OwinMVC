using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quick.OwinMVC.Controller;
using System.Threading.Tasks;

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
            if (session == null)
                return Next.Invoke(context);
            if (session.ContainsKey("UserId"))
                return Next.Invoke(context);
            session["UserId"] = Guid.NewGuid().ToString();
            return context.Response.WriteAsync("Login success,please refresh.");
        }
    }
}
