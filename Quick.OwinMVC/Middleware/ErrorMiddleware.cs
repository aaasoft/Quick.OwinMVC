using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class ErrorMiddleware : OwinMiddleware
    {
        public ErrorMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            try
            {
                return Next.Invoke(context);
            }
            catch (Exception ex)
            {
                var rep = context.Response;
                rep.StatusCode = 500;
                rep.ContentType = "text/plain; charset=UTF-8";
                return context.Response.WriteAsync(ex.ToString());
            }
        }
    }
}
