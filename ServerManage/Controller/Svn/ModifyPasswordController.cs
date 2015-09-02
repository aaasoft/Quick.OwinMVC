using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;

namespace ServerManage.Controller.Svn
{
    [Route("svn.modify_password")]
    public class ModifyPasswordController : IViewController
    {
        public void Init(IDictionary<string, string> properties) { }

        public string Service(IOwinContext context, IDictionary<string, object> data)
        {
            return "svn/modify_password";
        }
    }
}
