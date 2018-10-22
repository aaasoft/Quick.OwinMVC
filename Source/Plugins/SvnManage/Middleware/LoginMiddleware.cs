using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC;

namespace SvnManage.Middleware
{
    public class LoginMiddleware : OwinMiddleware
    {
        private const String loginPath = "/login";
        private readonly String[] allowPaths =
        {
            loginPath,
            "/SvnManage/api/language",
            "/SvnManage/api/login",
        };
        internal const String LOGINED_USER_KEY = "SVN_USER";
        internal const String RETURN_URL_KEY = "returnUrl";
        private Encoding encoding = new UTF8Encoding(false);

        public LoginMiddleware(OwinMiddleware next = null) : base(next) { }

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
            var contextPath = req.Get<String>("ContextPath");
            var requestPath = sourceRequestPath;
            if (contextPath.Length < sourceRequestPath.Length)
                requestPath = sourceRequestPath.Substring(contextPath.Length);
            //如果是登录页面，则允许访问
            if (allowPaths.Contains(requestPath))
                return Next.Invoke(context);
            else
            //否则
            {
                //如果是API访问，则返回带错误信息的JSON数据
                var strs = sourceRequestPath.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (strs.Length > 2 && strs[1] == "api")
                {
                    var rep = context.Response;
                    var result = ApiResult.Error(-1, "当前未登录！").ToString();
                    rep.ContentType = "application/json; charset=UTF-8";
                    byte[] content = encoding.GetBytes(result);
                    rep.ContentLength = content.Length;
                    return context.Response.WriteAsync(content);
                }
                //跳转到登录页面
                return Task.Factory.StartNew(() =>
                {
                    var rep = context.Response;
                    if (sourceRequestPath != null)
                        req.Set("owin.RequestPath", sourceRequestPath);
                    var returnUrl = req.Uri.PathAndQuery;
                    //对URL进行编码
                    returnUrl = System.Web.HttpUtility.UrlEncode(returnUrl);
                    var redirectUrl = $"{req.Get<String>("ContextPath")}{loginPath}?{RETURN_URL_KEY}={returnUrl}";
                    rep.Redirect(redirectUrl);
                });
            }
        }
    }
}
