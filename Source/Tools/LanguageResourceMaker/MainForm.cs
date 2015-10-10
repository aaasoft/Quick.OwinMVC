using LanguageResourceMaker.Feature;
using LanguageResourceMaker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            cbInputMode.SelectedIndex = 0;
#if DEBUG
            cbInputMode.SelectedIndex = 1;
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

        public String InputFolder
        {
            get
            {
                if (cbInputMode.SelectedIndex == 0)
                    return Application.StartupPath;
                return txtInputFolder.Text.Trim();
            }
        }

        public String LanguageFolder { get { return Path.Combine(InputFolder, "Language"); } }

        private void btnExtractLanguageResource_Click(object sender, EventArgs e)
        {
            new ExtractLanguageResourceForm(InputFolder).ShowDialog();
        }

        private void btnExtractLanguageDict_Click(object sender, EventArgs e)
        {
            new ExtractLanguageDictForm(LanguageFolder).ShowDialog();
        }

        private void btnAutoTranslateLanguageDict_Click(object sender, EventArgs e)
        {
            new AutoTranslateLanguageDictForm(LanguageFolder).ShowDialog();
        }

        private void cbInputMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlInputFolder.Visible = cbInputMode.SelectedIndex == 1;
        }

        private void lblShowInputFolder_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(InputFolder))
            {
                MessageBox.Show("输入目录不存在！");
                return;
            }
            Process.Start(InputFolder);
        }

        private void lblShowLanguageFolder_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(LanguageFolder))
            {
                MessageBox.Show("语言资源目录不存在！");
                return;
            }
            Process.Start(LanguageFolder);
        }

        private void lblShowLanguageDict_Click(object sender, EventArgs e)
        {
            var filePath = Path.Combine(LanguageFolder, "zh-CN.dict.txt");
            if (!File.Exists(filePath))
            {
                MessageBox.Show("语言字典文件不存在！");
                return;
            }
            Process.Start(filePath);
        }
    }
}
