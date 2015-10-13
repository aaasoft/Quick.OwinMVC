namespace LanguageResourceMaker.Feature
{
    partial class ImportLanguageDictForm
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
            this.SuspendLayout();
            // 
            // ImportLanguageDictForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 101);
            this.Level1Title = "词条";
            this.Level2Title = "字典文件";
            this.Name = "ImportLanguageDictForm";
            this.Text = "正在导入语言字典...";
            this.Title = "正在导入语言字典...";
            this.Load += new System.EventHandler(this.ImportLanguageDictForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}