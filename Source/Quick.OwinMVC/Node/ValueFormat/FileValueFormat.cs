using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quick.OwinMVC.Localization;

namespace Quick.OwinMVC.Node.ValueFormat
{
    public class FileValueFormat : Attribute, IValueFormat
    {
        [JsonIgnore]
        public override object TypeId
        {
            get { return base.TypeId; }
        }

        public string FileType { set; get; }

        public string FolderName { set; get; } = "default";

        public void Handle(TextManager textManager, FormFieldInfo field)
        {

        }

        public object GetValue(TextManager textManager, object obj)
        {
            return obj;
        }
    }
}
