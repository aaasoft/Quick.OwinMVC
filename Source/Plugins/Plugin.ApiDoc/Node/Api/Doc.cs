using Quick.OwinMVC.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Plugin.ApiDoc.Node.Api
{
    public class Doc : AbstractNode
    {
        public Doc()
        {
            AddGetMethod(new Get());
        }

        public class Get : AbstractMethod<DocParameter>
        {
            public override string Name { get { return "获取API文档"; } }
            public override string[] Tags { get { return new[] { "doc", "api", "internal", "hidden" }; } }

            public override object Invoke(IOwinContext context, DocParameter input)
            {
                var req = context.Request;
                var contextPath = req.Get<String>("ContextPath");

                Utils.OutputXml(context, input, string.Format("{0}{1}/resource/api/doc.xslt", contextPath, this.GetType().Assembly.GetName().Name), input.Output == DocOutput.XML);
                throw NodeMethodHandledException.Instance;
            }
        }
    }
}
