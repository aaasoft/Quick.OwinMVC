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

        #region 设置输入目录
        private void cbInputMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlInputFolder.Visible = cbInputMode.SelectedIndex == 1;
            onInputFolderChanged();
        }

        private void txtInputFolder_TextChanged(object sender, EventArgs e)
        {
            onInputFolderChanged();
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
            onInputFolderChanged();
        }
        #endregion

        private void onInputFolderChanged()
        {
            btnExtractLanguageResource.Enabled = false;
            btnExtractLanguageDict.Enabled = false;
            btnAutoTranslateLanguageDict.Enabled = false;
            btnImportLanguageDict.Enabled = false;
            btnDeleteLanguageDict.Enabled = false;
            btnDeleteLanguageFolder.Enabled = false;
            btnGenerateLanguageResource.Enabled = false;

            if (!Directory.Exists(InputFolder))
                return;

            btnExtractLanguageResource.Enabled = true;
            if (!Directory.Exists(LanguageFolder))
                return;
            btnExtractLanguageDict.Enabled = true;
            btnDeleteLanguageFolder.Enabled = true;

            if (!File.Exists(LanguageDictFile))
                return;
            btnDeleteLanguageDict.Enabled = true;
            btnAutoTranslateLanguageDict.Enabled = true;
            btnImportLanguageDict.Enabled = true;
            btnGenerateLanguageResource.Enabled = true;
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
        public String LanguageDictFile { get { return Path.Combine(InputFolder, "Language", $"{LanguageUtils.GetCurrentLanguage()}.dict.txt"); } }

        #region 功能按钮区
        private void btnExtractLanguageResource_Click(object sender, EventArgs e)
        {
            var dr = new ExtractLanguageResourceForm(InputFolder).ShowDialog();
            if (dr == DialogResult.OK)
                MessageBox.Show("提取语言资源完成！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            onInputFolderChanged();
        }

        private void btnExtractLanguageDict_Click(object sender, EventArgs e)
        {
            var dr = new ExtractLanguageDictForm(LanguageFolder).ShowDialog();
            if (dr == DialogResult.OK)
                MessageBox.Show("提取语言字典完成！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            onInputFolderChanged();
        }

        private void btnAutoTranslateLanguageDict_Click(object sender, EventArgs e)
        {
            var dr = new AutoTranslateLanguageDictForm(LanguageFolder).ShowDialog();
            if (dr == DialogResult.OK)
                MessageBox.Show("机器翻译完成！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            onInputFolderChanged();
        }

        private void btnImportLanguageDict_Click(object sender, EventArgs e)
        {
            var dr = new ImportLanguageDictForm(this).ShowDialog();
            if (dr == DialogResult.OK)
                MessageBox.Show("导入翻译结果完成！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            onInputFolderChanged();
        }

        private void btnGenerateLanguageResource_Click(object sender, EventArgs e)
        {
            var dr = new GenerateLanguageResourceForm(LanguageFolder).ShowDialog();
            if (dr == DialogResult.OK)
                MessageBox.Show("生成语言资源完成！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            onInputFolderChanged();
        }

        private void btnDeleteLanguageDict_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(LanguageFolder);
            if (!di.Exists)
                return;

            var dr = MessageBox.Show("确定要删除所有的语言字典？", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Cancel)
                return;            
            foreach (var file in di.GetFiles("*.dict.txt"))
                file.Delete();
            onInputFolderChanged();
            MessageBox.Show("删除所有的语言字典完成！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDeleteLanguageFolder_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("确定要删除整个语言目录？", Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Cancel)
                return;
            Directory.Delete(LanguageFolder, true);
            onInputFolderChanged();
            MessageBox.Show("删除整个语言目录完成！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region 点击流程图
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
            if (!File.Exists(LanguageDictFile))
            {
                MessageBox.Show("语言字典文件不存在！");
                return;
            }
            Process.Start(LanguageDictFile);
        }
        #endregion
    }
}
