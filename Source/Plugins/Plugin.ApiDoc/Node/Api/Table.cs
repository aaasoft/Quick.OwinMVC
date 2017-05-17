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
            public override string Name { get; } = "获取API表格";
            public override string[] Tags { get; } = { "doc", "api", "internal", "hidden" };

            public override object Invoke(IOwinContext context, DocParameter input)
            {
                Utils.OutputXml(context, input, $"/{this.GetType().Assembly.GetName().Name}/resource/api/table.xslt");
                throw NodeMethodHandledException.Instance;
            }
        }
    }
}
