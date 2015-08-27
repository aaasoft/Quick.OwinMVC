using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    public interface IHttpController
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="properties"></param>
        void Init(IDictionary<String, String> properties);

        /// <summary>
        /// 提供服务
        /// </summary>
        /// <param name="context"></param>
        /// <param name="plugin"></param>
        /// <param name="path"></param>
        void Service(IOwinContext context, String plugin, String path);
    }
}
