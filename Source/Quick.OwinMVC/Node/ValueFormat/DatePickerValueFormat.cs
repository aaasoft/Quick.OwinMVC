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
    public class DatePickerValueFormat : Attribute, IValueFormat
    {
        [JsonIgnore]
        public override object TypeId
        {
            get { return base.TypeId; }
        }

        void IValueFormat.Handle(TextManager textManager, FormFieldInfo field)
        {
            if (string.IsNullOrWhiteSpace(this.DateTimeFormat))
            {
                this.DateTimeFormat = "YYYY-MM-DD";
                if (IsContainTime)
                    this.DateTimeFormat = "YYYY-MM-DD hh:mm:ss";
            }
        }

        public string InitDateTimeFormat()
        {
            if (string.IsNullOrWhiteSpace(this.DateTimeFormat))
            {
                if (IsContainTime)
                    return "yyyy-MM-dd HH:mm:ss";
                return "yyyy-MM-dd";
            }

            return this.DateTimeFormat;
        }

        public Enum GetValueEnum(object value)
        {
            throw new NotImplementedException();
        }

        public object GetValue(TextManager textManager, object obj)
        {
            return Convert.ToDateTime(obj).ToString(InitDateTimeFormat());
        }

        public string DateTimeFormat { get; set; }
        public string MinDate { get; set; }
        public string MaxDate { get; set; }

        public Boolean IsContainTime { get; set; }

    }
}
