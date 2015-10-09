using LanguageResourceMaker.Feature;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        private void btnExtractLanguageResource_Click(object sender, EventArgs e)
        {
            this.Hide();
            new ExtractLanguageResourceForm().ShowDialog();
            this.Show();
        }

        private void btnExtractLanguageDict_Click(object sender, EventArgs e)
        {
            this.Hide();
            new ExtractLanguageDictForm().ShowDialog();
            this.Show();
        }
    }
}
