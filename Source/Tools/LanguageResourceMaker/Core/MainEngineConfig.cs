using LanguageResourceMaker.Translator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageResourceMaker.Core
{
    /// <summary>
    /// 主引擎配置
    /// </summary>
    public class MainEngineConfig
    {
        public String InputFolder { get; set; }
        public String OutputFolder { get; set; }
        public Boolean AutoTranslate { get; set; }
        public String[] TranslateTarget { get; set; }
        /// <summary>
        /// 翻译器
        /// </summary>
        public ITranslator Translator { get; set; }
        /// <summary>
        /// 新增一条日志的Action
        /// </summary>
        public Action<String> PushLogAction { get; set; }
        /// <summary>
        /// 更新最后一条日志的Action
        /// </summary>
        public Action<String> UpdateLogAction { get; set; }
        /// <summary>
        /// 完成后的Action
        /// </summary>
        public Action OnFinishAction { get; set; }
    }
}
