using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC.Controller;

namespace ServerManage.Middleware
{
    public class LoginMiddleware : OwinMiddleware
    {
        public LoginMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            Boolean allowAccess = false;
            //如果Session中已经有账号信息
            if (context.GetSession().ContainsKey("account"))
            {
                allowAccess = true;
            }
            else
            {
                var authorization = context.Request.Headers.Get("Authorization");
                if (authorization != null)
                {
                    var str = Encoding.Default.GetString(Convert.FromBase64String(authorization.Split(' ')[1]));
                    var index = str.IndexOf(":");
                    var account = str.Substring(0, index);
                    var password = str.Substring(index + 1);

                    if (Controller.Svn.ApiController.Instance.Check(account, password))
                    {
                        allowAccess = true;
                        context.GetSession()["account"] = account;
                    }
                }
            }
            //如果允许访问
            if (allowAccess)
            {
                context.Set<String>("account", context.GetSession()["account"].ToString());
                return Next.Invoke(context);
            }
            return AuthorizationRequired(context);
        }

        public static Task AuthorizationRequired(IOwinContext context)
        {
            //提示输入用户名和密码
            var rep = context.Response;
            rep.Headers["WWW-Authenticate"] = "Basic realm=\"LonCom Tech\"";
            rep.StatusCode = 401;
            rep.ContentType = "text/html; charset=UTF-8";
            return rep.WriteAsync(@"
<html><head>
<title>401 需要授权</title>
</head><body>
<h1>需要授权</h1>
<p>当前服务器无法验证您是否有访问此页面的权限。
或者您提供了错误的认证信息（比如：错误的密码），
或者您的浏览器不支持提供认证信息。</p>
<p>This server could not verify that you
are authorized to access the document
requested.  Either you supplied the wrong
credentials (e.g., bad password), or your
browser doesn't understand how to supply
the credentials required.</p>
</body></html>
");
        }
    }
}
