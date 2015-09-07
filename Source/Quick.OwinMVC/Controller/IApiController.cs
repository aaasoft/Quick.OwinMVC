using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    /// <summary>
    /// API控制器
    /// </summary>
    public interface IApiController : IPluginController
    {
        /// <summary>
        /// 执行API控制器，返回输出对象
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Object Service(IOwinContext context);
    }
}
