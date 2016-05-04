using Quick.OwinMVC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Startup
{
    public class EntranceParameter
    {
        private String _ConfigFilePath;
        /// <summary>
        /// Quick.OwinMVC配置文件路径
        /// </summary>
        public String ConfigFilePath
        {
            get { return _ConfigFilePath; }
            set
            {
                _ConfigFilePath = value;
                Properties = PropertyUtils.LoadFile(ConfigFilePath);
            }
        }
        /// <summary>
        /// 配置
        /// </summary>
        public IDictionary<String, String> Properties { get; private set; }
        /// <summary>
        /// 启动参数
        /// </summary>
        public String[] StartupArguments { get; set; }
    }
}
