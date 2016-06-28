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
        public abstract string Description { get; }
        public Type InputType { get; } = typeof(TInput);
        public virtual string InvokeExample { get; } = String.Empty;
        public virtual string ReturnValueExample
        {
            get
            {
                if (HttpMethod == AbstractNode.HTTP_METHOD_POST)
                {
                    return $@"code为0代表成功，否则代表失败。
成功时示例：
{JsonConvert.SerializeObject(ApiResult.Success("成功提示文本。"), Formatting.Indented)}

失败时示例：
{JsonConvert.SerializeObject(ApiResult.Error("失败提示文本。"), Formatting.Indented)}
";
                }
                return String.Empty;
            }
        }
        public virtual string[] Tags { get; }

        public object Invoke(IOwinContext context)
        {
            TInput input;
            if (context.Request.Method == "GET")
                input = context.GetQueryData<TInput>();
            else
                input = context.GetFormData<TInput>();
            return Invoke(context, input);
        }

        public abstract object Invoke(IOwinContext context, TInput input);
    }
}
