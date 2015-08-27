using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.Dynamic;
using Quick.OwinMVC.View;
using Quick.OwinMVC.Routing;
using Quick.OwinMVC.Middleware;

namespace Quick.OwinMVC.Controller.Impl
{
    [Route("/:" + MvcMiddleware.QOMVC_PLUGIN_KEY + "/view/:" + MvcMiddleware.QOMVC_PATH_KEY)]
    internal class MvcHttpController : ExtendHttpController<IMvcController>
    {
        public IViewRender ViewRender { get; set; }

        public override void ExecuteController(IMvcController controller, IOwinContext context, string plugin, string path)
        {
            String viewName = controller.Service(context, context.Environment);
            viewName = $"{plugin}:{viewName}";
            var content = ViewRender.Render(viewName, context.Environment);
            var bytes = encoding.GetBytes(content);

            var rep = context.Response;
            rep.ContentType = "text/html";
            rep.ContentLength = bytes.Length;
            rep.Write(content);
        }
    }
}
