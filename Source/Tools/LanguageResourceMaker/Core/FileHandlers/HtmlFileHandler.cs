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
        //["|>]\s*(?'value'[^>|^"]*?[\u4E00-\u9FA5]+[^>|^"]*?)\s*[<|"]
        private Regex regex = new Regex("[\"|>]\\s*(?'value'[^>|^\"]*?[\\u4E00-\\u9FA5]+[^>|^\"]*?)\\s*[<|\"]");
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
