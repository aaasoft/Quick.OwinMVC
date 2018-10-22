using Microsoft.Owin;
using Quick.OwinMVC.Hunter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class MiddlewareContext : OwinMiddleware
    {
        public static MiddlewareContext Instance { get; private set; }
        /// <summary>
        /// 所有中间件对象列表
        /// </summary>
        public IEnumerable<OwinMiddleware> Middlewares { get; private set; }
        /// <summary>
        /// HTTP服务是否已准备好
        /// </summary>
        public bool IsReady { get; set; } = false;

        /// <summary>
        /// 获取指定类型的中间件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetMiddleware<T>()
        {
            return GetMiddlewares<T>().FirstOrDefault();
        }

        /// <summary>
        /// 获取指定类型的全部中间件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetMiddlewares<T>()
        {
            return Middlewares.Where(m => m is T).Cast<T>();
        }

        public MiddlewareContext(OwinMiddleware next) : base(next)
        {
            Instance = this;
            var list = new List<OwinMiddleware>();

            var nextProperty = typeof(OwinMiddleware).GetProperty("Next", BindingFlags.Instance | BindingFlags.NonPublic);
            var currentMiddleware = next;
            while (currentMiddleware != null)
            {
                list.Add(currentMiddleware);
                currentMiddleware = nextProperty.GetValue(currentMiddleware, null) as OwinMiddleware;
            }
            Middlewares = list;
        }

        public override Task Invoke(IOwinContext context)
        {
            //如果WEB服务器还没有准备好
            if (!IsReady)
            {
                var rep = context.Response;
                rep.StatusCode = 503;
                rep.ReasonPhrase = "Service Unavailable";
                //5秒后重试
                rep.Headers["Retry-After"] = "5";
                return rep.WriteAsync("Oops,web server is not ready yet,please refresh later...");
            }

            String path = context.Get<String>("owin.RequestPath");
            //设置原始请求路径
            context.Set("Quick.OwinMVC.SourceRequestPath", path);
            context.Set("Quick.OwinMVC.SourceRequestUri", context.Request.Uri);

            var contextPath = string.Empty;
            if (Server.Instance.IsRootContextPath)
                contextPath = "";
            else
                contextPath = $"/{Server.Instance.ContextPath}";

            context.Set("ContextPath", contextPath);
            return Next.Invoke(context);
        }
    }
}
