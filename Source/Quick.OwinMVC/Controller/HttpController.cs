using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    public abstract class HttpController
    {
        /// <summary>
        /// 提供服务
        /// </summary>
        /// <param name="context"></param>
        public void Service(IOwinContext context)
        {
            switch (context.Request.Method)
            {
                case "DELETE":
                    doDelete(context);
                    break;
                case "GET":
                    doGet(context);
                    break;
                case "HEAD":
                    doHead(context);
                    break;
                case "OPTIONS":
                    doOptions(context);
                    break;
                case "POST":
                    doPost(context);
                    break;
                case "PUT":
                    doPut(context);
                    break;
                case "TRACE":
                    doTrace(context);
                    break;
            }
        }

        /// <summary>
        /// 处理DELETE请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual void doDelete(IOwinContext context) { }

        /// <summary>
        /// 处理GET请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual void doGet(IOwinContext context) { }

        /// <summary>
        /// 处理HEAD请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual void doHead(IOwinContext context) { }

        /// <summary>
        /// 处理OPTIONS请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual void doOptions(IOwinContext context) { }

        /// <summary>
        /// 处理POST请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual void doPost(IOwinContext context) { }

        /// <summary>
        /// 处理PUT请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual void doPut(IOwinContext context) { }

        /// <summary>
        /// 处理TRACE请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual void doTrace(IOwinContext context) { }
    }
}
