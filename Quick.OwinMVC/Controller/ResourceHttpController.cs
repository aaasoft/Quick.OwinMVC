using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Quick.OwinMVC.Controller
{
    public class ResourceHttpController : HttpController
    {
        public override void DoGet(IOwinContext context)
        {
            context.Response.ContentType = "text/json";
            context.Response.Write(JsonConvert.SerializeObject(context.Environment));
        }
    }
}
