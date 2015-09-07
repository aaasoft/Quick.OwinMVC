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

        public virtual void DoGet(IOwinContext context)
        { }
        public virtual void DoPost(IOwinContext context)
        { }
        public virtual void DoHead(IOwinContext context)
        { }
        public virtual void DoDelete(IOwinContext context)
        { }
        public virtual void DoTrace(IOwinContext context)
        { }

        public virtual void Service(IOwinContext context)
        {
            switch (context.Request.Method)
            {
                case "GET":
                    DoGet(context);
                    break;
                case "POST":
                    DoPost(context);
                    break;
                case "DELETE":
                    DoDelete(context);
                    break;
                case "HEAD":
                    DoHead(context);
                    break;
                case "TRACE":
                    DoTrace(context);
                    break;
            }
        }
    }
}
