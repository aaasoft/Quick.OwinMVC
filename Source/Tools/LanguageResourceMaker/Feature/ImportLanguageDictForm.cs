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
    public partial class ImportLanguageDictForm : Form
    {
        private MainForm mainForm;


        public ImportLanguageDictForm(MainForm mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private Int32 getLangaugeDictLinesCount(String fileName)
        {
            return File.ReadAllLines(fileName)
                .Where(t => !String.IsNullOrWhiteSpace(t)).Count();
        }

        private void ImportLanguageDictForm_Load(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "语言字典文件(*.dict.txt)|*.dict.txt";
            ofd.Multiselect = true;
            var dr = ofd.ShowDialog();
            if (dr == DialogResult.Cancel)
            {
                this.Close();
                return;
            }

            var srcDictLinesCount = getLangaugeDictLinesCount(mainForm.LanguageDictFile);
            var fileNames = ofd.FileNames;
            ofd.Dispose();
            ofd = null;

            for (int i = 0; i < fileNames.Length; i++)
            {
                pbLevel1.Value = (i + 1) * 100 / fileNames.Length;
                Application.DoEvents();
                var fileName = fileNames[i];
                var currentDictLinesCount = getLangaugeDictLinesCount(fileName);
                if (currentDictLinesCount != srcDictLinesCount)
                {
                    MessageBox.Show($"要导入的语言字典文件[{fileName}]与源语言字典文件[{mainForm.LanguageDictFile}]不匹配，已忽略。", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }
                FileInfo fileInfo = new FileInfo(fileName);
                fileInfo.CopyTo(Path.Combine(mainForm.LanguageFolder, fileInfo.Name), true);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
