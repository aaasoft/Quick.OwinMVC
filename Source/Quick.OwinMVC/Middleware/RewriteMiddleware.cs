using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class RewriteMiddleware : OwinMiddleware, IPropertyHunter
    {
        public const String REWRITE_PREFIX = "Quick.OwinMVC.Server.Rewrite.";
        private IDictionary<String, String> rewriteDict;
        public RewriteMiddleware(OwinMiddleware next) : base(next)
        {
            this.rewriteDict = new Dictionary<String, String>();
        }

        /// <summary>
        /// 注册重写
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="desPath"></param>
        public void RegisterRewrite(String srcPath, String desPath)
        {
            rewriteDict[srcPath] = desPath;
        }

        public override Task Invoke(IOwinContext context)
        {
            String path = context.Get<String>("owin.RequestPath");
            if (rewriteDict.ContainsKey(path))
                context.Set<String>("owin.RequestPath", rewriteDict[path]);
            return Next.Invoke(context);
        }

        public void Hunt(string key, string value)
        {
            if (key.StartsWith(REWRITE_PREFIX))
                RegisterRewrite(key.Substring(REWRITE_PREFIX.Length), value);
        }
    }
}
