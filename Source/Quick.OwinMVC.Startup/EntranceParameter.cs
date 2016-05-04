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
        /// <summary>
        /// 基础路径
        /// </summary>
        public String BasePath { get; set; }
        /// <summary>
        /// 库目录
        /// </summary>
        public String LibsPath { get; set; }
        /// <summary>
        /// 插件目录
        /// </summary>
        public String PluginsPath { get; set; }
        /// <summary>
        /// 是否加载全部的插件
        /// </summary>
        public bool LoadAllPlugins { get; set; }
    }
}
