using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class RewriteMiddleware : OwinMiddleware
    {
        private IDictionary<String, String> rewriteDict;
        public RewriteMiddleware(OwinMiddleware next, IDictionary<String, String> redirectDict) : base(next)
        {
            this.rewriteDict = redirectDict;
        }

        public override Task Invoke(IOwinContext context)
        {
            String path = context.Get<String>("owin.RequestPath");
            if (rewriteDict.ContainsKey(path))
                context.Set<String>("owin.RequestPath", rewriteDict[path]);
            return Next.Invoke(context);
        }
    }
}
