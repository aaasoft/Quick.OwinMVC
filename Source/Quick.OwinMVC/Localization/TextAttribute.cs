using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.OwinMVC.Localization
{
    /// <summary>
    /// 语言资源枚举
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class TextResourceAttribute : Attribute
    {
    }

    /// <summary>
    /// 语言资源枚举值对应的文本
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TextAttribute : Attribute
    {
        public const String DEFAULT_LANGUAGE = "zh-CN";

        public String Language { get; set; }
        public String Value { get; set; }

        public TextAttribute(String value)
        {
            this.Language =DEFAULT_LANGUAGE;
            this.Value = value;
        }

        public TextAttribute(String language, String value)
        {
            this.Language = language;
            this.Value = value;
        }
    }
}
