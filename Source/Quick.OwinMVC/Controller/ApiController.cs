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
                    return doDelete(context);
                case "GET":
                    return doGet(context);
                case "HEAD":
                    return doHead(context);
                case "OPTIONS":
                    return doOptions(context);
                case "POST":
                    return doPost(context);
                case "PUT":
                    return doPut(context);
                case "TRACE":
                    return doTrace(context);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 处理DELETE请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual async Task<object> doDelete(IOwinContext context)
        {
            return await Task.FromResult<object>(null);
        }

        /// <summary>
        /// 处理GET请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual async Task<object> doGet(IOwinContext context)
        {
            return await Task.FromResult<object>(null);
        }

        /// <summary>
        /// 处理HEAD请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual async Task<object> doHead(IOwinContext context)
        {
            return await Task.FromResult<object>(null);
        }

        /// <summary>
        /// 处理OPTIONS请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual async Task<object> doOptions(IOwinContext context)
        {
            return await Task.FromResult<object>(null);
        }

        /// <summary>
        /// 处理POST请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual async Task<object> doPost(IOwinContext context)
        {
            return await Task.FromResult<object>(null);
        }

        /// <summary>
        /// 处理PUT请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual async Task<object> doPut(IOwinContext context)
        {
            return await Task.FromResult<object>(null);
        }

        /// <summary>
        /// 处理TRACE请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual async Task<object> doTrace(IOwinContext context)
        {
            return await Task.FromResult<object>(null);
        }
    }
}
