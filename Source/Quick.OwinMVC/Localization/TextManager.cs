using Quick.OwinMVC.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Quick.OwinMVC.Localization
{
    public class TextManager
    {
        public static String LanguageFileExtension { get; set; } = ".txt";
        public static String LanguageFolder { get; set; } = Path.Combine(Path.GetDirectoryName(typeof(TextManager).Assembly.Location), "Language");
        public static String LanguagePathInAssembly { get; set; } = "Language";

        private static Dictionary<String, TextManager> textManagerDict = new Dictionary<string, TextManager>();

        /// <summary>
        /// 获取默认的文本管理器实例(语言为配置项Quick.OwinMVC.Server.Language的值)
        /// </summary>
        public static TextManager Instance { get { return GetInstance(Server.Instance.Language); } }

        /// <summary>
        /// 获取文本管理器实例
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static TextManager GetInstance(String language)
        {
            lock (textManagerDict)
            {
                if (textManagerDict.ContainsKey(language))
                    return textManagerDict[language];
                var newTextManager = new TextManager(language);
                textManagerDict[language] = newTextManager;
                return newTextManager;
            }
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

        //类的语言资源字典
        private Dictionary<String, Dictionary<String, String>> typeLanguageResourceDict = new Dictionary<String, Dictionary<String, string>>();
        private String language;

        private TextManager(String language)
        {
            this.language = language;
        }

        /// <summary>
        /// 获取语言文字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String GetText(Enum key)
        {
            return GetText(key.ToString(), key.GetType());
        }

        /// <summary>
        /// 获取语言文字(带标记)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String GetTextWithTag(Enum key)
        {
            return $"{GetText(key)}(LanguageKey: {key.GetType().FullName}.{key.ToString()})";
        }

        public String GetText(String key, Type type)
        {
            Dictionary<String, String> languageResourceDict = getLanguageResourceDict(type);
            if (languageResourceDict == null
                || !languageResourceDict.ContainsKey(key))
                return String.Format("Language Resource[Type:{0}, Key:{1}] not found!", type.FullName, key);
            return languageResourceDict[key];
        }

        private Dictionary<String, String> getLanguageResourceDict(Type type)
        {
            String key = String.Format("{0};{1}", type.Assembly.GetName().Name, type.FullName);
            lock (typeLanguageResourceDict)
            {
                if (typeLanguageResourceDict.ContainsKey(key))
                    return typeLanguageResourceDict[key];

                Dictionary<String, String> languageResourceDict = new Dictionary<String, string>();
                typeLanguageResourceDict.Add(key, languageResourceDict);

                if (type.IsEnum && type.GetCustomAttributes(typeof(TextResourceAttribute), false).Length > 0)
                {
                    foreach (Enum item in Enum.GetValues(type))
                    {
                        MemberInfo itemMemberInfo = type.GetMember(item.ToString())[0];
                        var textAttributes = itemMemberInfo.GetCustomAttributes(typeof(TextAttribute), false).Cast<TextAttribute>();
                        TextAttribute textAttribute = textAttributes.Where(attr => attr.Language == language).FirstOrDefault();
                        if (textAttribute == null)
                            textAttribute = textAttributes.Where(attr => attr.Language == TextAttribute.DEFAULT_LANGUAGE).FirstOrDefault();
                        if (textAttribute == null)
                            textAttribute = textAttributes.FirstOrDefault();
                        if (textAttribute == null)
                            continue;
                        languageResourceDict.Add(item.ToString(), textAttribute.Value);
                    }
                }
                fillLanguageResourceDict(type.Assembly, type.FullName, languageResourceDict);
                return languageResourceDict;
            }
        }

        private Dictionary<String, String> getLanguageResourceDict(Assembly assembly, String resourcePath, params TextAttribute[] textAttributes)
        {
            String key = String.Format("{0};{1}", assembly.GetName().Name, resourcePath);
            lock (typeLanguageResourceDict)
            {
                if (typeLanguageResourceDict.ContainsKey(key))
                    return typeLanguageResourceDict[key];
                Dictionary<String, String> languageResourceDict = new Dictionary<String, string>();
                typeLanguageResourceDict.Add(key, languageResourceDict);
                fillLanguageResourceDict(assembly, resourcePath, languageResourceDict);
                return languageResourceDict;
            }
        }

        private void fillLanguageResourceDict(Assembly assembly, String resourcePath, Dictionary<String, String> languageResourceDict)
        {
            //==========================
            //然后尝试从资源文件中读取
            //==========================
            //视图模型接口类所在的程序集名称
            String assemblyName = assembly.GetName().Name;

            //==========================
            //最后搜索语言目录和主题目录下的文件
            //==========================
            //要搜索的可能的语言文件名称
            List<String> languageFileNameList = new List<string>();

            if (resourcePath.StartsWith(assemblyName + "."))
            {
                String shortName = resourcePath.Substring((assemblyName + ".").Length);
                languageFileNameList.Add(shortName + LanguageFileExtension);
            }
            languageFileNameList.Add(resourcePath + LanguageFileExtension);

            //语言目录下的语言文件内容
            String languageBaseFolder = Path.Combine(LanguageFolder, this.language, assemblyName);
            String languageContent = ResourceUtils.GetResourceText(
                    languageFileNameList,
                    assembly,
                    languageBaseFolder,
                    //路径
                    assemblyName, LanguagePathInAssembly, this.language, "[fileName]"
                );
            if (languageContent != null)
            {
                var tmpDict = GetLanguageResourceDictionary(languageContent);
                foreach (String key in tmpDict.Keys)
                {
                    if (languageResourceDict.ContainsKey(key))
                        languageResourceDict.Remove(key);
                    languageResourceDict.Add(key, tmpDict[key]);
                }
            }

            /*
            //主题目录下的语言文件内容
            String viewBaseFolder = Path.Combine(ThemeFolder, this.CurrentTheme, assemblyName);
            languageContent = Quick.MVVM.Utils.ResourceUtils.GetResourceText(
                    languageFileNameList,
                    assembly,
                    Path.Combine(viewBaseFolder, this.Config.LanguagePathInAssembly, this.language),
                    //路径
                    assemblyName, this.Config.LanguagePathInAssembly, this.language, "[fileName]"
                );
            if (languageContent != null)
            {
                var tmpDict = Quick.MVVM.Utils.ResourceUtils.GetLanguageResourceDictionary(languageContent);
                foreach (String key in tmpDict.Keys)
                {
                    if (languageResourceDict.ContainsKey(key))
                        languageResourceDict.Remove(key);
                    languageResourceDict.Add(key, tmpDict[key]);
                }
            }
            */
        }
    }
}
