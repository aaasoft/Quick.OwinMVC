namespace LanguageResourceMaker.Feature
{
    partial class AutoTranslateLanguageDictForm
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
            this.lvLanguages = new System.Windows.Forms.ListView();
            this.chLanguage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnStart = new System.Windows.Forms.Button();
            this.pbLevel1 = new System.Windows.Forms.ProgressBar();
            this.pbLevel2 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // lvLanguages
            // 
            this.lvLanguages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLanguages.CheckBoxes = true;
            this.lvLanguages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chLanguage});
            this.lvLanguages.Location = new System.Drawing.Point(12, 12);
            this.lvLanguages.Name = "lvLanguages";
            this.lvLanguages.Size = new System.Drawing.Size(431, 364);
            this.lvLanguages.TabIndex = 8;
            this.lvLanguages.UseCompatibleStateImageBehavior = false;
            this.lvLanguages.View = System.Windows.Forms.View.Details;
            // 
            // chLanguage
            // 
            this.chLanguage.Text = "翻译为";
            this.chLanguage.Width = 400;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 382);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 9;
            this.btnStart.Text = "开始";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pbLevel1
            // 
            this.pbLevel1.Location = new System.Drawing.Point(93, 382);
            this.pbLevel1.Name = "pbLevel1";
            this.pbLevel1.Size = new System.Drawing.Size(153, 23);
            this.pbLevel1.TabIndex = 10;
            // 
            // pbLevel2
            // 
            this.pbLevel2.Location = new System.Drawing.Point(252, 382);
            this.pbLevel2.Name = "pbLevel2";
            this.pbLevel2.Size = new System.Drawing.Size(153, 23);
            this.pbLevel2.TabIndex = 10;
            // 
            // AutoTranslateLanguageDictForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 417);
            this.Controls.Add(this.pbLevel2);
            this.Controls.Add(this.pbLevel1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lvLanguages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AutoTranslateLanguageDictForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "机翻语言字典";
            this.Load += new System.EventHandler(this.AutoTranslateLanguageDictForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvLanguages;
        private System.Windows.Forms.ColumnHeader chLanguage;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ProgressBar pbLevel1;
        private System.Windows.Forms.ProgressBar pbLevel2;
    }
}