using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Node
{
    public interface INode
    {
        /// <summary>
        /// 节点编号
        /// </summary>
        String Id { get; }
        /// <summary>
        /// 节点名称
        /// </summary>
        String Name { get; }

        /// <summary>
        /// 获取全部的子节点
        /// </summary>
        /// <returns></returns>
        INode[] GetChildren();
        /// <summary>
        /// 获取指定编号的子节点
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        INode GetChild(String id);

        /// <summary>
        /// 获取全部的REST方法
        /// </summary>
        /// <returns></returns>
        IDictionary<String, IMethod> GetMethods();

        /// <summary>
        /// 获取指定HTTP请求方法的REST方法
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        IMethod GetMethod(String httpMethod);
    }
}
