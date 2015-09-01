using System;
using Microsoft.Owin;
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
            rep.ContentType = "text/html; charset=UTF-8";
            rep.ContentLength = bytes.Length;
            rep.Write(content);
        }
    }
}
