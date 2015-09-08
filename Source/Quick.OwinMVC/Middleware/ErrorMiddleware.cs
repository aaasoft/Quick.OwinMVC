using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class ErrorMiddleware : OwinMiddleware
    {
        private Encoding encoding = new UTF8Encoding(false);

        public ErrorMiddleware(OwinMiddleware next) : base(next)
        {
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
                    var message = ex.ToString();

                    var rep = context.Response;
                    //Nowin服务器会自动处理500，所以此处暂时用510代替。
                    rep.StatusCode = 510;
                    rep.ContentType = "text/plain; charset=UTF-8";
                    byte[] content = encoding.GetBytes(message);
                    rep.ContentLength = content.Length;
                    context.Response.Write(content);
                }
            });
        }
    }
}
