using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC.Localization;
using Newtonsoft.Json;

namespace Quick.OwinMVC.Node.ValueFormat
{
    public class SelectValueFormat : Attribute, IValueFormat
    {
        [JsonIgnore]
        public override object TypeId
        {
            get { return base.TypeId; }
        }

        /// <summary>
        /// 是否能够多选
        /// </summary>
        public Boolean MultiSelect { get; set; }
        /// <summary>
        /// 是否能够选择目录
        /// </summary>
        public Boolean FolderSelect { get; set; }

        [JsonIgnore]
        public Type ProviderType
        {
            get { return null; }
            set { Provider = value.FullName; }
        }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Leaf { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Group { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Provider { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Parameter { get; set; }
        public Boolean OnChage { get; set; }

        public String TriggerField { get; set; }

        public SelectValueFormat()
        {
            OnChage = false;
        }


        public void Handle(TextManager textManager, FormFieldInfo field)
        {
        }

        public object GetValue(TextManager textManager, object obj)
        {
            return obj;
        }
    }
}
