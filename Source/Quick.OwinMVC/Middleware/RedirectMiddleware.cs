using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class RedirectMiddleware : OwinMiddleware, IPropertyHunter
    {
        private IDictionary<String, String> redirectDict;
        public RedirectMiddleware(OwinMiddleware next) : base(next)
        {
            redirectDict = new Dictionary<String, String>();
        }

        /// <summary>
        /// 注册重定向
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="desPath"></param>
        public void RegisterRedirect(String srcPath, String desPath)
        {
            redirectDict[srcPath] = desPath;
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

        public void Hunt(string key, string value)
        {
            RegisterRedirect(key, value);
        }
    }
}
