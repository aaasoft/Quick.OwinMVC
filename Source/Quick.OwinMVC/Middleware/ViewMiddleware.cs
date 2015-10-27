using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.View;
using Quick.OwinMVC.Utils;
using System.IO;
using Quick.OwinMVC.Hunter;

namespace Quick.OwinMVC.Middleware
{
    public class ViewMiddleware : AbstractControllerMiddleware<IViewController>,IHungryPropertyHunter, IPropertyHunter
    {
        private IDictionary<String, String> properties;
        private IViewRender ViewRender;

        public ViewMiddleware(OwinMiddleware next) : base(next) { }

        public override string GetRouteMiddle()
        {
            return "view";
        }

        public void Hunt(IDictionary<string, string> properties)
        {
            this.properties = properties;
        }

        public override void Hunt(String key, String value)
        {
            base.Hunt(key, value);
            switch (key)
            {
                case nameof(ViewRender):
                    //创建视图渲染器
                    this.ViewRender = (IViewRender)AssemblyUtils.CreateObject(value);
                    this.ViewRender.Init(properties);
                    break;
            }
        }

        public override void ExecuteController(IViewController controller, IOwinContext context, string plugin, string path)
        {
            //执行MVC控制器，返回视图名称
            String viewName = controller.Service(context, context.Environment);
            if (viewName == null)
                return;
            var language = context.GetLanguage();
            switch (viewName.Split(':').Count())
            {
                case 1:
                    viewName = $"{plugin}:{viewName}:{language}";
                    break;
                case 2:
                    viewName = $"{viewName}:{language}";
                    break;
            }            
            //根据视图名称与数据，渲染输出页面
            var outputText = ViewRender.Render(viewName, context.Environment);
            var content = encoding.GetBytes(outputText);

            //将页面写到响应中
            var rep = context.Response;
            rep.ContentType = "text/html; charset=UTF-8";
            Output(context, content);
        }
    }
}
