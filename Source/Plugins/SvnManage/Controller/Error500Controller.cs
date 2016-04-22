using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace SvnManage.Controller
{
    [Route("500")]
    public class Error500Controller : ApiController
    {
        protected override object doGet(IOwinContext context)
        {
            throw new System.IO.IOException("此方法未实现！");
        }
    }
}
