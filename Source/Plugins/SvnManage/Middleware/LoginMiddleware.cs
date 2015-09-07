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
        private const String loginPath = "/base/view/login";
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


            //如果是登录页面，则允许访问
            if (context.Request.Path.Equals(new PathString(loginPath)))
                return Next.Invoke(context);
            else
            //否则，跳转到登录页面
            {
                return Task.Factory.StartNew(() =>
                {
                    var req = context.Request;
                    var rep = context.Response;

                    var returnUrl = req.Uri.ToString();
                    //对URL进行编码
                    returnUrl = System.Web.HttpUtility.UrlEncode(returnUrl);
                    rep.Redirect($"../..{loginPath}?{RETURN_URL_KEY}={returnUrl}");
                });
            }
        }
    }
}
