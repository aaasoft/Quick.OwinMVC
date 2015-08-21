using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    public interface IHttpController
    {
        void Service(IOwinContext context, String plugin, String path);
    }
}
