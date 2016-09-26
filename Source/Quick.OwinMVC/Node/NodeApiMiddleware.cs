using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC.Controller;
using Quick.OwinMVC.Hunter;

namespace Quick.OwinMVC.Node
{
    public class NodeApiMiddleware : OwinMiddleware, IHungryPropertyHunter
    {
        public const string JSONP_CALLBACK = "callback";
        public static NodeApiMiddleware Instance { get; private set; }
        public static string Prefix = "/api/";
        private Encoding encoding = new UTF8Encoding(false);

        public NodeApiMiddleware(OwinMiddleware next = null) : base(next)
        {
            Instance = this;
        }

        /// <summary>
        /// 获取节点路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetNodePath(string path)
        {
            if (path.StartsWith(Prefix))
                return path.Substring(Prefix.Length);
            return null;
        }

        public override Task Invoke(IOwinContext context)
        {
            var req = context.Request;
            var rep = context.Response;

            var nodePath = GetNodePath(req.Path.Value);
            if (!string.IsNullOrEmpty(nodePath))
            {
                var currentNode = NodeManager.Instance.GetNode(nodePath);
                var nodeMethod = currentNode?.GetMethod(req.Method);
                if (nodeMethod == null)
                    return Next.Invoke(context);

                Object data = null;
                try
                {
                    //调用方法处理
                    if (NodeManager.Instance.MethodInvokeHandler != null)
                        nodeMethod = NodeManager.Instance.MethodInvokeHandler.Invoke(nodeMethod, context);
                    //调用
                    data = nodeMethod?.Invoke(context);
                    //返回值处理
                    if (NodeManager.Instance.ReturnValueHandler != null)
                        data = NodeManager.Instance.ReturnValueHandler.Invoke(nodeMethod, data);
                }
                catch (NodeMethodException ex)
                {
                    data = ApiResult.Error(ex.HResult, ex.Message, ex.MethodData);
                }
                catch (AppDomainUnloadedException ex)
                when (ex.InnerException is NodeMethodHandledException)
                {
                    return Task.Delay(0);
                }
                catch (Exception ex)
                {
                    if (NodeManager.Instance.ExceptionHandler == null)
                        throw ex;
                    data = NodeManager.Instance.ExceptionHandler.Invoke(ex);
                }
                //要输出的内容
                string result = null;
                //JSON序列化的结果
                var json = JsonConvert.SerializeObject(data, NodeManager.Instance.JsonSerializerSettings);
                var jsonpCallback = req.Query[JSONP_CALLBACK];

                if (string.IsNullOrEmpty(jsonpCallback))
                {
                    rep.ContentType = "text/json; charset=UTF-8";
                    result = json;
                }
                else
                {
                    rep.ContentType = "application/x-javascript";
                    result = $"{jsonpCallback}({json})";
                }
                rep.Expires = new DateTimeOffset(DateTime.Now);
                return context.Output(encoding.GetBytes(result), true);
            }
            return Next.Invoke(context);
        }

        public void Hunt(IDictionary<string, string> properties)
        {
            NodeManager.Instance.Init(properties);
        }
    }
}
