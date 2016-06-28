using Microsoft.Owin;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node
{
    public static class IOwinContextExtension
    {
        public static TextManager GetTextManager(this IOwinContext context)
        {
            return TextManager.GetInstance(context.GetLanguage());
        }
    }
}
