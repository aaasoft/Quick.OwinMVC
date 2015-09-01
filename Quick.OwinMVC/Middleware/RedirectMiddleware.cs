using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class RedirectMiddleware : OwinMiddleware
    {
        private IDictionary<String, String> redirectDict;
        public RedirectMiddleware(OwinMiddleware next) : base(next)
        {
            this.redirectDict = Server.Instance.redirectDict;
        }

        public override Task Invoke(IOwinContext context)
        {
            String path = context.Get<String>("owin.RequestPath");
            if (redirectDict.ContainsKey(path))
            {
                context.Response.Redirect(redirectDict[path]);
                context.Response.ContentLength = 0;
                return context.Response.WriteAsync(String.Empty);
            }
            return Next.Invoke(context);
        }
    }
}
