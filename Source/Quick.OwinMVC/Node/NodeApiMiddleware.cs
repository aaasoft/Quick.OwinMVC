using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC.Controller;

namespace Quick.OwinMVC.Node
{
    public class NodeApiMiddleware : OwinMiddleware
    {
        public static NodeApiMiddleware Instance { get; private set; }
        public String Prefix = "/api/";
        private Encoding encoding = new UTF8Encoding(false);

        public NodeApiMiddleware(OwinMiddleware next = null) : base(next)
        {
            Instance = this;
        }

        public override Task Invoke(IOwinContext context)
        {
            var req = context.Request;
            var rep = context.Response;
            var requestPath = req.Path.Value;
            if (requestPath.StartsWith(Prefix))
            {
                var nodePath = requestPath.Substring(Prefix.Length);
                var nodeSegments = nodePath.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                INode currentNode = NodeManager.Instance;
                IEnumerable<String> currentSegments = nodeSegments;
                while (currentNode != null && currentSegments.Count() > 0)
                {
                    var currentSegment = currentSegments.First();
                    currentNode = currentNode.GetChild(currentSegment);
                    if (currentNode == null)
                        return Next.Invoke(context);

                    currentSegments = currentSegments.Skip(1);
                }

                var nodeMethod = currentNode?.GetMethod(req.Method);
                if (nodeMethod == null)
                    return Next.Invoke(context);

                Object data = null;
                try
                {
                    data = nodeMethod?.Invoke(context);
                    if (data == null)
                        return Task.Delay(0);
                    if (NodeManager.Instance.ReturnValueHandler != null)
                        data = NodeManager.Instance.ReturnValueHandler.Invoke(nodeMethod, data);
                }
                catch (Exception ex)
                {
                    if (NodeManager.Instance.ExceptionHandler == null)
                        throw ex;
                    data = NodeManager.Instance.ExceptionHandler.Invoke(ex);
                }
                var content = encoding.GetBytes(JsonConvert.SerializeObject(data));
                rep.Expires = new DateTimeOffset(DateTime.Now);
                rep.ContentType = "text/json; charset=UTF-8";
                return context.Output(content, true);
            }
            return Next.Invoke(context);
        }
    }
}
