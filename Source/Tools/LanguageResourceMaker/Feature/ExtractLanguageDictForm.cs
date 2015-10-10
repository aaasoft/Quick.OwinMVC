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
    public partial class ExtractLanguageDictForm : Form
    {
        private string inputFolder;
        public ExtractLanguageDictForm(string inputFolder)
        {
            this.inputFolder = inputFolder;
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Application.DoEvents();

            var languageDi = new DirectoryInfo(inputFolder);
            var dirs = languageDi.GetDirectories();
            for (var i = 0; i < dirs.Length; i++)
            {
                pbLevel2.Value = (i + 1) * 100 / dirs.Length;

                var subLanguageDi = dirs[i];
                HashSet<String> languageHashSet = new HashSet<string>();
                var files = subLanguageDi.GetFiles("*.txt", SearchOption.AllDirectories);
                for (var j = 0; j < files.Length; j++)
                {
                    var languageFile = files[j];
                    foreach (var line in File.ReadAllLines(languageFile.FullName))
                    {
                        var index = line.IndexOf("=");
                        if (index < 0)
                            continue;
                        var value = line.Substring(index + 1);
                        if (languageHashSet.Contains(value))
                            continue;
                        languageHashSet.Add(value);
                    }
                    pbLevel1.Value = (j + 1) * 100 / files.Length;
                    Application.DoEvents();
                }
                var array = languageHashSet.OrderBy(t => t).ToArray();
                if (array.Length == 0)
                    continue;
                File.WriteAllLines(Path.Combine(languageDi.FullName, subLanguageDi.Name + ".dict.txt"), array);
            }
            MessageBox.Show("提取完成");
            this.Enabled = true;
        }
    }
}
