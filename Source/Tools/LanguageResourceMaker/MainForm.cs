using LanguageResourceMaker.Feature;
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

namespace LanguageResourceMaker
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
#if DEBUG
            txtInputFolder.Text = DebugUtils.GetSourceCodeFolder();
#endif
        }
        private void btnSelectInput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = false;
            fbd.Description = "选择输入目录...";
            var ret = fbd.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
                return;
            txtInputFolder.Text = fbd.SelectedPath;
        }

        public String InputFolder { get { return txtInputFolder.Text.Trim(); } }
        public String LanguageFolder { get { return Path.Combine(InputFolder, "Language"); } }

        private void btnExtractLanguageResource_Click(object sender, EventArgs e)
        {
            this.Hide();
            new ExtractLanguageResourceForm(InputFolder).ShowDialog();
            this.Show();
        }

        private void btnExtractLanguageDict_Click(object sender, EventArgs e)
        {
            this.Hide();
            new ExtractLanguageDictForm(LanguageFolder).ShowDialog();
            this.Show();
        }

        private void btnAutoTranslateLanguageDict_Click(object sender, EventArgs e)
        {
            this.Hide();
            new AutoTranslateLanguageDictForm(LanguageFolder).ShowDialog();
            this.Show();
        }
    }
}
