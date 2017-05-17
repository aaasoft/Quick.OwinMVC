using Quick.OwinMVC.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.ApiDoc.Node.Api
{
    public class Api : AbstractNode
    {
        public override string Name { get; } = "API";

        public Api(params INode[] nodes) : base(nodes) { }
    }
}
