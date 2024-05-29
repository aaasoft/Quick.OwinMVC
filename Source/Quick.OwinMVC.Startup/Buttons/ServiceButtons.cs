using Quick.OwinMVC.Startup.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quick.OwinMVC.Startup.Buttons
{
    public class ServiceButtons
    {
        private static ServiceButtons _Instance;
        public static ServiceButtons Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ServiceButtons();
                return _Instance;
            }
        }

        private WinServiceInstaller winServiceInstaller = null;


        private Label _StatusLabel;
        public object StatusLabel { get { return _StatusLabel; } }

        private Button _InstallButton;
        public object InstallButton { get { return _InstallButton; } }

        private Button _UninstallButton;
        public object UninstallButton { get { return _UninstallButton; } }

        private Button _StartButton;
        public object StartButton { get { return _StartButton; } }

        private Button _StopButton;
        public object StopButton { get { return _StopButton; } }

        public ServiceButtons()
        {
            winServiceInstaller = new WinServiceInstaller();

            _StatusLabel = new Label() { ForeColor = Color.Red };
            _InstallButton = new Button() { Size = new Size(75, 23) };
            _InstallButton.Click += InstallButton_Click;
            _UninstallButton = new Button() { Size = new Size(75, 23) };
            _UninstallButton.Click += UninstallButton_Click;
            _StartButton = new Button() { Size = new Size(75, 23) };
            _StartButton.Click += StartButton_Click;
            _StopButton = new Button() { Size = new Size(75, 23) };
            _StopButton.Click += StopButton_Click;

            checkServiceStatus();
        }

        private void InstallButton_Click(object sender, EventArgs e)
        {
            try { ProgramUtils.StartSelfProcess("-install", true).WaitForExit(); }
            catch { MessageBox.Show("安装服务失败!"); }
        }

        private void UninstallButton_Click(object sender, EventArgs e)
        {
            try { ProgramUtils.StartSelfProcess("-uninstall", true).WaitForExit(); }
            catch { MessageBox.Show("卸载服务失败!"); }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            try { ProgramUtils.StartSelfProcess("-start", true).WaitForExit(); }
            catch { MessageBox.Show("启动服务失败!"); }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            try { ProgramUtils.StartSelfProcess("-stop", true).WaitForExit(); }
            catch { MessageBox.Show("停止服务失败!"); }
        }

        private void checkServiceStatus()
        {
            Task.Run(() => doCheck()).ContinueWith(t1 =>
              {
                  Task.Delay(1000).ContinueWith(t2 => checkServiceStatus());
              });
        }

        private void doCheck()
        {
            ServiceController service = winServiceInstaller.GetService();
            if (service == null)
            {
                setButtonEnable(_InstallButton, true);
                setButtonEnable(_UninstallButton, false);
                setButtonEnable(_StartButton, false);
                setButtonEnable(_StopButton, false);
                setLabelText(_StatusLabel, "未安装");
            }
            else
            {
                setLabelText(_StatusLabel, service.Status.ToString());
                switch (service.Status)
                {
                    case ServiceControllerStatus.Stopped:
                        setButtonEnable(_InstallButton, false);
                        setButtonEnable(_UninstallButton, true);
                        setButtonEnable(_StartButton, true);
                        setButtonEnable(_StopButton, false);
                        break;
                    case ServiceControllerStatus.Running:
                        setButtonEnable(_InstallButton, false);
                        setButtonEnable(_UninstallButton, false);
                        setButtonEnable(_StartButton, false);
                        setButtonEnable(_StopButton, true);
                        break;
                    default:
                        setButtonEnable(_InstallButton, false);
                        setButtonEnable(_UninstallButton, false);
                        setButtonEnable(_StartButton, false);
                        setButtonEnable(_StopButton, false);
                        break;
                }
            }
        }

        private void setButtonEnable(Button button, bool enable)
        {
            if (button.InvokeRequired)
                button.Invoke(new Action(() => button.Enabled = enable));
            else
                button.Enabled = enable;
        }

        private void setLabelText(Label label, String text)
        {
            if (label.InvokeRequired)
                label.Invoke(new Action(() => label.Text = text));
            else
                label.Text = text;
        }
    }
}
