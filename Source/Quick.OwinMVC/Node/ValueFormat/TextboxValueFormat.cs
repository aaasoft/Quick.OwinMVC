using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC.Localization;
using Newtonsoft.Json;

namespace Quick.OwinMVC.Node.ValueFormat
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TextboxValueFormat : Attribute, IValueFormat
    {
        [JsonIgnore]
        public override object TypeId
        {
            get { return base.TypeId; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxLength { get; set; }

        private String _Regex;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Regex
        {
            get { return _Regex; }
            set
            {
                this._Regex = value;
            }
        }

        public Boolean IsMultiLine { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Boolean IsPassword { get; set; }

        public void Handle(TextManager textManager, FormFieldInfo field)
        {
        }
        
        public object GetValue(TextManager textManager, object obj)
        {
            return obj;
        }
    }
}
