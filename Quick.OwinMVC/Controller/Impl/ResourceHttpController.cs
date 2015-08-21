using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;

namespace Quick.OwinMVC.Controller.Impl
{
    internal class ResourceHttpController : HttpController
    {
        public override void DoGet(IOwinContext context, string plugin, string path)
        {
            context.Response.ContentType = "text/json";
            var data = context.Environment.ToDictionary(t => t.Key, t => t.Value);
            context.Response.Write(JsonConvert.SerializeObject(data));
        }
    }
}
