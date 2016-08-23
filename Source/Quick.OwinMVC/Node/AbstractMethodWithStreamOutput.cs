using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node
{
    public class StreamOutputResult
    {
        /// <summary>
        /// 文件大小
        /// </summary>
        public long? ContentLength { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 响应流处理器
        /// </summary>
        public Action<Stream> ResponseStreamHandler { get; set; }
    }

    public abstract class AbstractMethodWithStreamOutput<TInput> : AbstractMethod<TInput>
                where TInput : class
    {
        public abstract StreamOutputResult HandleOutput(IOwinContext context, TInput input);

        public sealed override object Invoke(IOwinContext context, TInput input)
        {
            var result = HandleOutput(context, input);
            var rep = context.Response;
            //内容大小
            rep.ContentLength = result.ContentLength;
            //内容类型
            if (!string.IsNullOrEmpty(result.ContentType))
                rep.ContentType = result.ContentType;
            result.ResponseStreamHandler.Invoke(rep.Body);
            throw NodeMethodHandledException.Instance;
        }
    }
}
