using LanguageResourceMaker.Translator;
using LanguageResourceMaker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LanguageResourceMaker.Feature
{
    public partial class AutoTranslateLanguageDictForm : Form
    {
        private string inputFolder;
        private ITranslator translator = null;

        public AutoTranslateLanguageDictForm(string inputFolder)
        {
            this.inputFolder = inputFolder;
            InitializeComponent();

            var translatorTypes = this.GetType().Assembly.GetTypes().Where(
                t => t.IsPublic
                && t.IsClass
                && typeof(ITranslator).IsAssignableFrom(t));

            cbTranslator.DisplayMember = nameof(ITranslator.Name);
            foreach (var translatorType in translatorTypes)
            {
                cbTranslator.Items.Add(Activator.CreateInstance(translatorType));
            }
            cbTranslator.SelectedIndex = 0;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void cbTranslator_SelectedIndexChanged(object sender, EventArgs e)
        {
            translator = (ITranslator)cbTranslator.SelectedItem;
            lvLanguages.Items.Clear();
            String currentLanguage = LanguageUtils.GetCurrentLanguage();
            foreach (String language in translator.GetSupportLanguages())
            {
                //跳过当前语言
                if (language == currentLanguage)
                    continue;
                CultureInfo cultureInfo = new CultureInfo(language);
                var lvi = lvLanguages.Items.Add(cultureInfo.DisplayName);
                lvi.Tag = language;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Application.DoEvents();

            String currentLanguage = LanguageUtils.GetCurrentLanguage();

            var languageDictFileFormat = Path.Combine(inputFolder, "{0}.dict.txt");

            var languageDictFile = String.Format(languageDictFileFormat, currentLanguage);
            if (!File.Exists(languageDictFile))
            {
                MessageBox.Show($"语言字典文件[{currentLanguage + ".dict.txt"}]不存在！");
                return;
            }
            String[] srcWords = File.ReadAllLines(languageDictFile);

            List<String> list = new List<string>();
            foreach (ListViewItem lvi in lvLanguages.CheckedItems)
            {
                list.Add(lvi.Tag.ToString());
            }
            var translateTarget = list.ToArray();

            Level2ProgressForm progressForm = new Level2ProgressForm();
            progressForm.Title = $"正在使用[{translator.Name}]翻译语言字典...";
            progressForm.Level1Title = "词条";
            progressForm.Level2Title = "目标语言";
            progressForm.Show();
            Application.DoEvents();
            this.Hide();

            progressForm.Level2Count = translateTarget.Length;
            for (int i = 0; i < translateTarget.Length; i++)
            {
                var toLangauge = translateTarget[i];
                var desWords = new String[srcWords.Length];
                progressForm.Level2Index = i;

                progressForm.Level1Index = 0;
                progressForm.Level1Count = srcWords.Length;
                Application.DoEvents();

                for (int j = 0; j < srcWords.Length; j++)
                {
                    var srcWord = srcWords[j];
                    var desWord = (String)null;
                    //最多重试三次
                    for (var retry = 0; retry < 3; retry++)
                    {
                        desWord = translator.Translate(currentLanguage, toLangauge, srcWord);
                        //如果翻译失败，则休息5秒
                        if (desWord == null)
                            Thread.Sleep(5 * 1000);
                        else
                            break;
                    }
                    if (desWord == null)
                    {
                        MessageBox.Show("重试3次，仍然翻译失败！请确保网络连接正常后再试。");
                        progressForm.Close();
                        this.Enabled = true;
                        this.Show();
                        return;
                    }
                    desWords[j] = desWord;
                    progressForm.Level1Index = j;
                    Application.DoEvents();
                }
                File.WriteAllLines(String.Format(languageDictFileFormat, toLangauge), desWords);
            }
            progressForm.Close();
            this.Enabled = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
