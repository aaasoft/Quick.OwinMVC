using Quick.OwinMVC.Startup.Utils;
using System;
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

        private Process startSelfProcess(String arguments, bool needAdmin, bool hideConsole = true)
        {
            var fileName = Application.ExecutablePath;
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, arguments);
            if (hideConsole)
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (needAdmin && System.Environment.OSVersion.Version.Major >= 6)
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
            startSelfProcess("-install", true)?.WaitForExit();
            enableForm();
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            disableForm();
            startSelfProcess("-uninstall", true)?.WaitForExit();
            enableForm();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            disableForm();
            startSelfProcess("-start", true)?.WaitForExit();
            enableForm();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            disableForm();
            startSelfProcess("-stop", true)?.WaitForExit();
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
            disableForm();
            startSelfProcess("-debug", false, false);
            this.Close();
        }
    }
}
