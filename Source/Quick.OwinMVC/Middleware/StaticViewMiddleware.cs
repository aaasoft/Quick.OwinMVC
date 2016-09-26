using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Quick.OwinMVC.Hunter;

namespace Quick.OwinMVC.Middleware
{
    public class StaticViewMiddleware : AbstractPluginPathMiddleware, IPropertyHunter
    {
        //过期时间(默认1秒)
        public double Expires { get; private set; } = 1;
        //视图文件后缀
        public string ViewFileSuffix { get; private set; }

        public StaticViewMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override void Hunt(string key, string value)
        {
            base.Hunt(key, value);
            switch (key)
            {
                case nameof(ViewFileSuffix):
                    ViewFileSuffix = value;
                    break;
            }
        }

        public override Task Invoke(IOwinContext context, string plugin, string path)
        {
            var resourceMiddleware = Server.Instance.GetMiddleware<ResourceMiddleware>();
            return resourceMiddleware.InvokeFinal(context, Route, ViewFileSuffix, plugin, path, t => Next.Invoke(t), Expires);
        }
    }
}
