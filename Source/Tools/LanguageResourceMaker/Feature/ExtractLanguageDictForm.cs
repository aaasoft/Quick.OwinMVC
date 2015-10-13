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
    public partial class ExtractLanguageDictForm : Level2ProgressForm
    {
        private string inputFolder;
        public ExtractLanguageDictForm(string inputFolder)
        {
            this.inputFolder = inputFolder;
            InitializeComponent();
        }

        private void ExtractLanguageDictForm_Load(object sender, EventArgs e)
        {
            var languageDi = new DirectoryInfo(inputFolder);
            var dirs = languageDi.GetDirectories();
            base.Level2Count = dirs.Length;
            Application.DoEvents();
            for (var i = 0; i < dirs.Length; i++)
            {
                base.Level2Index = i;

                var subLanguageDi = dirs[i];
                HashSet<String> languageHashSet = new HashSet<string>();
                var files = subLanguageDi.GetFiles("*.txt", SearchOption.AllDirectories);

                base.Level1Index = 0;
                base.Level1Count = files.Length;
                Application.DoEvents();

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
                    base.Level1Index = j;
                    Application.DoEvents();
                }
                var array = languageHashSet.OrderBy(t => t).ToArray();
                if (array.Length == 0)
                    continue;
                File.WriteAllLines(Path.Combine(languageDi.FullName, subLanguageDi.Name + ".dict.txt"), array);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
