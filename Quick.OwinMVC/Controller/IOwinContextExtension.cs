using Microsoft.Owin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;
using Quick.OwinMVC.Middleware;
using System.Reflection;

namespace Quick.OwinMVC.Controller
{
    public static class IOwinContextExtension
    {
        /// <summary>
        /// 得到Session信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IDictionary<String, Object> GetSession(this IOwinContext context)
        {
            return context.Get<IDictionary<String, Object>>(SessionMiddleware.QUICK_OWINMVC_SESSION_KEY);
        }

        /// <summary>
        /// 获取POST提交的表单数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IFormCollection GetFormData(this IOwinContext context)
        {
            IFormCollection form = context.Request.Get<IFormCollection>("Microsoft.Owin.Form#collection");
            return form;
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this Assembly assembly)
            where T : Attribute
        {
            return GetCustomAttributes<T>(assembly, true);
        }
        public static IEnumerable<T> GetCustomAttributes<T>(this Assembly assembly, Boolean inherit)
            where T : Attribute
        {
            return assembly.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this Type type)
            where T : Attribute
        {
            return GetCustomAttributes<T>(type, true);
        }
        public static IEnumerable<T> GetCustomAttributes<T>(this Type type, Boolean inherit)
            where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }
    }
}
