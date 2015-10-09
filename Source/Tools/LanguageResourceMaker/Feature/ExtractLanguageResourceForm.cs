using LanguageResourceMaker.Core;
using LanguageResourceMaker.Translator;
using LanguageResourceMaker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LanguageResourceMaker.Feature
{
    public partial class ExtractLanguageResourceForm : Form
    {
        private ITranslator translator = null;

        public ExtractLanguageResourceForm()
        {
            InitializeComponent();
            translator = new BaiduTranslator();

            String currentLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            foreach (String language in translator.GetSupportLanguages())
            {
                //跳过中文
                if (language == currentLanguage)
                    continue;
                CultureInfo cultureInfo = new CultureInfo(language);
                var lvi = lvLanguages.Items.Add(cultureInfo.DisplayName);
                lvi.Tag = language;
            }
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

        private void btnSelectOutput_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "选择输出目录...";
            var ret = fbd.ShowDialog();
            if (ret == System.Windows.Forms.DialogResult.Cancel)
                return;
            txtOutputFolder.Text = fbd.SelectedPath;
        }

        private void cbAutoTranslate_CheckedChanged(object sender, EventArgs e)
        {
            lvLanguages.Enabled = cbAutoTranslate.Checked;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            MainEngineConfig config = new MainEngineConfig()
            {
                InputFolder = txtInputFolder.Text.Trim(),
                AutoTranslate = cbAutoTranslate.Checked,
                Translator = translator,
                PushLogAction = pushLog,
                UpdateLogAction = updateLog
            };
            switch (cbOutputMode.SelectedIndex)
            {
                case 0:
                    config.OutputFolder = config.InputFolder;
                    break;
                case 1:
                    config.OutputFolder = null;
                    break;
                case 2:
                    config.OutputFolder = txtOutputFolder.Text.Trim();
                    break;
            }
            if (config.AutoTranslate)
            {
                List<String> list = new List<string>();
                foreach (ListViewItem lvi in lvLanguages.CheckedItems)
                {
                    list.Add(lvi.Tag.ToString());
                }
                config.TranslateTarget = list.ToArray();
            }

            tabControl1.TabPages.Remove(tabPage1);
            MainEngine engine = new MainEngine(config);
            engine.Start();
        }

        private void pushLog(String msg)
        {
            this.BeginInvoke(new Action(() =>
                {
                    txtLog.AppendText(Environment.NewLine + msg);
                    txtLog.ScrollToCaret();
                }));
        }

        private void updateLog(String msg)
        {
            this.BeginInvoke(new Action(() =>
            {
                String[] lines = txtLog.Lines;
                lines[lines.Length - 1] = msg;
                txtLog.Lines = lines;
            }));
        }

        private void cbOutputMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlOutputFolder.Visible = cbOutputMode.SelectedIndex == 2;
        }

        private void ExtractLanguageResourceForm_Load(object sender, EventArgs e)
        {
            cbOutputMode.SelectedIndex = 0;
        }
    }
}
