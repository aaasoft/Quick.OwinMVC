using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageResourceMaker.Utils
{
    public class LanguageUtils
    {
        public static String GetCurrentLanguage()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }

        /// <summary>
        /// 获取语言资源字典
        /// </summary>
        /// <param name="languageContent"></param>
        /// <returns></returns>
        public static Dictionary<String, String> GetLanguageResourceDictionary(String languageContent)
        {
            Dictionary<String, String> languageDict = new Dictionary<String, string>();

            //(?'key'.+)\s*=(?'value'.+)
            Regex regex = new Regex(@"(?'key'.+)\s*=(?'value'.+)");
            MatchCollection languageMatchCollection = regex.Matches(languageContent);
            foreach (Match match in languageMatchCollection)
            {
                var indexGroup = match.Groups["key"];
                var valueGroup = match.Groups["value"];

                if (!indexGroup.Success || !valueGroup.Success)
                    continue;
                String key = indexGroup.Value;
                String value = valueGroup.Value;
                if (value.EndsWith("\r"))
                    value = value.Substring(0, value.Length - 1);
                if (languageDict.ContainsKey(key))
                    languageDict.Remove(key);
                languageDict.Add(key, value);
            }
            return languageDict;
        }

        public static String GetToWriteLanguageText(Dictionary<String, String> textDict)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String key in textDict.Keys)
            {
                sb.AppendLine(String.Format("{0}={1}", key, textDict[key]));
            }
            return sb.ToString();
        }
    }
}
