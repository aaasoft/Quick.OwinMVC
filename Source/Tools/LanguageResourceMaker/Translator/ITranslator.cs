using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageResourceMaker.Translator
{
    public interface ITranslator
    {
        /// <summary>
        /// 获取支持的语言列表
        /// </summary>
        /// <returns></returns>
        String[] GetSupportLanguages();
        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="from">源语言</param>
        /// <param name="to">目的语言</param>
        /// <param name="source">原句子</param>
        /// <returns></returns>
        String Translate(String from, String to, String source);
    }
}
