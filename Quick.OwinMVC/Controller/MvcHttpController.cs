using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Quick.OwinMVC.Controller
{
    public class MvcHttpController : HttpController
    {
        public override void Service(IOwinContext context)
        {
            base.Service(context);
        }
    }
}
