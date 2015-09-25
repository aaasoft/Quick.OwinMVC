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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

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

        private static String getJsonString(IEnumerable<KeyValuePair<string, string[]>> data)
        {
            JObject jObj = new JObject();
            foreach (var pair in data)
            {
                if (pair.Value.Length > 1)
                    jObj.Add(pair.Key, JToken.FromObject(pair.Value));
                else
                    jObj.Add(pair.Key, JToken.FromObject(pair.Value[0]));
            }
            return jObj.ToString();
        }

        /// <summary>
        /// 获取POST提交的表单数据到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T GetFormData<T>(this IOwinContext context)
            where T : class
        {
            return JsonConvert.DeserializeObject<T>(getJsonString(context.GetFormData()));
        }

        /// <summary>
        /// 获取POST提交的表单数据到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetFormData<T>(this IOwinContext context, T obj)
        {
            var jsonString = getJsonString(context.GetFormData());
            Boolean hasCompilerGeneratedAttribute = typeof(T).GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
            if (hasCompilerGeneratedAttribute)
                return JsonConvert.DeserializeAnonymousType(jsonString, obj);
            JsonConvert.PopulateObject(jsonString, obj);
            return obj;
        }

        /// <summary>
        /// 获取URL参数数据到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T GetQueryData<T>(this IOwinContext context)
            where T : class
        {
            return JsonConvert.DeserializeObject<T>(getJsonString(context.Request.Query));
        }

        /// <summary>
        /// 获取URL参数数据到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetQueryData<T>(this IOwinContext context, T obj)
        {
            var jsonString = getJsonString(context.Request.Query);
            Boolean hasCompilerGeneratedAttribute = typeof(T).GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0;
            if (hasCompilerGeneratedAttribute)
                return JsonConvert.DeserializeAnonymousType(jsonString, obj);
            JsonConvert.PopulateObject(jsonString, obj);
            return obj;
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
