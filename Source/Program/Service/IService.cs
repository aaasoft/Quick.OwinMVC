using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Program.Service
{
    /// <summary>
    /// 内部微服务
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        String Name { get; }
        /// <summary>
        /// 启动
        /// </summary>
        void Start();
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }
}
