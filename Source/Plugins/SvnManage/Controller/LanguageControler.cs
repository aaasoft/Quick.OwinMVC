using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin;
using Quick.OwinMVC.Localization;
using System.Globalization;
using System.Threading.Tasks;

namespace SvnManage.Controller
{
    [Route("language")]
    public class LanguageControler : ApiController
    {
        protected override async Task<object> doGet(IOwinContext context)
        {
            return await Task.FromResult(TextManager.GetLanguages().Select(t => new
            {
                key = t,
                value = new CultureInfo(t).NativeName
            }));
        }
    }
}
