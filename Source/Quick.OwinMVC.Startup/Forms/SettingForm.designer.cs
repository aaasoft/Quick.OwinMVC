namespace Quick.OwinMVC.Startup.Forms
{
    partial class SettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.gbWebEngine = new System.Windows.Forms.GroupBox();
            this.nudWebServer_Port = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cbWebServer_IPAddress = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCheck = new System.Windows.Forms.Button();
            this.gbWebEngine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWebServer_Port)).BeginInit();
            this.SuspendLayout();
            // 
            // gbWebEngine
            // 
            this.gbWebEngine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWebEngine.Controls.Add(this.btnCheck);
            this.gbWebEngine.Controls.Add(this.nudWebServer_Port);
            this.gbWebEngine.Controls.Add(this.label2);
            this.gbWebEngine.Controls.Add(this.cbWebServer_IPAddress);
            this.gbWebEngine.Controls.Add(this.label1);
            this.gbWebEngine.Location = new System.Drawing.Point(12, 12);
            this.gbWebEngine.Name = "gbWebEngine";
            this.gbWebEngine.Size = new System.Drawing.Size(191, 75);
            this.gbWebEngine.TabIndex = 0;
            this.gbWebEngine.TabStop = false;
            this.gbWebEngine.Text = "Web服务";
            // 
            // nudWebServer_Port
            // 
            this.nudWebServer_Port.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudWebServer_Port.Location = new System.Drawing.Point(66, 44);
            this.nudWebServer_Port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudWebServer_Port.Name = "nudWebServer_Port";
            this.nudWebServer_Port.Size = new System.Drawing.Size(62, 21);
            this.nudWebServer_Port.TabIndex = 3;
            this.nudWebServer_Port.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudWebServer_Port.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "端口：";
            // 
            // cbWebServer_IPAddress
            // 
            this.cbWebServer_IPAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbWebServer_IPAddress.FormattingEnabled = true;
            this.cbWebServer_IPAddress.Location = new System.Drawing.Point(66, 18);
            this.cbWebServer_IPAddress.Name = "cbWebServer_IPAddress";
            this.cbWebServer_IPAddress.Size = new System.Drawing.Size(119, 20);
            this.cbWebServer_IPAddress.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP地址：";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 93);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(93, 93);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnCheck
            // 
            this.btnCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheck.Location = new System.Drawing.Point(134, 43);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(51, 23);
            this.btnCheck.TabIndex = 4;
            this.btnCheck.Text = "检测";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 125);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbWebEngine);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置";
            this.Load += new System.EventHandler(this.SettingForm_Load);
            this.gbWebEngine.ResumeLayout(false);
            this.gbWebEngine.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWebServer_Port)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbWebEngine;
        private System.Windows.Forms.NumericUpDown nudWebServer_Port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbWebServer_IPAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnCheck;
    }
}