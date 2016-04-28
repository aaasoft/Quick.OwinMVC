using Quick.OwinMVC.Program.Static;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Quick.OwinMVC.Program.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void tmrCheckServiceStatus_Tick(object sender, EventArgs e)
        {
            ServiceController service = WinServiceInstaller.getService();
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
        }

        private Process startSelfProcess(String arguments)
        {
            var fileName = Application.ExecutablePath;
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, arguments);
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                processStartInfo.Verb = "runas";
            }
            try
            {
                return Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "安装失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            disableForm();
            startSelfProcess("-install")?.WaitForExit();
            enableForm();
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            disableForm();
            startSelfProcess("-uninstall")?.WaitForExit();
            enableForm();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            disableForm();
            startSelfProcess("-start")?.WaitForExit();
            enableForm();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            disableForm();
            startSelfProcess("-stop")?.WaitForExit();
            enableForm();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            this.Hide();
            new SettingForm().ShowDialog();
            this.Show();
        }

        private void btnRunDebug_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
