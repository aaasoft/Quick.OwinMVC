using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node
{
    public interface IMethod
    {
        /// <summary>
        /// 路径
        /// </summary>
        String Path { get; set; }
        /// <summary>
        /// HTTP方法
        /// </summary>
        String HttpMethod { get; set; }
        /// <summary>
        /// 方法名称
        /// </summary>
        String Name { get; }
        /// <summary>
        /// 方法描述
        /// </summary>
        String Description { get; }
        /// <summary>
        /// 标签
        /// </summary>
        String[] Tags { get; }
        /// <summary>
        /// 调用示例
        /// </summary>
        String InvokeExample { get; }
        /// <summary>
        /// 返回值示例
        /// </summary>
        String ReturnValueExample { get; }

        /// <summary>
        /// 输入参数类型
        /// </summary>
        Type InputType { get; }
        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        object Invoke(IOwinContext context);
    }
}
