using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageResourceMaker.Translator
{
    public class BaiduTranslator : ITranslator
    {
        private const String TRANSLATE_URL = "http://fanyi.baidu.com/v2transapi?from={0}&to={1}&transtype=trans&query={2}";
        /*
            'auto' => '自动检测',
            'ara' => '阿拉伯语',
            'de' => '德语',
            'ru' => '俄语',
            'fra' => '法语',
            'kor' => '韩语',
            'nl' => '荷兰语',
            'pt' => '葡萄牙语',
            'jp' => '日语',
            'th' => '泰语',
            'wyw' => '文言文',
            'spa' => '西班牙语',
            'el' => '希腊语',
            'it' => '意大利语',
            'en' => '英语',
            'yue' => '粤语',
            'zh' => '中文' 
         */

        private String languageMap = @"
zh-CN => zh
en-US => en
de-DE => de
ru-RU => ru
fr-FR => fra
ko-KR => kor
nl-NL => nl
pt-PT => pt
ja-JP => jp
th-TH => th
es-ES => spa
el-GR => el
it-IT => it
            ";
        private Dictionary<String, String> languageMapDict = new Dictionary<string, string>();
        public BaiduTranslator()
        {
            //(?'key'.+?)\s*=>\s*(?'value'.+)
            Regex regex = new Regex("(?'key'.+?)\\s*=>\\s*(?'value'.+)");
            foreach (Match match in regex.Matches(languageMap))
            {
                var keyGroup = match.Groups["key"];
                var valueGroup = match.Groups["value"];
                if (!keyGroup.Success || !valueGroup.Success)
                    continue;
                String key = keyGroup.Value.Trim();
                String value = valueGroup.Value.Trim();
                languageMapDict.Add(key, value);
            }
        }


        public string[] GetSupportLanguages()
        {
            return languageMapDict.Keys.ToArray();
        }

        public class RootClass
        {
            public TransResult trans_result { get; set; }
        }

        public class TransResult
        {
            public Data[] data { get; set; }
        }

        public class Data
        {
            public String dst { get; set; }
        }

        public string Translate(string from, string to, string source)
        {
            String currentUrl = String.Format(TRANSLATE_URL, languageMapDict[from], languageMapDict[to], source);
            WebClient webClient = new WebClient();
            String ret = webClient.DownloadString(currentUrl);
            RootClass obj = Newtonsoft.Json.JsonConvert.DeserializeObject<RootClass>(ret);
            if (obj == null || obj.trans_result == null || obj.trans_result.data == null)
                return null;
            return obj.trans_result.data[0].dst;
        }
    }
}
