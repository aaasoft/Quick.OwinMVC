using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.Dynamic;
using Quick.OwinMVC.View;
using Quick.OwinMVC.Routing;

namespace Quick.OwinMVC.Controller.Impl
{
    [Route("/:" + Middleware.QOMVC_PLUGIN_KEY + "/view/:" + Middleware.QOMVC_PATH_KEY)]
    internal class MvcHttpController : ExtendHttpController<IMvcController>
    {
        public IViewRender ViewRender { get; set; }

        public override void ExecuteController(IMvcController controller, IOwinContext context, string plugin, string path)
        {
            IDictionary<String, Object> data = new ExpandoObject();
            String viewName = controller.Service(context, data);
            viewName = $"{plugin}:{viewName}";
            var content = ViewRender.Render(viewName, data);
            var bytes = encoding.GetBytes(content);

            var rep = context.Response;
            rep.ContentType = "text/html";
            rep.ContentLength = bytes.Length;
            rep.Write(content);
        }
    }
}
