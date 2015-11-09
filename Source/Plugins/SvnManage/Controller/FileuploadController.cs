using Quick.OwinMVC.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Owin;
using Quick.OwinMVC.Routing;
using Quick.OwinMVC.Utils;
using System.IO;

namespace SvnManage.Controller
{
    [Route("fileupload")]
    public class FileuploadController : ViewController
    {
        protected override string doGet(IOwinContext context, IDictionary<string, object> data)
        {
            return base.doGet(context, data);
        }

        protected override string doPost(IOwinContext context, IDictionary<string, object> data)
        {
            var arg_name = (String)null;
            var stream = (Stream)null;

            MultipartFormDataUtils.HandleMultipartData(context.Request, part =>
            {
                switch (part.Name)
                {
                    case "name":
                        arg_name = part.Data;
                        break;
                }
            }, (name, fileName, type, disposition, buffer, bytes) =>
            {
                if (stream == null)
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    stream = new FileStream(fileName, FileMode.OpenOrCreate);
                }
                stream.Write(buffer, 0, bytes);
            });
            if (stream != null)
                stream.Close();
            return base.doPost(context, data);
        }
    }
}
