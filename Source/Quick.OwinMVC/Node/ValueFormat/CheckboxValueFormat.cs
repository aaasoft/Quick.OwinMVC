using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC.Localization;
using Newtonsoft.Json;

namespace Quick.OwinMVC.Node.ValueFormat
{
    [TextResource]
    public enum BoolSignEnum
    {
        [Text("<i class=\"icon-remove bigger-110 red\"></i>")]
        BoolSignEnum_0 = 0,

        [Text("<i class=\"icon-ok bigger-110 green\"></i>")]
        BoolSignEnum_1 = 1,
    }

    public class CheckboxValueFormat : Attribute, IValueFormat
    {
        [JsonIgnore]
        public Object ValuesEnum { get; set; }
        
        public object GetValue(TextManager textManager, object obj)
        {
            return textManager.GetText(GetValueEnum(obj));
        }

        public Enum GetValueEnum(object value)
        {
            Type enumType = ValuesEnum as Type;
            return Enum.Parse(enumType, string.Format("{0}", Convert.ToInt32(value))) as Enum;
        }

        public void Handle(TextManager textManager, FormFieldInfo field)
        {
        }
    }
}
