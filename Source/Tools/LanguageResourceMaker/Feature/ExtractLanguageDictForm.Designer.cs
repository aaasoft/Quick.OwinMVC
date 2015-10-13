namespace LanguageResourceMaker.Feature
{
    partial class ExtractLanguageDictForm
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
            // ExtractLanguageDictForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 101);
            this.Level2Title = "目录";
            this.Name = "ExtractLanguageDictForm";
            this.Text = "正在提取语言字典...";
            this.Title = "正在提取语言字典...";
            this.Load += new System.EventHandler(this.ExtractLanguageDictForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }
}