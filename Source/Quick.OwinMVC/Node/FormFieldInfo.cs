using Microsoft.Owin;
using Newtonsoft.Json;
using Quick.OwinMVC.Localization;
using Quick.OwinMVC.Node.ValueFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Quick.OwinMVC.Node
{
    /// <summary>
    /// 表单字段类
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FormFieldInfo : Attribute
    {
        [JsonIgnore]
        public override object TypeId
        {
            get { return base.TypeId; }
        }

        [JsonIgnore]
        public Type FieldType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Type
        {
            get
            {
                if (FieldType == null)
                    return null;
                //如果是泛型
                if (FieldType.IsGenericType)
                {
                    var genericType = FieldType.GetGenericTypeDefinition();
                    //如果是Nullable<>
                    if (genericType == typeof(Nullable<>))
                        return FieldType.GenericTypeArguments.First().Name;
                    if (typeof(IEnumerable<>).IsAssignableFrom(genericType))
                        return FieldType.GenericTypeArguments.First().Name+"[]";
                }
                return FieldType.Name;
            }
        }

        public String Key { get; set; }
        public String Name { get; set; }

        [JsonIgnore]
        public Object NameEnum { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Description { get; set; }

        [JsonIgnore]
        public Object DescriptionEnum { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Boolean NotEmpty { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Object Value { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Boolean IsHidden { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Boolean IsReadOnly { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IValueFormat ValueFormatValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String ValueFormatType
        {
            get
            {
                if (ValueFormatValue == null)
                    return null;
                return ValueFormatValue.GetType().Name;
            }
        }

        /// <summary>
        /// 获取某个类的全部表单信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<FormFieldInfo> GetAll(TextManager textManager, Type type, object obj = null)
        {
            List<FormFieldInfo> list = new List<FormFieldInfo>();
            if (type != null)
                foreach (var pi in type.GetProperties()
                    .OrderBy(t => t.DeclaringType == type)
                    .ToArray())
                {
                    var fieldInfo = Get(pi, null);
                    if (fieldInfo != null)
                    {
                        fieldInfo.FieldType = pi.PropertyType;
                        if (obj != null)
                        {
                            fieldInfo.Value = pi.GetValue(obj, null);
                        }
                        list.Add(fieldInfo);
                    }
                }
            return LanguageProcess(textManager, list);
        }

        /// <summary>
        /// 获取某个类的全部表单信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<FormFieldInfo> GetAll<T>(TextManager textManager, object obj = null)
        {
            return GetAll(textManager, typeof(T), obj);
        }

        /// <summary>
        /// 获取某个类的某个属性的表单信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static FormFieldInfo Get<T>(String propertyName)
        {
            return Get<T>(propertyName, null);
        }

        public static FormFieldInfo Get<T>(String propertyName, Object value)
        {
            return Get<T>(propertyName, t => { t.Value = value; });
        }

        public static string GetName<T>(String propertyName, IOwinContext context)
        {
            return GetName<T>(propertyName, context.GetTextManager());
        }

        public static string GetName<T>(String propertyName, TextManager textManager)
        {
            var formFieldInfo = Get<T>(propertyName, null);
            formFieldInfo.Handle(textManager);
            return formFieldInfo.Name;
        }

        /// <summary>
        /// 获取某个类的某个属性的表单信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static FormFieldInfo Get<T>(String propertyName, Action<FormFieldInfo> handler)
        {
            return Get(typeof(T), propertyName, handler);
        }

        /// <summary>
        /// 获取某个类的某个属性的表单信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static FormFieldInfo Get(Type type, String propertyName, Action<FormFieldInfo> handler = null)
        {
            var pi = type.GetProperty(propertyName);
            if (pi == null)
                return null;
            return Get(pi, handler);
        }

        /// <summary>
        /// 获取某个类的某个属性的表单信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static FormFieldInfo Get(PropertyInfo pi, Action<FormFieldInfo> handler)
        {
            var fieldInfo = pi.GetCustomAttributes(typeof(FormFieldInfo), false).FirstOrDefault() as FormFieldInfo;
            if (fieldInfo == null)
                return null;
            if (String.IsNullOrEmpty(fieldInfo.Key))
                fieldInfo.Key = pi.Name;
            var valueFormat = pi.GetCustomAttributes(typeof(IValueFormat), false).FirstOrDefault() as IValueFormat;
            fieldInfo.ValueFormatValue = valueFormat;
            if (handler != null)
                handler(fieldInfo);
            return fieldInfo;
        }

        /// <summary>
        /// 多语言处理
        /// </summary>
        /// <param name="textManager"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static object LanguageProcess(TextManager textManager, IEnumerable<FormFieldInfo> fields, Boolean isGroup = false)
        {
            var list = new List<object>();
            fields.ToList().ForEach(t =>
            {
                t.Handle(textManager);
                list.Add(t);
            });
            return list;
        }

        public static IEnumerable<FormFieldInfo> LanguageProcess(IOwinContext context, IEnumerable<FormFieldInfo> fields)
        {
            return LanguageProcess(context.GetTextManager(), fields);
        }

        /// <summary>
        /// 多语言处理
        /// </summary>
        /// <param name="textManager"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IEnumerable<FormFieldInfo> LanguageProcess(TextManager textManager, IEnumerable<FormFieldInfo> fields)
        {
            foreach (var field in fields)
                field.Handle(textManager);
            return fields;
        }

        public void Handle(TextManager textManager)
        {
            Enum nameEnum = this.NameEnum as Enum;
            if (nameEnum != null)
                this.Name = textManager.GetText(nameEnum);
            Enum descriptionEnum = this.DescriptionEnum as Enum;
            if (descriptionEnum != null)
                this.Description = textManager.GetText(descriptionEnum);
            if (this.ValueFormatValue != null)
                this.ValueFormatValue.Handle(textManager, this);
        }

        public object GetValue(TextManager textManager, object obj)
        {
            if (ValueFormatType == null || obj == null) return obj;
            return ValueFormatValue.GetValue(textManager, obj);
        }
    }
}
