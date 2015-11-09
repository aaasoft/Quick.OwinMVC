using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;
using SvnManage.Middleware;
using Quick.OwinMVC.Localization;

namespace SvnManage.Controller
{
    /// <summary>
    /// 登录控制器
    /// </summary>
    [Route("login")]
    public class LoginController : ViewController
    {
        [TextResource]
        public enum Texts
        {
            [Text("用户名密码验证失败!")]
            ERROR_USER_PASSWORD_INCORRECT
        }

        protected override string doGet(IOwinContext context, IDictionary<string, object> data)
        {
#if DEBUG
            //data["account"] = "test";
            //data["password"] = "test";
#endif
            preperaData(context, data, null);
            return base.doGet(context, data);
        }

        protected override string doPost(IOwinContext context, IDictionary<string, object> data)
        {
            var req = context.Request;
            var session = context.GetSession();
            var formData = context.GetFormData();
            var arg_password = formData["password"];
            if (arg_password == null)
            {
                preperaData(context, data, null);
                return base.doPost(context, data);
            }
            String arg_password_str = Encoding.UTF8.GetString(Convert.FromBase64String(arg_password));
            String[] args = arg_password_str.Split(':');
            var account = args[0].Trim();
            var password = args[1];

            if (Svn.ApiController.Instance.Check(account, password))
            {
                session[LoginMiddleware.LOGINED_USER_KEY] = account;
                var returnUrl = req.Query.Get(LoginMiddleware.RETURN_URL_KEY);
                if (String.IsNullOrEmpty(returnUrl))
                    returnUrl = "/";
                var rep = context.Response;
                rep.Redirect(returnUrl);
            }

            preperaData(context, data, context.GetText(Texts.ERROR_USER_PASSWORD_INCORRECT));
            return base.doPost(context, data);
        }

        private void preperaData(IOwinContext context, IDictionary<string, object> data, String message)
        {
            var session = context.GetSession();
            var salt = Guid.NewGuid().ToString();
            if (!String.IsNullOrEmpty(message))
            {
                data["login_message"] = message;
            }
        }
    }
}
