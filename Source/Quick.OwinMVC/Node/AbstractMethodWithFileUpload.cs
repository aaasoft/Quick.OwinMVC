using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.IO;

namespace Quick.OwinMVC.Node
{
    public abstract class AbstractMethodWithFileUpload<TInput> : AbstractMethod<TInput>
                where TInput : class
    {
        public static string STREAM_DICT = $"{typeof(AbstractMethodWithFileUpload<TInput>).FullName}.{nameof(STREAM_DICT)}";

        /// <summary>
        /// 获取上传文件路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract string GetUploadFilePath(IOwinContext context, string name);

        public override TInput HandleParameter(IOwinContext context, TInput input)
        {
            if (input == null)
                return null;
            foreach (var pi in typeof(TInput).GetProperties())
            {
                if (pi.PropertyType == typeof(FileInfo))
                {
                    var filePath = GetUploadFilePath(context, pi.Name);
                    pi.SetValue(input, new FileInfo(filePath));
                }
            }
            return input;
        }

        public override void HandleFileUpload(IOwinContext context, string name, string fileName, string contentType, string contentDisposition, byte[] buffer, int bytes)
        {
            if (bytes == 0)
                return;
            //得到流字典
            var streamDict = context.Get<Dictionary<string, Stream>>(STREAM_DICT);
            if (streamDict == null)
            {
                streamDict = new Dictionary<string, Stream>();
                context.Set(STREAM_DICT, streamDict);
            }
            //得到流
            if (!streamDict.ContainsKey(name))
            {
                var filePath = GetUploadFilePath(context, name);
                FileInfo fileInfo = new FileInfo(filePath);
                var dir = fileInfo.Directory;
                if (!dir.Exists)
                    dir.Create();
                streamDict[name] = fileInfo.OpenWrite();
            }
            //写入流
            var stream = streamDict[name];
            stream.Seek(0, SeekOrigin.End);
            stream.Write(buffer, 0, bytes);
        }

        public override void FinishFileUpload(IOwinContext context)
        {
            //得到流字典
            var streamDict = context.Get<Dictionary<string, Stream>>(STREAM_DICT);
            if (streamDict == null)
                return;
            foreach (var stream in streamDict.Values)
            {
                stream.Flush();
                stream.Close();
            }
            streamDict.Clear();
        }
    }
}
