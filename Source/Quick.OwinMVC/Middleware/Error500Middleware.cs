using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class Error500Middleware : OwinMiddleware, IPropertyHunter
    {
        private Server server;
        private String RewritePath;
        private Encoding encoding = new UTF8Encoding(false);

        public Error500Middleware(OwinMiddleware next) : base(next)
        {
            server = Server.Instance;
        }

        public override Task Invoke(IOwinContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    Next.Invoke(context).Wait();
                }
                catch (Exception ex)
                {
                    var rep = context.Response;
                    rep.StatusCode = 500;
                    try
                    {
                        if (String.IsNullOrEmpty(RewritePath))
                            throw new ArgumentNullException($"Property '{this.GetType().FullName}.{RewritePath}' must be set.");

                        context.Set("Exception", ex);
                        context.Set("owin.RequestPath", RewritePath);

                        //清理OwinContext
                        foreach (var cleaner in server.GetMiddlewares<IOwinContextCleaner>())
                            cleaner.Clean(context);

                        Next.Invoke(context).Wait();
                        if (rep.StatusCode == 404)
                            throw ex;
                    }
                    catch (Exception ex2)
                    {
                        rep.ContentType = "text/plain; charset=UTF-8";
                        byte[] content = encoding.GetBytes(ex2.ToString());
                        rep.ContentLength = content.Length;
                        context.Response.Write(content);
                    }
                }
            });
        }

        void IPropertyHunter.Hunt(string key, string value)
        {
            if (key == nameof(RewritePath))
                RewritePath = value;
        }
    }
}
