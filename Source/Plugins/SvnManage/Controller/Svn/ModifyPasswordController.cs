using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;

namespace SvnManage.Controller.Svn
{
    [Route("svn.modify_password")]
    public class ModifyPasswordController : ViewController
    {
        public string Service(IOwinContext context, IDictionary<string, object> data)
        {
            return "svn/modify_password";
        }
    }
}
