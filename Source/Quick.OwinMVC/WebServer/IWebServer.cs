using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.WebServer
{
    public interface IWebServer : IDisposable
    {
        /// <summary>
        /// 当前是否正在运行
        /// </summary>
        Boolean IsRuning { get; }
        /// <summary>
        /// 启动
        /// </summary>
        void Start(Func<IDictionary<string, object>, Task> app, IPEndPoint endpoint);
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }
}
