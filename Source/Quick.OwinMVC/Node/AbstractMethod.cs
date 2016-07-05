using Microsoft.Owin;
using Newtonsoft.Json;
using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node
{
    public abstract class AbstractMethod<TInput> : IMethod
                where TInput : class
    {
        public String Path { get; set; }
        public String HttpMethod { get; set; }
        public abstract string Name { get; }
        public virtual string Description { get; } = String.Empty;
        public Type InputType { get; } = typeof(TInput);
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

        public object Invoke(IOwinContext context)
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
            if (context.Request.Method == "GET")
                input = context.GetQueryData<TInput>(valueToObject, ignoreProperties);
            else
                input = context.GetFormData<TInput>(valueToObject, ignoreProperties);
            return Invoke(context, input);
        }

        public abstract object Invoke(IOwinContext context, TInput input);
    }
}
