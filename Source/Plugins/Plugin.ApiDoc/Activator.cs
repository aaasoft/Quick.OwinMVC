using Quick.OwinMVC.Node;
using Quick.OwinMVC.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.ApiDoc
{
    public class Activator : IPluginActivator
    {
        private INode rootNode = null;
        public void Start()
        {
            rootNode = new Node.Api.Api(
                    new Node.Api.Doc(),
                    new Node.Api.Detail(),
                    new Node.Api.Table()
                );
            NodeManager.Instance.Register(rootNode);
        }

        public void Stop()
        {
            NodeManager.Instance.Unregister(rootNode);
        }
    }
}
