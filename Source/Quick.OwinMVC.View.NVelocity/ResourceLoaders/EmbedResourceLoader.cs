using NVelocity.Runtime.Resource.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons.Collections;
using NVelocity.Runtime.Resource;
using System.IO;
using System.Net;
using NVelocity.Exception;
using System.Text.RegularExpressions;
using Quick.OwinMVC.Localization;
using Quick.OwinMVC.Resource;

namespace Quick.OwinMVC.View.NVelocity.ResourceLoaders
{
    public class EmbedResourceLoader : ResourceLoader
    {
        //替换velocity的parse指令的语言的正则表达式
        //#parse\("(?'value'.*?)"\)
        private static Regex replaceParseRegex = new Regex(@"#parse\(""(?'value'.*?)""\)");
        /*
        替换多语言文本资源的正则表达式
        第一段："\s*(?'value'[^>|^<|^\n|^\r|^"]*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r|^"]*?)"
        第二段：'\s*(?'value'[^>|^<|^\n|^\r|^']*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r|^']*?)'
        第三段：>[^\u4E00-\u9FA5]*(?'value'[^>|^<|^\n|^\r]*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r]*?)<
        第四段：^[\s|^#]*(?'value'[^>|^<|^\n|^\r|^#|^"|^'|^/]*?[\u4E00-\u9FA5]+?.*?)$
        */
        private static Regex replaceTextRegex = new Regex(
            @"""\s*(?'value'[^>|^<|^\n|^\r|^""]*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r|^""]*?)""|'\s*(?'value'[^>|^<|^\n|^\r|^']*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r|^']*?)'|>[^\u4E00-\u9FA5]*(?'value'[^>|^<|^\n|^\r]*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r]*?)<|^[\s|^#]*(?'value'[^>|^<|^\n|^\r|^#|^""|^'|^/]*?[\u4E00-\u9FA5]+?.*?)$", RegexOptions.Multiline | RegexOptions.Compiled);
        

        private String pluginNameAndPathSplitString;
        private String viewNameSuffix;
        private String viewNamePrefix;

        public override long GetLastModified(global::NVelocity.Runtime.Resource.Resource resource)
        {
            return resource.LastModified;
        }

        public override Stream GetResourceStream(string name)
        {
            if ("VM_global_library.vm".Equals(name))
                return null;

            String[] tmpArray = name.Split(new String[] { pluginNameAndPathSplitString }, StringSplitOptions.RemoveEmptyEntries);
            if (tmpArray.Length < 3)
            {
                throw new VelocityException("视图名称[" + name + "]不符合规则：“[插件名]"
                        + pluginNameAndPathSplitString + "[路径]”");
            }
            String pluginName = tmpArray[0];
            String path = tmpArray[1];
            String language = tmpArray[2];

            var resourceResponse = WebRequest.Create($"resource://{pluginName}/{viewNamePrefix}{path}{viewNameSuffix}").GetResponse() as ResourceWebResponse;
            if (resourceResponse == null)
                return null;
            Stream stream = null;
            using (StreamReader reader = new StreamReader(resourceResponse.GetResponseStream()))
            {
                var content = reader.ReadToEnd();

                //为parse指令增加语言参数
                content = replaceParseRegex.Replace(content, match =>
                    {
                        var srcFullText = match.Value;
                        var valueGroup = match.Groups["value"];
                        var value = valueGroup.Value;

                        String newValue = null;
                        switch (value.Split(':').Length)
                        {
                            case 1:
                                newValue = $"{pluginName}:{value}:{language}";
                                break;
                            case 2:
                                newValue = $"{value}:{language}";
                                break;
                            case 3:
                                return srcFullText;
                        }
                        var strIndex = valueGroup.Index - match.Index;
                        StringBuilder sb = new StringBuilder(srcFullText);
                        sb.Remove(strIndex, valueGroup.Length);
                        sb.Insert(strIndex, newValue);
                        return sb.ToString();
                    });
                //替换多语言文本
                var textManager = TextManager.GetInstance(language);
                var assembly = resourceResponse.Assembly;
                int index = 0;
                content = replaceTextRegex.Replace(content, match =>
                 {
                     index++;
                     var srcFullText = match.Value;
                     var valueGroup = match.Groups["value"];
                     var newValue = textManager.GetText(index.ToString(), assembly, $"{path}{viewNameSuffix}");
                     if (newValue == null)
                         return srcFullText;
                     var strIndex = valueGroup.Index - match.Index;
                     StringBuilder sb = new StringBuilder(srcFullText);
                     sb.Remove(strIndex, valueGroup.Length);
                     sb.Insert(strIndex, newValue);
                     return sb.ToString();
                 });
                stream = new MemoryStream(reader.CurrentEncoding.GetBytes(content));
            }
            return stream;
        }

        public override void Init(ExtendedProperties configuration)
        {
            this.pluginNameAndPathSplitString = configuration.GetString("class.pluginNameAndPathSplitString");
            if (this.pluginNameAndPathSplitString == null)
                this.pluginNameAndPathSplitString = ":";
            this.viewNamePrefix = configuration.GetString("class.viewNamePrefix");
            if (this.viewNamePrefix == null)
                this.viewNamePrefix = "view/";
            this.viewNameSuffix = configuration.GetString("class.viewNameSuffix");
            if (this.viewNameSuffix == null)
                this.viewNameSuffix = ".html";
        }

        public override bool IsSourceModified(global::NVelocity.Runtime.Resource.Resource resource)
        {
            return resource.IsSourceModified();
        }
    }
}
