using Microsoft.Owin;
using Quick.OwinMVC.Hunter;
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
            if (!Server.Instance.IsRootContextPath)
            {
                var tmpContextPath = Server.Instance.ContextPath.Substring(0, Server.Instance.ContextPath.Length - 1);
                srcPath = string.Format("{0}{1}",tmpContextPath,srcPath);
                desPath = string.Format("{0}{1}",tmpContextPath,desPath);
            }
            redirectDict[srcPath] = desPath;
        }

        public override Task Invoke(IOwinContext context)
        {
            String path = context.Get<String>("owin.RequestPath");
            if (redirectDict.ContainsKey(path))
            {
                var req = context.Request;
                var rep = context.Response;                
                var desPath = redirectDict[path] + req.QueryString.ToString();
                rep.Redirect(desPath);
                rep.ContentLength = 0;
                return rep.WriteAsync(String.Empty);
            }
            return Next.Invoke(context);
        }

        public void Hunt(string key, string value)
        {
            RegisterRedirect(key, value);
        }
    }
}
