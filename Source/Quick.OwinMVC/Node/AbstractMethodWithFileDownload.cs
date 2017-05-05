using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.IO;
using Quick.OwinMVC.Utils;

namespace Quick.OwinMVC.Node
{
    public class FileDownloadResult : StreamOutputResult
    {
        /// <summary>
        /// 文件名称(附件文件名)
        /// </summary>
        public string FileName { get; set; }
    }

    public abstract class AbstractMethodWithFileDownload<TInput> : AbstractMethod<TInput>
                where TInput : class
    {
        public abstract FileDownloadResult HandleDownload(IOwinContext context, TInput input);

        public sealed override object Invoke(IOwinContext context, TInput input)
        {
            var result = HandleDownload(context, input);
            var rep = context.Response;
            //输出附件文件名
            if (!string.IsNullOrEmpty(result.FileName))
            {
                var encodedFileName = System.Net.WebUtility.UrlEncode(result.FileName);
                rep.Headers["Content-Disposition"] = $"attachment; filename=\"{encodedFileName}\"";
            }
            //内容大小
            rep.ContentLength = result.ContentLength;
            //内容类型
            if (string.IsNullOrEmpty(result.ContentType)
                && !string.IsNullOrEmpty(result.FileName))
                rep.ContentType = MimeUtils.GetMime(result.FileName);
            else
                rep.ContentType = result.ContentType;
            //操作流
            result.ResponseStreamHandler.Invoke(rep.Body);
            throw NodeMethodHandledException.Instance;
        }
    }
}
