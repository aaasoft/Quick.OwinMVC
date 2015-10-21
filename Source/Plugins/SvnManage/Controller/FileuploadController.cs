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
    public class FileuploadController : IViewController
    {
        public string Service(IOwinContext context, IDictionary<string, object> data)
        {
            var req = context.Request;
            switch (req.Method)
            {
                case "GET":
                    break;
                case "POST":
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
                    break;
            }
            return "fileupload";
        }
    }
}
