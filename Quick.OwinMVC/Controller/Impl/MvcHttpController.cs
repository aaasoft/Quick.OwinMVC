using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.Dynamic;
using Quick.OwinMVC.View;

namespace Quick.OwinMVC.Controller.Impl
{
    internal class MvcHttpController : ExtendHttpController<IMvcController>
    {
        private IViewRender viewRender;

        public MvcHttpController(IViewRender viewRender)
        {
            this.viewRender = viewRender;
        }

        public override void ExecuteController(IMvcController controller, IOwinContext context)
        {
            IDictionary<String, Object> data = new ExpandoObject();
            String viewName = controller.Service(context, data);
            var content = viewRender.Render(viewName, data);
            var bytes = encoding.GetBytes(content);

            var rep = context.Response;
            rep.ContentType = "text/html";
            rep.ContentLength = content.Length;
            rep.Write(content);
        }
    }
}
