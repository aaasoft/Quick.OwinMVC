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
using System.IO;
using System.Web;

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
            StreamReader reader = new StreamReader(context.Request.Body);
            var formData = reader.ReadToEnd();
            IDictionary<String, IList<String>> dict = new Dictionary<String, IList<String>>();
            foreach (var line in formData.Split('&'))
            {
                var strs = line.Split('=');
                if (line.Length < 2)
                    continue;
                var key = strs[0].Trim();
                var value = strs[1].Trim();
                value = HttpUtility.UrlDecode(value);
                if (!dict.ContainsKey(key))
                    dict.Add(key, new List<String>());
                dict[key].Add(value);
            }
            return new FormCollection(dict.ToDictionary(t => t.Key, t => t.Value.ToArray()));
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
