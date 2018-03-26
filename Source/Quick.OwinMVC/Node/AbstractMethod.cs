using HttpMultipartParser;
using Microsoft.Owin;
using Newtonsoft.Json;
using Quick.OwinMVC.Localization;
using Quick.OwinMVC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node
{
    public abstract class AbstractMethod : IMethod
    {
        public String Path { get; set; }
        public String HttpMethod { get; set; }
        public abstract string Name { get; }
        public virtual Type InputType { get; } = null;
        public virtual string Description { get; } = String.Empty;
        public virtual string InvokeExample { get; } = String.Empty;
        public virtual string ReturnValueExample
        {
            get
            {
                if (HttpMethod == AbstractNode.HTTP_METHOD_POST)
                {
                    return $@"成功时示例：
{JsonConvert.SerializeObject(ApiResult.Success($"{Name}成功"), Formatting.Indented)}

失败时示例：
{JsonConvert.SerializeObject(ApiResult.Error($"{Name}失败"), Formatting.Indented)}
";
                }
                return String.Empty;
            }
        }
        public virtual string[] Tags { get; }

        /// <summary>
        /// 处理文件上传
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <param name="contentDisposition"></param>
        /// <param name="buffer"></param>
        /// <param name="bytes"></param>
        public virtual void HandleFileUpload(IOwinContext context, string name, string fileName, string contentType, string contentDisposition, byte[] buffer, int bytes) { }
        /// <summary>
        /// 完成文件上传
        /// </summary>
        public virtual void FinishFileUpload(IOwinContext context) { }

        public abstract object Invoke(IOwinContext context);

        protected string GetLanguage(IOwinContext context)
        {
            return context.GetLanguage();
        }

        protected TextManager GetTextManager(IOwinContext context)
        {
            return context.GetTextManager();
        }
    }

    public abstract class AbstractMethod<TInput> : AbstractMethod
                where TInput : class
    {
        public override Type InputType { get; } = typeof(TInput);

        [TextResource]
        private enum Texts
        {
            [Text("传入参数错误，请检查参数是否正确。{0}")]
            ARGUMENT_ERROR = 410
        }
        /// <summary>
        /// 处理参数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual TInput HandleParameter(IOwinContext context, TInput input) => input;

        public override object Invoke(IOwinContext context)
        {
            NodeParameterAttribute attribute = typeof(TInput)
                .GetCustomAttributes(typeof(NodeParameterAttribute), false)
                .FirstOrDefault() as NodeParameterAttribute;

            bool valueToObject = false;
            String[] ignoreProperties = null;

            if (attribute != null)
            {
                valueToObject = attribute.ValueToObject;
                ignoreProperties = attribute.IgnoreProperties;
            }

            TInput input;
            try
            {
                if (typeof(TInput) == typeof(object))
                {
                    input = default(TInput);
                }
                else
                {
                    if (context.Request.Method == "GET")
                        input = context.GetQueryData<TInput>(valueToObject, ignoreProperties);
                    else if (context.Request.ContentType != null
                        && context.Request.ContentType.StartsWith("multipart/form-data;"))
                    {
                        IDictionary<string, string[]> paramDict = new Dictionary<string, string[]>();
                        MultipartFormDataUtils.HandleMultipartData(context.Request, part =>
                         {
                             if (paramDict.ContainsKey(part.Name))
                                 paramDict[part.Name] = paramDict[part.Name].Union(new[] { part.Data }).ToArray();
                             else
                                 paramDict[part.Name] = new[] { part.Data };
                         }, (string name, string fileName, string contentType, string contentDisposition, byte[] buffer, int bytes) =>
                         {
                             HandleFileUpload(context, name, fileName, contentType, contentDisposition, buffer, bytes);
                         }, () => FinishFileUpload(context));

                        input = context.GetDictData<TInput>(paramDict, valueToObject, ignoreProperties);
                    }
                    else
                        input = context.GetFormData<TInput>(valueToObject, ignoreProperties);
                }
            }
            catch (Exception ex)
            {
                throw new NodeMethodException(context, Texts.ARGUMENT_ERROR, ex.Message);
            }
            //自身参数处理器
            input = HandleParameter(context, input);
            //全局参数处理器
            if (NodeManager.Instance.ParameterHandler != null)
                input = (TInput)NodeManager.Instance.ParameterHandler.Invoke(this, context, input);
            //调用
            return Invoke(context, input);
        }

        public abstract object Invoke(IOwinContext context, TInput input);
    }
}
