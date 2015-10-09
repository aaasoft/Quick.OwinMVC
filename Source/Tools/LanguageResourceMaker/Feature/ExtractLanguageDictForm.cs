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
        public ExtractLanguageDictForm()
        {
            InitializeComponent();
#if DEBUG
            txtInputFolder.Text = Path.Combine(DebugUtils.GetSourceCodeFolder(), "Language");
#endif
        }

        private void btnSelectInput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            fbd.Description = "选择语言目录...";
            var ret = fbd.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
                return;
            txtInputFolder.Text = fbd.SelectedPath;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var languageDi = new DirectoryInfo(txtInputFolder.Text.Trim());
            foreach (var subLanguageDi in languageDi.GetDirectories())
            {
                HashSet<String> languageHashSet = new HashSet<string>();
                foreach (var languageFile in subLanguageDi.GetFiles("*.txt", SearchOption.AllDirectories))
                {
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
                }
                var array = languageHashSet.OrderBy(t => t).ToArray();
                if (array.Length == 0)
                    continue;
                File.WriteAllLines(Path.Combine(languageDi.FullName, subLanguageDi.Name + ".dict"), array);
            }
            MessageBox.Show("提取完成");
        }
    }
}
