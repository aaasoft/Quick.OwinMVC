using Microsoft.Owin;
using Quick.OwinMVC.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    /// <summary>
    /// MVC控制器
    /// </summary>
    public abstract class ViewController : IPluginController
    {
        /// <summary>
        /// 执行MVC控制器，返回视图名称
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual String Service(IOwinContext context, IDictionary<String, Object> data)
        {
            switch (context.Request.Method)
            {
                case "DELETE":
                    return doDelete(context, data);
                case "GET":
                    return doGet(context, data);
                case "HEAD":
                    return doHead(context, data);
                case "OPTIONS":
                    return doOptions(context, data);
                case "POST":
                    return doPost(context, data);
                case "PUT":
                    return doPut(context, data);
                case "TRACE":
                    return doTrace(context, data);
                default:
                    return GetViewNameByAttribute();
            }
        }

        /// <summary>
        /// 根据RouteAttribute返回视图名称
        /// </summary>
        /// <returns></returns>
        protected String GetViewNameByAttribute()
        {
            RouteAttribute attr = this.GetType().GetCustomAttributes(typeof(RouteAttribute), false)
                .FirstOrDefault() as RouteAttribute;
            if (attr == null)
                throw new ApplicationException("Cann't find RouteAttribute for this ViewController class.");
            return attr.Path;
        }

        /// <summary>
        /// 处理DELETE请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual String doDelete(IOwinContext context, IDictionary<String, Object> data)
        {
            return GetViewNameByAttribute();
        }

        /// <summary>
        /// 处理GET请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual String doGet(IOwinContext context, IDictionary<String, Object> data)
        {
            return GetViewNameByAttribute();
        }

        /// <summary>
        /// 处理HEAD请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual String doHead(IOwinContext context, IDictionary<String, Object> data)
        {
            return GetViewNameByAttribute();
        }

        /// <summary>
        /// 处理OPTIONS请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual String doOptions(IOwinContext context, IDictionary<String, Object> data)
        {
            return GetViewNameByAttribute();
        }

        /// <summary>
        /// 处理POST请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual String doPost(IOwinContext context, IDictionary<String, Object> data)
        {
            return GetViewNameByAttribute();
        }

        /// <summary>
        /// 处理PUT请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual String doPut(IOwinContext context, IDictionary<String, Object> data)
        {
            return GetViewNameByAttribute();
        }

        /// <summary>
        /// 处理TRACE请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual String doTrace(IOwinContext context, IDictionary<String, Object> data)
        {
            return GetViewNameByAttribute();
        }
    }
}
