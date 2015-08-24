using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    /// <summary>
    /// MVC控制器
    /// </summary>
    public interface IMvcController
    {
        /// <summary>
        /// 执行MVC控制器，返回视图名称
        /// </summary>
        /// <param name="context"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        String Service(IOwinContext context, IDictionary<String,Object> data);
    }
}
