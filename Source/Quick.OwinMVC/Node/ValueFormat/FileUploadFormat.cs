using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC.Localization;

namespace Quick.OwinMVC.Node.ValueFormat
{
    public class FileUploadFormat : Attribute, IValueFormat
    {
        /// <summary>
        /// 允许上传的最大文件大小
        /// </summary>
        public long MaxFileSize { get; set; }

        public object GetValue(TextManager textManager, object obj)
        {
            return obj;
        }

        public void Handle(TextManager textManager, FormFieldInfo field)
        {

        }
    }
}
