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
        public const string JSONP_CALLBACK = "callback";

        public ApiMiddleware(OwinMiddleware next) : base(next) { }

        public override async Task ExecuteController(ApiController controller, IOwinContext context, string plugin, string path)
        {
            var rep = context.Response;
            var obj = await controller.Service(context);
            if (obj == null)
                return;

            var req = context.Request;
            //要输出的内容
            string result = null;
            //JSON序列化的结果
            var json = JsonConvert.SerializeObject(obj);
            var jsonpCallback = req.Query[JSONP_CALLBACK];

            if (string.IsNullOrEmpty(jsonpCallback))
            {
                rep.ContentType = "application/json; charset=UTF-8";
                result = json;
            }
            else
            {
                rep.ContentType = "application/x-javascript";
                result = $"{jsonpCallback}({json})";
            }
            rep.Expires = new DateTimeOffset(DateTime.Now);
            await context.Output(encoding.GetBytes(result), EnableCompress);
        }
    }
}
