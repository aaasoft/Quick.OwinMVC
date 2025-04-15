namespace Quick.OwinMVC.Startup.Forms
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
            this.components = new System.ComponentModel.Container();
            this.flpTools = new System.Windows.Forms.FlowLayoutPanel();
            this.niMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // flpTools
            // 
            this.flpTools.AutoSize = true;
            this.flpTools.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpTools.Location = new System.Drawing.Point(10, 6);
            this.flpTools.MaximumSize = new System.Drawing.Size(300, 0);
            this.flpTools.Name = "flpTools";
            this.flpTools.Size = new System.Drawing.Size(0, 0);
            this.flpTools.TabIndex = 6;
            // 
            // niMain
            // 
            this.niMain.ContextMenuStrip = this.cmsMain;
            this.niMain.Visible = true;
            this.niMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.niMain_MouseClick);
            // 
            // cmsMain
            // 
            this.cmsMain.Name = "cmsMain";
            this.cmsMain.Size = new System.Drawing.Size(61, 4);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(52, 14);
            this.Controls.Add(this.flpTools);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel flpTools;
        private System.Windows.Forms.NotifyIcon niMain;
        private System.Windows.Forms.ContextMenuStrip cmsMain;
    }
}