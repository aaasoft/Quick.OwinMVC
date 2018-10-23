using Quick.OwinMVC.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Plugin.ApiDoc.Node.Api
{
    public class Detail : AbstractNode
    {
        public override string Name => "详情";

        public Detail()
        {
            AddGetMethod(new Get());
        }

        public class GetParameter
        {
            [FormFieldInfo(Key = nameof(Id), Name = "API编号", Description = "例如：/api/Api/Detail:GET", NotEmpty = true)]
            public String Id { get; set; }
        }

        public class Get : AbstractMethod<GetParameter>
        {
            public override string Name { get; } = "获取API接口详情";
            public override string[] Tags { get; } = { "api", "internal" };
            public override string ReturnValueExample { get; } =
@"{
    ""code"": 0,
    ""message"": ""获取API接口详情成功"",
    ""data"": {
        ""Path"": ""/api/Api/Detail"",
        ""Name"": ""获取API接口详情"",
        ""Description"": """",
        ""HttpMethod"": ""GET"",
        ""InvokeExample"": """",
        ""Tags"": [
            ""api"",
            ""internal""
        ],
        ""Parameters"": [
            {
                ""Type"": ""String"",
                ""Key"": ""Path"",
                ""Name"": ""API路径"",
                ""NotEmpty"": true,
                ""IsHidden"": false,
                ""IsReadOnly"": false,
                ""IgnoreSelectHidden"": false
            },
            {
                ""Type"": ""String"",
                ""Key"": ""Method"",
                ""Name"": ""HTTP方法"",
                ""NotEmpty"": true,
                ""IsHidden"": false,
                ""IsReadOnly"": false,
                ""IgnoreSelectHidden"": false
            }
        ]
    }
}";
            public override object Invoke(IOwinContext context, GetParameter input)
            {
                var idArray = input.Id.Split(new Char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (idArray.Length < 2)
                    throw new NodeMethodException($"[{input.Id}]不是有效的API编号！");

                var req = context.Request;
                var contextPath = req.Get<String>("ContextPath");

                var path = contextPath + idArray[0];
                var httpMethod = idArray[1];

                var nodePath = path.Substring(NodeApiMiddleware.Prefix.Length);
                var nodeSegments = nodePath.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                INode currentNode = NodeManager.Instance;
                IEnumerable<String> currentSegments = nodeSegments;
                while (currentNode != null && currentSegments.Count() > 0)
                {
                    var currentSegment = currentSegments.First();
                    currentNode = currentNode.GetChild(currentSegment);
                    if (currentNode == null)
                        throw new NodeMethodException($"未找到路径为[{path}]的节点！");
                    currentSegments = currentSegments.Skip(1);
                }
                var nodeMethod = currentNode.GetMethod(httpMethod);
                if (nodeMethod == null)
                    throw new NodeMethodException($"路径为[{path}]的节点没有[{httpMethod}]方法！");

                return new
                {
                    nodeMethod.Path,
                    nodeMethod.Name,
                    nodeMethod.Description,
                    nodeMethod.HttpMethod,
                    nodeMethod.InvokeExample,
                    nodeMethod.Tags,
                    Parameters = FormFieldInfo.GetAll(GetTextManager(context), nodeMethod.InputType)
                };
            }
        }
    }
}
