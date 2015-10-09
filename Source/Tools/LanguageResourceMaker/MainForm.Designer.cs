namespace LanguageResourceMaker
{
    partial class MainForm
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
            this.btnExtractLanguageResource = new System.Windows.Forms.Button();
            this.btnExtractLanguageDict = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnExtractLanguageResource
            // 
            this.btnExtractLanguageResource.Location = new System.Drawing.Point(12, 12);
            this.btnExtractLanguageResource.Name = "btnExtractLanguageResource";
            this.btnExtractLanguageResource.Size = new System.Drawing.Size(172, 23);
            this.btnExtractLanguageResource.TabIndex = 0;
            this.btnExtractLanguageResource.Text = "提取语言资源";
            this.btnExtractLanguageResource.UseVisualStyleBackColor = true;
            this.btnExtractLanguageResource.Click += new System.EventHandler(this.btnExtractLanguageResource_Click);
            // 
            // btnExtractLanguageDict
            // 
            this.btnExtractLanguageDict.Location = new System.Drawing.Point(12, 41);
            this.btnExtractLanguageDict.Name = "btnExtractLanguageDict";
            this.btnExtractLanguageDict.Size = new System.Drawing.Size(172, 23);
            this.btnExtractLanguageDict.TabIndex = 1;
            this.btnExtractLanguageDict.Text = "提取语言字典";
            this.btnExtractLanguageDict.UseVisualStyleBackColor = true;
            this.btnExtractLanguageDict.Click += new System.EventHandler(this.btnExtractLanguageDict_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(196, 185);
            this.Controls.Add(this.btnExtractLanguageDict);
            this.Controls.Add(this.btnExtractLanguageResource);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "语言工作室";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExtractLanguageResource;
        private System.Windows.Forms.Button btnExtractLanguageDict;
    }
}