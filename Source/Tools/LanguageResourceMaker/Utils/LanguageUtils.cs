using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LanguageResourceMaker.Utils
{
    public class LanguageUtils
    {
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
