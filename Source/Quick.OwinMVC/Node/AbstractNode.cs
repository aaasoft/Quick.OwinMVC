using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Quick.OwinMVC.Node
{
    public abstract class AbstractNode : INode
    {
        public const String HTTP_METHOD_GET = "GET";
        public const String HTTP_METHOD_POST = "POST";
        public const String HTTP_METHOD_PUT = "PUT";
        public const String HTTP_METHOD_DELETE = "DELETE";

        //以类名作为ID
        public string Id { get { return this.GetType().Name; } }

        public abstract string Name { get; }

        private Dictionary<String, INode> childDict = new Dictionary<string, INode>();
        private Dictionary<String, IMethod> methodDict = new Dictionary<string, IMethod>();

        public AbstractNode(params INode[] nodes)
        {
            if (nodes != null && nodes.Length > 0)
                foreach (var node in nodes)
                    childDict[node.Id] = node;
        }

        public void AddGetMethod(IMethod method)
        {
            AddMethod(HTTP_METHOD_GET, method);
        }

        public void AddPostMethod(IMethod method)
        {
            AddMethod(HTTP_METHOD_POST, method);
        }

        private void AddMethod(String httpMethod, IMethod method)
        {
            methodDict[httpMethod] = method;
        }

        public virtual INode GetChild(string id)
        {
            if (childDict.ContainsKey(id))
                return childDict[id];
            if (childDict.ContainsKey(String.Empty))
            {
                return childDict[String.Empty];
            }
            return null;
        }

        public virtual INode[] GetChildren()
        {
            return childDict.Values.ToArray();
        }

        public IDictionary<string, IMethod> GetMethods()
        {
            return methodDict;
        }

        public IMethod GetMethod(string httpMethod)
        {
            if (methodDict.ContainsKey(httpMethod))
                return methodDict[httpMethod];
            return null;
        }
    }
}
