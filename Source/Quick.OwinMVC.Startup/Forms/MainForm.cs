using Quick.OwinMVC.Startup.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Quick.OwinMVC.Startup.Forms
{
    public partial class MainForm : Form
    {
        private WinServiceInstaller winServiceInstaller = null;

        public MainForm()
        {
            InitializeComponent();
            winServiceInstaller = new WinServiceInstaller();
            this.Text = winServiceInstaller.DisplayName;
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
            //托盘图标
            niMain.Text = this.Text;
            niMain.Icon = this.Icon;
        }

        private void tmrCheckServiceStatus_Tick(object sender, EventArgs e)
        {
            ServiceController service = winServiceInstaller.GetService();
            if (service == null)
            {
                btnInstall.Enabled = true;
                btnUninstall.Enabled = false;
                btnStart.Enabled = false;
                btnStop.Enabled = false;
                lblStatus.Text = "未安装";
            }
            else
            {
                lblStatus.Text = service.Status.ToString();
                switch (service.Status)
                {
                    case ServiceControllerStatus.Stopped:
                        btnInstall.Enabled = false;
                        btnUninstall.Enabled = true;
                        btnStart.Enabled = true;
                        btnStop.Enabled = false;
                        break;
                    case ServiceControllerStatus.Running:
                        btnInstall.Enabled = false;
                        btnUninstall.Enabled = false;
                        btnStart.Enabled = false;
                        btnStop.Enabled = true;
                        break;
                    default:
                        btnInstall.Enabled = false;
                        btnUninstall.Enabled = false;
                        btnStart.Enabled = false;
                        btnStop.Enabled = false;
                        break;
                }
            }
        }

        private void disableForm()
        {
            this.Enabled = false;
        }
        private void enableForm()
        {
            this.Enabled = true;
            this.Activate();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            disableForm();
            try { ProgramUtils.StartSelfProcess("-install", true)?.WaitForExit(); }
            catch { MessageBox.Show("安装服务失败!"); }
            enableForm();
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            disableForm();
            try { ProgramUtils.StartSelfProcess("-uninstall", true)?.WaitForExit(); }
            catch { MessageBox.Show("卸载服务失败!"); }
            enableForm();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            disableForm();
            try { ProgramUtils.StartSelfProcess("-start", true)?.WaitForExit(); }
            catch { MessageBox.Show("启动服务失败!"); }
            enableForm();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            disableForm();
            try { ProgramUtils.StartSelfProcess("-stop", true)?.WaitForExit(); }
            catch { MessageBox.Show("停止服务失败!"); }
            enableForm();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Entrance.Parameter.ButtonDict == null
                || Entrance.Parameter.ButtonDict.Count == 0)
                return;

            currentToolStripItemCollection = cmsMain.Items;

            foreach (var control in this.Controls)
            {
                if (control is Button)
                    addNotifyIconButton((Button)control);
            }
            foreach (var key in Entrance.Parameter.ButtonDict.Keys)
            {
                var value = Entrance.Parameter.ButtonDict[key];
                if (value == null)
                {
                    flpTools.Controls.Add(new Label()
                    {
                        Text = key,
                        Font = new System.Drawing.Font(Font.FontFamily, Font.Size, System.Drawing.FontStyle.Bold),
                        Margin = new Padding(0),
                        Width = flpTools.Width,
                        TextAlign = System.Drawing.ContentAlignment.BottomCenter
                    });
                    flpTools.Controls.Add(new GroupBox()
                    {
                        Width = flpTools.Width,
                        Height = 0,
                        Margin = new Padding(0),
                    });
                    var dropDown = new ToolStripMenuItem();
                    dropDown.Text = key;
                    cmsMain.Items.Add(dropDown);
                    currentToolStripItemCollection = dropDown.DropDownItems;
                }
                else
                {
                    var btn = new Button() { Text = key };
                    btn.Click += (sender2, e2) =>
                    {
                        disableForm();
                        value();
                        enableForm();
                    };
                    flpTools.Controls.Add(btn);
                    addNotifyIconButton(btn);
                }
            }
            //加入退出按钮
            currentToolStripItemCollection = cmsMain.Items;
            var exitButton = new Button() { Text = "退出" };
            exitButton.Click += (sender2, e2) =>
            {
                niMain.Visible = false;
                Environment.Exit(0);
            };
            addNotifyIconButton(exitButton);
        }

        private ToolStripItemCollection currentToolStripItemCollection;

        private void addNotifyIconButton(Button button)
        {
            var tsButton = new ToolStripMenuItem();
            tsButton.Text = button.Text;
            tsButton.Enabled = button.Enabled;
            button.EnabledChanged += (sender, e) =>
             {
                 tsButton.Enabled = button.Enabled;
             };
            tsButton.Click += (sender, e) =>
              {
                  button.PerformClick();
              };
            currentToolStripItemCollection.Add(tsButton);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void niMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.Activate();
            }
        }
    }
}
