namespace LanguageResourceMaker.Feature
{
    partial class Level2ProgressForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblLevel2 = new System.Windows.Forms.Label();
            this.lblLevel1 = new System.Windows.Forms.Label();
            this.pbLevel1 = new System.Windows.Forms.ProgressBar();
            this.pbLevel2 = new System.Windows.Forms.ProgressBar();
            this.lblLevel1Title = new System.Windows.Forms.Label();
            this.lblLevel2Title = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblLevel2Title);
            this.panel1.Controls.Add(this.lblLevel1Title);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Controls.Add(this.lblLevel2);
            this.panel1.Controls.Add(this.lblLevel1);
            this.panel1.Controls.Add(this.pbLevel1);
            this.panel1.Controls.Add(this.pbLevel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(376, 101);
            this.panel1.TabIndex = 15;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.Location = new System.Drawing.Point(11, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(65, 12);
            this.lblTitle.TabIndex = 15;
            this.lblTitle.Text = "请等待...";
            // 
            // lblLevel2
            // 
            this.lblLevel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLevel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLevel2.Location = new System.Drawing.Point(270, 61);
            this.lblLevel2.Name = "lblLevel2";
            this.lblLevel2.Size = new System.Drawing.Size(89, 23);
            this.lblLevel2.TabIndex = 14;
            this.lblLevel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLevel1
            // 
            this.lblLevel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLevel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLevel1.Location = new System.Drawing.Point(270, 32);
            this.lblLevel1.Name = "lblLevel1";
            this.lblLevel1.Size = new System.Drawing.Size(89, 23);
            this.lblLevel1.TabIndex = 13;
            this.lblLevel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbLevel1
            // 
            this.pbLevel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLevel1.Location = new System.Drawing.Point(82, 32);
            this.pbLevel1.Name = "pbLevel1";
            this.pbLevel1.Size = new System.Drawing.Size(182, 23);
            this.pbLevel1.TabIndex = 12;
            // 
            // pbLevel2
            // 
            this.pbLevel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLevel2.Location = new System.Drawing.Point(82, 61);
            this.pbLevel2.Name = "pbLevel2";
            this.pbLevel2.Size = new System.Drawing.Size(182, 23);
            this.pbLevel2.TabIndex = 11;
            // 
            // lblLevel1Title
            // 
            this.lblLevel1Title.Location = new System.Drawing.Point(11, 32);
            this.lblLevel1Title.Name = "lblLevel1Title";
            this.lblLevel1Title.Size = new System.Drawing.Size(65, 23);
            this.lblLevel1Title.TabIndex = 16;
            this.lblLevel1Title.Text = "Level1标题";
            this.lblLevel1Title.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblLevel2Title
            // 
            this.lblLevel2Title.Location = new System.Drawing.Point(11, 61);
            this.lblLevel2Title.Name = "lblLevel2Title";
            this.lblLevel2Title.Size = new System.Drawing.Size(65, 23);
            this.lblLevel2Title.TabIndex = 17;
            this.lblLevel2Title.Text = "Level2标题";
            this.lblLevel2Title.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Level2ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 101);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Level2ProgressForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Level2ProgressForm";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ProgressBar pbLevel1;
        private System.Windows.Forms.ProgressBar pbLevel2;
        private System.Windows.Forms.Label lblLevel2;
        private System.Windows.Forms.Label lblLevel1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblLevel2Title;
        private System.Windows.Forms.Label lblLevel1Title;
    }
}