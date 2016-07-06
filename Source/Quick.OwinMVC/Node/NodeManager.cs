﻿using Quick.OwinMVC.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Quick.OwinMVC.Node
{
    public class NodeManager : INode
    {
        public static NodeManager Instance { get; } = new NodeManager();

        private Dictionary<String, INode> dict = new Dictionary<string, INode>();
        private Dictionary<string, IMethod> methodDict = new Dictionary<string, IMethod>();

        /// <summary>
        /// 传入参数处理器
        /// </summary>
        public Func<IMethod, IOwinContext, Object, Object> ParameterHandler { get; set; }
        /// <summary>
        /// 返回值处理器
        /// </summary>
        public Func<IMethod, Object, Object> ReturnValueHandler { get; set; }
        /// <summary>
        /// 异常处理器
        /// </summary>
        public Func<Exception,Object> ExceptionHandler { get; set; }

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
                method.Value.Path = NodeApiMiddleware.Instance.Prefix + String.Join("/", nodeStack
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

        string INode.Id { get; } = null;

        string INode.Name { get; } = null;

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
    }
}
