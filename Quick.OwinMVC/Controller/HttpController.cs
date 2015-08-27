using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Quick.OwinMVC.Controller
{
    public abstract class HttpController : IHttpController
    {
        protected IDictionary<string, string> properties;
        public virtual void Init(IDictionary<string, string> properties)
        {
            this.properties = properties;
        }

        public virtual void DoGet(IOwinContext context, String plugin, String path)
        { }
        public virtual void DoPost(IOwinContext context, String plugin, String path)
        { }
        public virtual void DoHead(IOwinContext context, String plugin, String path)
        { }
        public virtual void DoDelete(IOwinContext context, String plugin, String path)
        { }
        public virtual void DoTrace(IOwinContext context, String plugin, String path)
        { }

        public virtual void Service(IOwinContext context, String plugin, String path)
        {
            switch (context.Request.Method)
            {
                case "GET":
                    DoGet(context, plugin, path);
                    break;
                case "POST":
                    DoPost(context, plugin, path);
                    break;
                case "DELETE":
                    DoDelete(context, plugin, path);
                    break;
                case "HEAD":
                    DoHead(context, plugin, path);
                    break;
                case "TRACE":
                    DoTrace(context, plugin, path);
                    break;
            }
        }
    }
}
