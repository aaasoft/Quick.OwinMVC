using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    public interface IPluginController
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="properties"></param>
        void Init(IDictionary<String, String> properties);
    }
}
