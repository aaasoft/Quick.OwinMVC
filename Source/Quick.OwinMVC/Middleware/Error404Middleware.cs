using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class Error404Middleware : OwinMiddleware, IPropertyHunter
    {
        private Server server;
        private String RewritePath;

        public Error404Middleware(OwinMiddleware next) : base(next)
        {
            this.server = Server.Instance;
        }

        void IPropertyHunter.Hunt(string key, string value)
        {
            if (key == nameof(RewritePath))
                RewritePath = value;
        }

        public override Task Invoke(IOwinContext context)
        {
            if (String.IsNullOrEmpty(RewritePath))
                throw new ArgumentNullException($"Property '{this.GetType().FullName}.{RewritePath}' must be set.");
            
            context.Set<String>("owin.RequestPath", RewritePath);
            OwinMiddleware first = server.GetFirstMiddlewareInstance();
            if (first == null)
                throw new ArgumentException($"Middleware '{this.GetType().FullName};{this.GetType().Assembly.GetName().Name}' must not be the first middleware,and recommand to be set to the last one.");

            //清理OwinContext
            foreach (var cleaner in server.GetMiddlewares<IOwinContextCleaner>())
                cleaner.Clean(context);

            return first.Invoke(context);
        }
    }
}
