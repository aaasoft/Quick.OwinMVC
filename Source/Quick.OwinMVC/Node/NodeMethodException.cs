using Microsoft.Owin;
using Quick.OwinMVC.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node
{
    /// <summary>
    /// Node方法异常，用于返回错误信息
    /// </summary>
    public class NodeMethodException : Exception
    {
        public object MethodData { get; protected set; }

        public NodeMethodException(string message)
            : this(-1, message)
        { }

        public NodeMethodException(int code, string message)
            : this(code, message, null)
        { }

        public NodeMethodException(IOwinContext context, Enum enumValue, params object[] args)
            : this(context.GetTextManager(), enumValue, args)
        { }

        public NodeMethodException(TextManager textManager, Enum enumValue, params object[] args)
            : this(Convert.ToInt32(enumValue), textManager.GetText(enumValue, args))
        { }

        public NodeMethodException(int code, string message, object methodData)
            : base(message)
        {
            HResult = code;
            MethodData = methodData;
        }
    }
}
