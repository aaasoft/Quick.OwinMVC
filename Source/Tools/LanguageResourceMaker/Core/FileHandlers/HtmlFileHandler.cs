using LanguageResourceMaker.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace LanguageResourceMaker.Core.FileHandlers
{
    public class HtmlFileHandler : AbstractFileHandler
    {
        /*
        第一段："\s*(?'value'[^>|^<|^\n|^\r|^"]*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r|^"]*?)"
        第二段：'\s*(?'value'[^>|^<|^\n|^\r|^']*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r|^']*?)'
        第三段：>[^\u4E00-\u9FA5]*(?'value'[^>|^<|^\n|^\r]*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r]*?)<
        第四段：^[\s|^#]*(?'value'[^>|^<|^\n|^\r|^#|^"|^'|^/]*?[\u4E00-\u9FA5]+?.*?)$
        */
        private Regex regex = new Regex(
            @"""\s*(?'value'[^>|^<|^\n|^\r|^""]*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r|^""]*?)""|'\s*(?'value'[^>|^<|^\n|^\r|^']*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r|^']*?)'|>[^\u4E00-\u9FA5]*(?'value'[^>|^<|^\n|^\r]*?[\u4E00-\u9FA5]+?[^>|^<|^\n|^\r]*?)<|^[\s|^#]*(?'value'[^>|^<|^\n|^\r|^#|^""|^'|^/]*?[\u4E00-\u9FA5]+?.*?)$");
        private MainEngineConfig config;

        public HtmlFileHandler(MainEngineConfig config)
        {
            this.config = config;
        }

        public override string GetFolderPath()
        {
            return "view";
        }

        public override void Handle(FileInfo viewFile, DirectoryInfo projectFolder)
        {
            String themeBaseFolder = Path.Combine(projectFolder.FullName, GetFolderPath());

            Dictionary<String, String> textDict = new Dictionary<string, string>();

            String xamlContent = File.ReadAllText(viewFile.FullName);

            Int32 index = 1;
            xamlContent = regex.Replace(xamlContent, match =>
            {
                var valueGroup = match.Groups["value"];
                if (!valueGroup.Success)
                    return match.Value;
                String value = valueGroup.Value;
                textDict.Add(index.ToString(), value);
                index++;
                return value;
            });
            if (textDict.Count == 0)
                return;

            String reFilePath = viewFile.FullName.Substring(themeBaseFolder.Length + 1);
            OutputLanguageFileAction(reFilePath, projectFolder, textDict, Thread.CurrentThread.CurrentCulture.Name);
        }
    }
}
