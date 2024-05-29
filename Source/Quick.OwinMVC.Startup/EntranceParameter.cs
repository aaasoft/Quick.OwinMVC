using Owin;
using Quick.OwinMVC.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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
                if (File.Exists(_ConfigFilePath))
                    Properties = PropertyUtils.LoadFile(_ConfigFilePath);
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

        private String _BasePath;
        /// <summary>
        /// 基础路径
        /// </summary>
        public String BasePath
        {
            get { return _BasePath; }
            set
            {
                _BasePath = value;
                if (!String.IsNullOrEmpty(value))
                    Environment.CurrentDirectory = value;
            }
        }

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

        /// <summary>
        /// 当服务正在启动时
        /// </summary>
        public Action OnServiceStarting { get; set; }
        /// <summary>
        /// 当服务启动后
        /// </summary>
        public Action OnServiceStarted { get; set; }
        /// <summary>
        /// 当服务正在停止时
        /// </summary>
        public Action OnServiceStoping { get; set; }
        /// <summary>
        /// 当服务停止后
        /// </summary>
        public Action OnServiceStoped { get; set; }

        /// <summary>
        /// 是否分隔顶层控件
        /// </summary>
        public bool SplitTopControl { get; set; }
        /// <summary>
        /// 获取控件的函数
        /// </summary>
        public Func<KeyValuePair<String, Object>[]> GetControlConfigFunc { get; set; }

        public EntranceParameter()
        {
            SplitTopControl = true;
        }
    }
}
