using Quick.OwinMVC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Service
{
    /// <summary>
    /// 插件服务
    /// </summary>
    public interface IService : IRunnable
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        String Name { get; }
    }
}
