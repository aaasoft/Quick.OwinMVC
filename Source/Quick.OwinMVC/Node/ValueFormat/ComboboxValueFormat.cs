using Newtonsoft.Json;
using Quick.OwinMVC.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node.ValueFormat
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ComboboxValueFormat : Attribute, IValueFormat
    {
        [JsonIgnore]
        public override object TypeId
        {
            get { return base.TypeId; }
        }

        public IEnumerable<KeyValuePair<String, String>> Values { get; set; }
        public object Values_IValueTransfer
        {
            get { return null; }
            set
            {
                var type = value as Type;
                if (type == null)
                    return;
                if (!typeof(IValueTransfer).IsAssignableFrom(type))
                    return;
                var valueTransfer = System.Activator.CreateInstance(type) as IValueTransfer;
                if (valueTransfer == null)
                    return;
                Values = (valueTransfer.GetValue() as IEnumerable<KeyValuePair<String, String>>)?
                    .ToList();
            }
        }

        [JsonIgnore]
        public Object ValuesEnum { get; set; }
        /// <summary>
        /// 当ValuesEnum属性设置为枚举类型时，key的值是否取枚举的名称。如果为否时，取枚举的数字
        /// </summary>
        [JsonIgnore]
        public Boolean ValuesEnum_KeyByEnumName { get; set; }
        /// <summary>
        /// 当ValuesEnum属性设置为枚举类型时，value的值是否取枚举的名称。如果为否时，取文件资源
        /// </summary>
        [JsonIgnore]
        public Boolean ValuesEnum_ValueByEnumName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Group { get; set; }

        [JsonIgnore]
        public Type ProviderType
        {
            get { return null; }
            set { Provider = value.FullName; }
        }

        [JsonIgnore]
        public Type SubProviderType
        {
            get { return null; }
            set { SubProvider = value.FullName; }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String Provider { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String SubProvider { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String SubLabel { get; set; }

        public Boolean OnChage { get; set; } = false;

        public Enum GetValueEnum(Object value)
        {
            Type enumType = ValuesEnum as Type;
            if (ValuesEnum_KeyByEnumName)
            {
                return Enum.Parse(enumType, value.ToString()) as Enum;
            }
            else
            {
                return Enum.ToObject(enumType, value) as Enum;
            }
        }

        public void Handle(TextManager textManager, FormFieldInfo field)
        {
            if (OnChage && string.IsNullOrWhiteSpace(SubProvider))
                SubProvider = Provider;


            var valuesEnum = this.ValuesEnum as Enum;
            if (valuesEnum != null)
            {
                var txt = textManager.GetText(valuesEnum);
                this.Values = JsonConvert.DeserializeObject<KeyValuePair<String, String>[]>(txt);
            }

            var valuesEnumType = this.ValuesEnum as Type;
            if (valuesEnumType != null
            && valuesEnumType.IsEnum
            && valuesEnumType.GetCustomAttributes(typeof(TextResourceAttribute), false).Length > 0)
            {
                var list = new List<KeyValuePair<String, String>>();
                foreach (Enum item in Enum.GetValues(valuesEnumType))
                {
                    String key = null;
                    String value = null;
                    if (ValuesEnum_ValueByEnumName)
                        value = Enum.GetName(valuesEnumType, item);
                    else
                        value = textManager.GetText(item);

                    if (ValuesEnum_KeyByEnumName)
                        key = Enum.GetName(valuesEnumType, item);
                    else
                        key = Convert.ToInt32(item).ToString();

                    list.Add(new KeyValuePair<string, string>(key, value));
                }
                this.Values = list;
            }
        }

        public object GetValue(TextManager textManager, object obj)
        {
            return textManager.GetText(GetValueEnum(obj));
        }
    }
}
