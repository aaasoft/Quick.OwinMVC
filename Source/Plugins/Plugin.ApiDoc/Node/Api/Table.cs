using Quick.OwinMVC.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Plugin.ApiDoc.Node.Api
{
    public class Table : AbstractNode
    {
        public Table()
        {
            AddGetMethod(new Get());
        }

        public class Get : AbstractMethod<DocParameter>
        {
            public override string Name { get { return "获取API表格"; } }
            public override string[] Tags { get { return new[] { "doc", "api", "internal", "hidden" }; } }

            public override object Invoke(IOwinContext context, DocParameter input)
            {
                Utils.OutputXml(context, input, string.Format("/{0}/resource/api/table.xslt", this.GetType().Assembly.GetName().Name));
                throw NodeMethodHandledException.Instance;
            }
        }
    }
}
