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
        private string inputFolder;

        public ExtractLanguageResourceForm(string inputFolder)
        {
            this.inputFolder = inputFolder;
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            MainEngineConfig config = new MainEngineConfig()
            {
                InputFolder = inputFolder,
                OutputFolder = inputFolder,
                AutoTranslate = false,
                PushLogAction = pushLog,
                UpdateLogAction = updateLog
            };
            MainEngine engine = new MainEngine(config);
            engine.Start();
        }

        private void pushLog(String msg)
        {
            this.BeginInvoke(new Action(() =>
                {
                    lblLog.Text = msg;
                }));
        }

        private void updateLog(String msg)
        {
            this.BeginInvoke(new Action(() =>
            {
                lblLog.Text = msg;
            }));
        }
    }
}
