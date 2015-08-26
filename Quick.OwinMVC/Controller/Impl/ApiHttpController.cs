﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using Quick.OwinMVC.Routing;

namespace Quick.OwinMVC.Controller.Impl
{
    [Route("/:" + Middleware.QOMVC_PLUGIN_KEY + "/api/:" + Middleware.QOMVC_PATH_KEY)]
    internal class ApiHttpController : ExtendHttpController<IApiController>
    {
        public override void ExecuteController(IApiController controller, IOwinContext context, string plugin, string path)
        {
            var rep = context.Response;
            Object obj = controller.Service(context);
            if (obj == null)
            {
                rep.StatusCode = 204;
            }
            else
            {
                try
                {
                    var content = encoding.GetBytes(JsonConvert.SerializeObject(obj));
                    rep.ContentType = "text/json";
                    rep.ContentLength = content.Length;
                    rep.Write(content);
                }
                catch (Exception ex)
                {
                    rep.StatusCode = 500;
#if DEBUG
                    rep.Write(ex.ToString());
#else
                    rep.Write(ex.Message);
#endif
                }
            }
        }
    }
}