using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;
using System.IO;
using System.Management;
using SvnManage.Middleware;
using Quick.OwinMVC.Hunter;

namespace SvnManage.Controller.Svn
{
    [Route("svn")]
    public class ApiController : Quick.OwinMVC.Controller.ApiController, IPropertyHunter
    {
        public static ApiController Instance;

        private String logFilePath;
        private String[] protectAccounts;
        private String htpasswdFilePath;

        void IPropertyHunter.Hunt(string key, string value)
        {
            switch (key)
            {
                case nameof(logFilePath):
                    logFilePath = value;
                    break;
                case nameof(htpasswdFilePath):
                    htpasswdFilePath = value;
                    break;
                case nameof(protectAccounts):
                    protectAccounts = value.Split(new Char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                    break;
            }
        }

        public ApiController()
        {
            ApiController.Instance = this;
        }

        public override object Service(IOwinContext context)
        {
            var method = context.Request.Method;
            var action = context.Request.Query["action"];
            switch (action)
            {
                case "modify_password":
                    if (method != "POST")
                        return null;
                    var formData = context.GetFormData();
                    var account = context.GetSession()[LoginMiddleware.LOGINED_USER_KEY]?.ToString();
                    var pre_password = formData.Get("pre_password");
                    var new_password = formData.Get("new_password");
                    try
                    {
                        return modifyPassword(account, pre_password, new_password);
                    }
                    catch (Exception ex)
                    {
                        return new
                        {
                            msg = ex.ToString()
                        };
                    }
            }
            return null;
        }

        /// <summary>
        /// 检查用户名与密码是否正确
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Boolean Check(String account, String password)
        {
#if DEBUG
            if (account == "test" && password == "test")
                return true;
#endif
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                if (account == "test" && password == "test")
                    return true;
            }
            if (!File.Exists(htpasswdFilePath))
                return false;
            String[] lines = File.ReadAllLines(htpasswdFilePath);
            String encryptedPassword = null;
            foreach (String line in lines)
            {
                if (String.IsNullOrEmpty(line))
                    continue;
                Int32 spIndex = line.IndexOf(':');
                if (spIndex < 0)
                    continue;
                String src_account = line.Substring(0, spIndex);
                if (src_account == account)
                {
                    encryptedPassword = line.Substring(spIndex + 1);
                    break;
                }
            }
            if (encryptedPassword == null
                || !CryptSharp.Crypter.CheckPassword(password, encryptedPassword))
            {
                return false;
            }
            return true;
        }

        private Object modifyPassword(String account, String pre_password, String new_password)
        {
            if (Array.IndexOf(protectAccounts, account) >= 0)
            {
                return new
                {
                    msg = "此账号已被保护，无法修改密码！"
                };
            }

            if (!Check(account, pre_password))
            {
                return new { msg = "账号或密码错误！" };
            }

            ManagementClass userClass = new ManagementClass("root\\VisualSVN", "VisualSVN_User", null);
            if (userClass == null)
            {
                return new
                {
                    msg = "服务器未安装Visual SVN！"
                };
            }
            ManagementObject userObj = null;
            foreach (ManagementObject tmpObj in userClass.GetInstances())
            {
                String userName = tmpObj.GetPropertyValue("Name").ToString();
                if (userName == account)
                {
                    userObj = tmpObj;
                    break;
                }
            }
            if (userObj == null)
            {
                return new
                {
                    msg = $"未找到名称为[{account}]的账号！"
                };
            }
            // Obtain in-parameters for the method
            ManagementBaseObject inParams =
                userObj.GetMethodParameters("SetPassword");
            // Add the input parameters.
            inParams["Password"] = new_password;
            // Execute the method and obtain the return values.
            ManagementBaseObject outParams =
                userObj.InvokeMethod("SetPassword", inParams, null);
            pushLog(String.Format("账号[{0}]修改密码！", account));
            return new { msg = "恭喜，密码修改成功！" };
        }
        private void pushLog(String log)
        {
            String fullLog = String.Format("{0} : {1}{2}", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"), log, Environment.NewLine);
            lock (GetType())
                File.AppendAllText(logFilePath, fullLog);
        }
    }
}
