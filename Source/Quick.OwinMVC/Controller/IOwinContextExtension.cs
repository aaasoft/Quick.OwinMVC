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
        private static readonly String FORMDATA_KEY = $"{typeof(IOwinContextExtension).FullName}.{nameof(FORMDATA_KEY)}";

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
            IFormCollection formCollection = context.Get<IFormCollection>(FORMDATA_KEY);
            if (formCollection != null)
                return formCollection;

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
            formCollection = new FormCollection(dict.ToDictionary(t => t.Key, t => t.Value.ToArray()));
            context.Set(FORMDATA_KEY, formCollection);
            return formCollection;
        }

        private static JObject getJObject(IEnumerable<KeyValuePair<string, string[]>> data, bool valueToObject)
        {
            JObject jObj = new JObject();
            foreach (var pair in data)
            {
                if (pair.Value.Length > 1)
                {
                    jObj.Add(pair.Key, JToken.FromObject(pair.Value));
                }
                else
                {
                    var text = pair.Value[0];
                    //如果要将字符串转换为JSON对象或数组
                    if (valueToObject)
                    {
                        //如果文本是一个JSON对象
                        if (text.StartsWith("{") && text.EndsWith("}"))
                        {
                            try
                            {
                                JObject subObj = JObject.Parse(text);
                                jObj.Add(pair.Key, subObj);
                                continue;
                            }
                            catch { }
                        }
                        //如果文本是一个JSON数组
                        if (text.StartsWith("[") && text.EndsWith("]"))
                        {
                            try
                            {
                                JArray subObj = JArray.Parse(text);
                                jObj.Add(pair.Key, subObj);
                                continue;
                            }
                            catch { }
                        }
                    }
                    jObj.Add(pair.Key, JToken.FromObject(text));
                }
            }
            return jObj;
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
            return context.GetFormData<T>(true);
        }

        /// <summary>
        /// 获取POST提交的表单数据到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="valueToObject">是否将值转换为对象</param>
        /// <returns></returns>
        public static T GetFormData<T>(this IOwinContext context, bool valueToObject)
            where T : class
        {
            return getJObject(context.GetFormData(), valueToObject).ToObject<T>();
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
            return context.GetFormData<T>(obj, true);
        }

        /// <summary>
        /// 获取POST提交的表单数据到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <param name="valueToObject">是否将值转换为对象</param>
        /// <returns></returns>
        public static T GetFormData<T>(this IOwinContext context, T obj, bool valueToObject)
        {
            var jsonString = getJObject(context.GetFormData(), valueToObject).ToString();
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
            return context.GetQueryData<T>(true);
        }

        /// <summary>
        /// 获取URL参数数据到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="valueToObject">是否将值转换为对象</param>
        /// <returns></returns>
        public static T GetQueryData<T>(this IOwinContext context, bool valueToObject)
            where T : class
        {
            return getJObject(context.Request.Query, valueToObject).ToObject<T>();
        }

        /// <summary>
        /// 获取URL参数数据到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetQueryData<T>(this IOwinContext context, T obj)
            where T : class
        {
            return context.GetQueryData<T>(obj, true);
        }

        /// <summary>
        /// 获取URL参数数据到对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="obj"></param>
        /// <param name="valueToObject">是否将值转换为对象</param>
        /// <returns></returns>
        public static T GetQueryData<T>(this IOwinContext context, T obj, bool valueToObject)
            where T : class
        {
            var jsonString = getJObject(context.Request.Query, valueToObject).ToString();
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

        public static void Add(this IDictionary<string, object> dict, Object obj)
        {
            Type type = obj.GetType();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                dict[propertyInfo.Name] = propertyInfo.GetValue(obj, null);
            }
        }
    }
}
