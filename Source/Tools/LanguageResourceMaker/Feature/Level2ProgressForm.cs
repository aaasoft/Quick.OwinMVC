using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LanguageResourceMaker.Feature
{
    public partial class Level2ProgressForm : Form
    {
        private Int32 _Level1Index;
        private Int32 _Level1Count;
        private Int32 _Level2Index;
        private Int32 _Level2Count;

        public Int32 Level1Index
        {
            get { return _Level1Index; }
            set
            {
                _Level1Index = value;
                onLevel1Changed();
            }
        }

        public Int32 Level1Count
        {
            get { return _Level1Count; }
            set
            {
                _Level1Count = value;
                pbLevel1.Maximum = value;
            }
        }
        public string Level1Title
        {
            get { return lblLevel1Title.Text; }
            set { lblLevel1Title.Text = value; }
        }


        public Int32 Level2Index
        {
            get { return _Level2Index; }
            set
            {
                _Level2Index = value;
                onLevel2Changed();
            }
        }
        public Int32 Level2Count
        {
            get { return _Level2Count; }
            set
            {
                _Level2Count = value;
                pbLevel2.Maximum = value;
            }
        }
        public string Level2Title
        {
            get { return lblLevel2Title.Text; }
            set { lblLevel2Title.Text = value; }
        }

        public String Title
        {
            get { return this.Text; }
            set
            {
                this.Text = value;
                lblTitle.Text = value;
            }
        }


        public Level2ProgressForm()
        {
            InitializeComponent();
        }

        private void onLevel1Changed()
        {
            lblLevel1.Text = $"{Level1Index + 1}/{Level1Count}";
            pbLevel1.Value = Level1Index + 1;
        }

        private void onLevel2Changed()
        {
            lblLevel2.Text = $"{Level2Index + 1}/{Level2Count}";
            pbLevel2.Value = Level2Index + 1;
        }
    }
}
