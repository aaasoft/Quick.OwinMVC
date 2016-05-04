using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Model
{
    /// <summary>
    /// 可运行接口(可启动和停止)
    /// </summary>
    public interface IRunnable
    {
        void Start();
        void Stop();
    }
}
