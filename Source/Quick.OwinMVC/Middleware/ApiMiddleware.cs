using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.Reflection;
using Quick.OwinMVC.Routing;
using Quick.OwinMVC.Controller;
using Newtonsoft.Json;

namespace Quick.OwinMVC.Middleware
{
    public class ApiMiddleware : AbstractControllerMiddleware<ApiController>
    {
        public ApiMiddleware(OwinMiddleware next) : base(next) { }

        public override async Task ExecuteController(ApiController controller, IOwinContext context, string plugin, string path)
        {
            var rep = context.Response;
            var obj = await controller.Service(context);
            if (obj == null)
                return;
            var content = encoding.GetBytes(JsonConvert.SerializeObject(obj));
            rep.Expires = new DateTimeOffset(DateTime.Now);
            rep.ContentType = "text/json; charset=UTF-8";
            await context.Output(content, EnableCompress);
        }
    }
}
