using LanguageResourceMaker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LanguageResourceMaker.Feature
{
    public partial class GenerateLanguageResourceForm : Form
    {
        private String inputFolder;
        public GenerateLanguageResourceForm(String inputFolder)
        {
            this.inputFolder = inputFolder;
            InitializeComponent();
        }

        private void GenerateLanguageResourceForm_Load(object sender, EventArgs e)
        {
            var currentLanguage = LanguageUtils.GetCurrentLanguage();
            //语言目录
            DirectoryInfo languageFolder = new DirectoryInfo(inputFolder);
            //取出所有的语言资源文件
            DirectoryInfo srcLanguageResourceFolder = new DirectoryInfo(Path.Combine(inputFolder, currentLanguage));
            var srcLanguageResourceFiles = srcLanguageResourceFolder.GetFiles("*.txt", SearchOption.AllDirectories);
            //取出当前语言的字典文件的全部条目
            var srcLanguageDictLines = File.ReadAllLines(Path.Combine(inputFolder, $"{currentLanguage}.dict.txt"));

            //开始生成
            var dictFiles = languageFolder.GetFiles("*.dict.txt").Where(t => !t.Name.StartsWith(currentLanguage + ".")).ToArray();
            for (var i = 0; i < dictFiles.Length; i++)
            {
                pbLevel2.Value = (i + 1) * 100 / dictFiles.Length;
                var dictFile = dictFiles[i];
                var desLanguage = dictFile.Name.Replace(".dict.txt", "");
                var desLanguageDictLines = File.ReadAllLines(dictFile.FullName);
                //语言替换对应字典
                Dictionary<String, String> languageDict = new Dictionary<string, string>();
                for (var x = 0; x < srcLanguageDictLines.Length; x++)
                {
                    languageDict[srcLanguageDictLines[x]] = desLanguageDictLines[x];
                }
                //生成语言资源文件
                for (var j = 0; j < srcLanguageResourceFiles.Length; j++)
                {
                    var srcLanguageResourceFile = srcLanguageResourceFiles[j];
                    var content = File.ReadAllText(srcLanguageResourceFile.FullName);
                    var contentDict = LanguageUtils.GetLanguageResourceDictionary(content);
                    foreach (var key in contentDict.Keys.ToArray())
                    {
                        var value = contentDict[key];
                        if (!languageDict.ContainsKey(value))
                            continue;
                        contentDict[key] = languageDict[value];
                    }
                    var desContent = LanguageUtils.GetToWriteLanguageText(contentDict);
                    var desLanguageResourceFile = Path.Combine(inputFolder, desLanguage, srcLanguageResourceFile.FullName.Substring(srcLanguageResourceFolder.FullName.Length + 1));
                    //如果目录不存在，则创建目录
                    var desLanguageResourceFileFolder = Path.GetDirectoryName(desLanguageResourceFile);
                    if (!Directory.Exists(desLanguageResourceFileFolder))
                        Directory.CreateDirectory(desLanguageResourceFileFolder);
                    //保存文件
                    File.WriteAllText(desLanguageResourceFile, desContent);
                    //更新进度条
                    pbLevel1.Value = (j + 1) * 100 / srcLanguageResourceFiles.Length;
                    Application.DoEvents();
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
