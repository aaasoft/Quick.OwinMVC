using Quick.OwinMVC.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using Newtonsoft.Json;
using Quick.OwinMVC.Hunter;

namespace Quick.OwinMVC.Node
{
    public class NodeManager : INode
    {
        private static NodeManager _Instance = new NodeManager();
        public static NodeManager Instance { get{return _Instance;} }

        private Dictionary<String, INode> dict = new Dictionary<string, INode>();
        private Dictionary<string, IMethod> methodDict = new Dictionary<string, IMethod>();

        /// <summary>
        /// JSON序列化设置
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
        /// <summary>
        /// 方法调用处理器
        /// </summary>
        public Func<IMethod, IOwinContext, IMethod> MethodInvokeHandler { get; set; }
        /// <summary>
        /// 传入参数处理器
        /// </summary>
        public Func<IMethod, IOwinContext, Object, Object> ParameterHandler { get; set; }
        /// <summary>
        /// 返回值处理器
        /// </summary>
        public Func<IMethod, Object, DateTime, Object> ReturnValueHandler { get; set; }
        /// <summary>
        /// 异常处理器
        /// </summary>
        public Func<Exception, Object> ExceptionHandler { get; set; }

        public void Register(INode t)
        {
            Stack<INode> nodeStack = new Stack<INode>();
            handleNode(t, nodeStack);
            dict[t.Id] = t;
        }

        private void handleNode(INode node, Stack<INode> nodeStack)
        {
            nodeStack.Push(node);
            foreach (var method in node.GetMethods())
            {
                method.Value.HttpMethod = method.Key;
                method.Value.Path = NodeApiMiddleware.Prefix + String.Join("/", nodeStack
                        .Reverse()
                        .Select(t => t.Id)
                        .ToArray());
            }
            foreach (var childNode in node.GetChildren())
            {
                handleNode(childNode, nodeStack);
            }
            nodeStack.Pop();
        }

        public void Unregister(INode t)
        {
            if (dict.ContainsKey(t.Id))
                dict.Remove(t.Id);
        }

        string INode.Id { get { return null; } }

        string INode.Name { get { return null; } }

        INode[] INode.GetChildren()
        {
            return dict.Values.ToArray();
        }

        INode INode.GetChild(string id)
        {
            if (dict.ContainsKey(id))
                return dict[id];
            return null;
        }

        IDictionary<string, IMethod> INode.GetMethods()
        {
            return methodDict;
        }

        IMethod INode.GetMethod(string httpMethod)
        {
            if (methodDict.ContainsKey(httpMethod))
                return methodDict[httpMethod];
            return null;
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public INode GetNode(string path)
        {
            var nodeSegments = path.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            INode currentNode = this;
            IEnumerable<String> currentSegments = nodeSegments;
            while (currentNode != null && currentSegments.Count() > 0)
            {
                var currentSegment = currentSegments.First();
                currentNode = currentNode.GetChild(currentSegment);
                if (currentNode == null)
                    return null;
                currentSegments = currentSegments.Skip(1);
            }
            return currentNode;
        }

        public void Init(IDictionary<string, string> properties)
        {
            _Init(this, properties);
        }

        private void _Init(INode node, IDictionary<string, string> properties)
        {
            HunterUtils.TryHunt(node, properties);
            HunterUtils.TryHunt(node.GetMethods().Values, properties);
            foreach (var childNode in node.GetChildren())
                _Init(childNode, properties);
        }
    }
}
