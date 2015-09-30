using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC.Controller;

namespace SvnManage.Middleware
{
    public class LoginMiddleware : OwinMiddleware
    {
        private const String loginPath = "/login";
        private readonly String[] allowPaths = { loginPath, "/base/api/language" };
        internal const String LOGINED_USER_KEY = "SVN_USER";
        internal const String RETURN_URL_KEY = "returnUrl";


        public LoginMiddleware(OwinMiddleware next) : base(next) { }

        public override Task Invoke(IOwinContext context)
        {
            //如果已经登录
            if (context.GetSession().ContainsKey(LOGINED_USER_KEY))
            {
                context.Environment[LOGINED_USER_KEY] = context.GetSession()[LOGINED_USER_KEY];
                return Next.Invoke(context);
            }

            var req = context.Request;
            var sourceRequestPath = req.Get<String>("Quick.OwinMVC.SourceRequestPath");
            if (sourceRequestPath == null)
                sourceRequestPath = req.Get<String>("owin.RequestPath");
            //如果是登录页面，则允许访问
            if (allowPaths.Contains(sourceRequestPath))
                return Next.Invoke(context);
            else
            //否则，跳转到登录页面
            {
                return Task.Factory.StartNew(() =>
                {

                    var rep = context.Response;

                    if (sourceRequestPath != null)
                        req.Set("owin.RequestPath", sourceRequestPath);
                    var returnUrl = req.Uri.ToString();
                    //对URL进行编码
                    returnUrl = System.Web.HttpUtility.UrlEncode(returnUrl);
                    var redirectUrl = $"{req.Get<String>("ContextPath")}{loginPath}?{RETURN_URL_KEY}={returnUrl}";
                    rep.Redirect(redirectUrl);
                });
            }
        }
    }
}
