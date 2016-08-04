using HttpMultipartParser;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.OwinMVC.Utils
{
    public class MultipartFormDataUtils
    {
        /// <summary>
        /// 处理多段表单数据
        /// </summary>
        /// <param name="req"></param>
        /// <param name="parameterHandler"></param>
        /// <param name="fileHandler"></param>
        public static void HandleMultipartData(IOwinRequest req,
            StreamingMultipartFormDataParser.ParameterDelegate parameterHandler,
            StreamingMultipartFormDataParser.FileStreamDelegate fileHandler,
            StreamingMultipartFormDataParser.StreamClosedDelegate streamClosedDelegate = null)
        {
            if (!req.ContentType.StartsWith("multipart/form-data;"))
                throw new ArgumentException("'ContentType' not start with 'multipart/form-data;'.");
            var parser = new StreamingMultipartFormDataParser(req.Body);
            parser.ParameterHandler = parameterHandler;
            parser.FileHandler = fileHandler;
            parser.StreamClosedHandler = streamClosedDelegate;
            parser.Run();
        }
    }
}
