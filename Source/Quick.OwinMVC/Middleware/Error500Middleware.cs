using Microsoft.Owin;
using Newtonsoft.Json;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Hunter;
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

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (Exception ex)
            {
                var rep = context.Response;
                rep.StatusCode = 500;
                try
                {
                    if (String.IsNullOrEmpty(RewritePath))
                    {
                        var result = ApiResult.Error(ex.HResult, ex.Message, new
                        {
                            Type = ex.GetType().FullName,
                            ex.Source,
                            ex.Data
#if DEBUG
                            ,
                            ex.StackTrace
#endif
                        }).ToString();

                        rep.ContentType = "text/json; charset=UTF-8";
                        byte[] content = encoding.GetBytes(result);
                        rep.ContentLength = content.Length;
                        await context.Response.WriteAsync(content);
                        return;
                    }

                    context.Set("Exception", ex);
                    context.Set("owin.RequestPath", RewritePath);

                    //清理OwinContext
                    foreach (var cleaner in server.GetMiddlewares<IOwinContextCleaner>())
                        cleaner.Clean(context);

                    await Next.Invoke(context);
                    if (rep.StatusCode == 404)
                        throw ex;
                }
                catch (Exception ex2)
                {
                    rep.ContentType = "text/plain; charset=UTF-8";
                    byte[] content = encoding.GetBytes(ex2.ToString());
                    rep.ContentLength = content.Length;
                    await context.Response.WriteAsync(content);
                }
            }
        }

        void IPropertyHunter.Hunt(string key, string value)
        {
            if (key == nameof(RewritePath))
                RewritePath = value;
        }
    }
}
