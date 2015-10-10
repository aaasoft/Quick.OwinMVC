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
            this.btnAutoTranslateLanguageDict = new System.Windows.Forms.Button();
            this.txtInputFolder = new System.Windows.Forms.TextBox();
            this.btnSelectInput = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExtractLanguageResource
            // 
            this.btnExtractLanguageResource.Location = new System.Drawing.Point(12, 78);
            this.btnExtractLanguageResource.Name = "btnExtractLanguageResource";
            this.btnExtractLanguageResource.Size = new System.Drawing.Size(172, 23);
            this.btnExtractLanguageResource.TabIndex = 0;
            this.btnExtractLanguageResource.Text = "提取语言资源";
            this.btnExtractLanguageResource.UseVisualStyleBackColor = true;
            this.btnExtractLanguageResource.Click += new System.EventHandler(this.btnExtractLanguageResource_Click);
            // 
            // btnExtractLanguageDict
            // 
            this.btnExtractLanguageDict.Location = new System.Drawing.Point(12, 107);
            this.btnExtractLanguageDict.Name = "btnExtractLanguageDict";
            this.btnExtractLanguageDict.Size = new System.Drawing.Size(172, 23);
            this.btnExtractLanguageDict.TabIndex = 1;
            this.btnExtractLanguageDict.Text = "提取语言字典";
            this.btnExtractLanguageDict.UseVisualStyleBackColor = true;
            this.btnExtractLanguageDict.Click += new System.EventHandler(this.btnExtractLanguageDict_Click);
            // 
            // btnAutoTranslateLanguageDict
            // 
            this.btnAutoTranslateLanguageDict.Location = new System.Drawing.Point(12, 136);
            this.btnAutoTranslateLanguageDict.Name = "btnAutoTranslateLanguageDict";
            this.btnAutoTranslateLanguageDict.Size = new System.Drawing.Size(172, 23);
            this.btnAutoTranslateLanguageDict.TabIndex = 2;
            this.btnAutoTranslateLanguageDict.Text = "机翻语言字典";
            this.btnAutoTranslateLanguageDict.UseVisualStyleBackColor = true;
            this.btnAutoTranslateLanguageDict.Click += new System.EventHandler(this.btnAutoTranslateLanguageDict_Click);
            // 
            // txtInputFolder
            // 
            this.txtInputFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputFolder.Location = new System.Drawing.Point(12, 24);
            this.txtInputFolder.Name = "txtInputFolder";
            this.txtInputFolder.Size = new System.Drawing.Size(125, 21);
            this.txtInputFolder.TabIndex = 3;
            // 
            // btnSelectInput
            // 
            this.btnSelectInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectInput.Location = new System.Drawing.Point(143, 22);
            this.btnSelectInput.Name = "btnSelectInput";
            this.btnSelectInput.Size = new System.Drawing.Size(41, 23);
            this.btnSelectInput.TabIndex = 4;
            this.btnSelectInput.Text = "...";
            this.btnSelectInput.UseVisualStyleBackColor = true;
            this.btnSelectInput.Click += new System.EventHandler(this.btnSelectInput_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "项目目录:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(196, 185);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtInputFolder);
            this.Controls.Add(this.btnSelectInput);
            this.Controls.Add(this.btnAutoTranslateLanguageDict);
            this.Controls.Add(this.btnExtractLanguageDict);
            this.Controls.Add(this.btnExtractLanguageResource);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "语言工作室";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExtractLanguageResource;
        private System.Windows.Forms.Button btnExtractLanguageDict;
        private System.Windows.Forms.Button btnAutoTranslateLanguageDict;
        private System.Windows.Forms.TextBox txtInputFolder;
        private System.Windows.Forms.Button btnSelectInput;
        private System.Windows.Forms.Label label1;
    }
}