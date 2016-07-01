using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NodeParameterAttribute : Attribute
    {
        public bool ValueToObject { get; set; }
        public String[] IgnoreProperties { get; set; }

        public NodeParameterAttribute(bool valueToObject, params String[] ignoreProperties)
        {
            ValueToObject = valueToObject;
            IgnoreProperties = ignoreProperties;
        }
    }
}
