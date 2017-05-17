using Quick.OwinMVC.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node.ValueFormat
{
    public interface IValueFormat
    {
        void Handle(TextManager textManager, FormFieldInfo field);
        object GetValue(TextManager textManager, object obj);
    }
}
