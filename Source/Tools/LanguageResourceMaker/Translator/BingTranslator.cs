﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LanguageResourceMaker.Translator
{
    public class BingTranslator : ITranslator
    {
        public string Name { get { return "Bing翻译"; } }

        private const String TRANSLATE_URL = "http://api.microsofttranslator.com/V2/Ajax.svc/TranslateArray?from={0}&to={1}&appId=T0yi1jHo-IzX9jgQX4GoNcIORb9AUoPRxsGygHj7uU8g*&texts=%5B\"{2}\"%5D";
        private String languageMap = @"
zh-CN => zh-CHS
en-US => en
de-DE => de
ru-RU => ru
fr-FR => fr
ko-KR => ko
nl-NL => nl
pt-PT => pt
ja-JP => ja
th-TH => th
es-ES => es
el-GR => el
it-IT => it
vi => vi
            ";
        private Dictionary<String, String> languageMapDict = new Dictionary<string, string>();
        public BingTranslator()
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

        public class TranslateResult
        {
            public Int32[] OriginalTextSentenceLengths { get; set; }
            public Int32[] TranslatedTextSentenceLengths { get; set; }
            public String TranslatedText { get; set; }
        }

        public string Translate(string from, string to, string source)
        {
            String currentUrl = String.Format(TRANSLATE_URL, languageMapDict[from], languageMapDict[to], source);
            try
            {                
                WebClient webClient = new WebClient();
                String ret = webClient.DownloadString(currentUrl);
                TranslateResult[] results = Newtonsoft.Json.JsonConvert.DeserializeObject<TranslateResult[]>(ret);
                if (results == null || results.Length == 0)
                    return null;
                return results[0].TranslatedText;
            }
            catch(Exception ex) { return null; }
        }
    }
}
