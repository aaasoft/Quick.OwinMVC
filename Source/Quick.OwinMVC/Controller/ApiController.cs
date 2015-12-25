using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    /// <summary>
    /// API控制器
    /// </summary>
    public abstract class ApiController : IPluginController
    {
        /// <summary>
        /// 执行API控制器，返回输出对象
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task<object> Service(IOwinContext context)
        {
            switch (context.Request.Method)
            {
                case "DELETE":
                    return doDeleteAsync(context);
                case "GET":
                    return doGetAsync(context);
                case "HEAD":
                    return doHeadAsync(context);
                case "OPTIONS":
                    return doOptionsAsync(context);
                case "POST":
                    return doPostAsync(context);
                case "PUT":
                    return doPutAsync(context);
                case "TRACE":
                    return doTraceAsync(context);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 异步处理DELETE请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Task<object> doDeleteAsync(IOwinContext context)
        {
            return Task.FromResult(doDelete(context));
        }

        /// <summary>
        /// 处理DELETE请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual object doDelete(IOwinContext context)
        {
            return null;
        }

        /// <summary>
        /// 异步处理GET请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Task<object> doGetAsync(IOwinContext context)
        {
            return Task.FromResult(doGet(context));
        }

        /// <summary>
        /// 处理GET请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual object doGet(IOwinContext context)
        {
            return null;
        }

        /// <summary>
        /// 异步处理HEAD请求请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Task<object> doHeadAsync(IOwinContext context)
        {
            return Task.FromResult(doHead(context));
        }

        /// <summary>
        /// 处理HEAD请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual object doHead(IOwinContext context)
        {
            return null;
        }

        /// <summary>
        /// 异步处理OPTIONS请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Task<object> doOptionsAsync(IOwinContext context)
        {
            return Task.FromResult(doOptions(context));
        }

        /// <summary>
        /// 处理OPTIONS请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual object doOptions(IOwinContext context)
        {
            return null;
        }

        /// <summary>
        /// 异步处理POST请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Task<object> doPostAsync(IOwinContext context)
        {
            return Task.FromResult(doPost(context));
        }

        /// <summary>
        /// 处理POST请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual object doPost(IOwinContext context)
        {
            return null;
        }

        /// <summary>
        /// 异步处理PUT请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Task<object> doPutAsync(IOwinContext context)
        {
            return Task.FromResult(doPut(context));
        }

        /// <summary>
        /// 处理PUT请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual object doPut(IOwinContext context)
        {
            return null;
        }

        /// <summary>
        /// 异步处理TRACE请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual Task<object> doTraceAsync(IOwinContext context)
        {
            return Task.FromResult(doTrace(context));
        }

        /// <summary>
        /// 处理TRACE请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual object doTrace(IOwinContext context)
        {
            return null;
        }
    }
}
