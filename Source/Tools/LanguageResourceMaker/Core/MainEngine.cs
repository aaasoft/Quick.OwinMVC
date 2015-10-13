using LanguageResourceMaker.Core.FileHandlers;
using LanguageResourceMaker.Translator;
using LanguageResourceMaker.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace LanguageResourceMaker.Core
{
    public class MainEngine
    {
        private MainEngineConfig config;
        private Dictionary<String, IFileHandler> fileHandlerDict = new Dictionary<string, IFileHandler>();

        public MainEngine(MainEngineConfig config)
        {
            this.config = config;
            fileHandlerDict.Add("*.html", new HtmlFileHandler(config) { OutputLanguageFileAction = outputLanguageFile });
            fileHandlerDict.Add("*.cs", new CsFileHandler() { OutputLanguageFileAction = outputLanguageFile });
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem(target =>
                {
                    _Start();
                });
        }

        private void outputLanguageFile(String outputFilePath, DirectoryInfo projectFolder, Dictionary<String, String> textDict, String language)
        {
            //输出到语言文件
            String abstractOutputFolder = null;
            if (String.IsNullOrEmpty(config.OutputFolder))
            {
                abstractOutputFolder = Path.Combine(projectFolder.FullName, "Language", "{0}");
            }
            else
            {
                abstractOutputFolder = Path.Combine(config.OutputFolder, "Language", "{0}", projectFolder.Name);
            }

            String abstractOutputFileName = Path.Combine(abstractOutputFolder, outputFilePath + ".txt");
            String languageFileName = String.Format(abstractOutputFileName, language);
            String dirPath = Path.GetDirectoryName(languageFileName);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            File.WriteAllText(languageFileName, LanguageUtils.GetToWriteLanguageText(textDict), Encoding.UTF8);
        }

        private void _Start()
        {
            config.PushLogAction("开始");

            DirectoryInfo di = new DirectoryInfo(config.InputFolder);
            var projectFiles = di.GetFiles("*.csproj", SearchOption.AllDirectories);

            config.PushLogAction("搜索中...");
            for (int i = 0; i < projectFiles.Length; i++)
            {
                var projectFile = projectFiles[i];
                DirectoryInfo projectFolder = projectFile.Directory;
                config.UpdateLogAction(String.Format("正在处理第[{0}/{1}]个项目[{2}]", i + 1, projectFiles.Length, projectFolder.Name));
                foreach (String searchPattern in fileHandlerDict.Keys)
                {
                    IFileHandler fileHandler = fileHandlerDict[searchPattern];
                    DirectoryInfo fileFolder = new DirectoryInfo(Path.Combine(projectFolder.FullName, fileHandler.GetFolderPath()));
                    if (!fileFolder.Exists)
                        continue;
                    foreach (var viewFile in fileFolder.GetFiles(searchPattern, SearchOption.AllDirectories))
                        fileHandler.Handle(viewFile, projectFolder);
                }
            }

            config.PushLogAction("处理完成");
            config.OnFinishAction();
        }
    }
}
