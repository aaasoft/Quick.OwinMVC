using Quick.OwinMVC.Startup.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Threading;
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
            this.Text = ProgramUtils.GetProgramTitle();
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
            //托盘图标
            niMain.Text = this.Text;
            niMain.Icon = this.Icon;

            ensureOnlyOne();
        }

        private NamedPipeServerStream createNewNamedPipedServerStream(String pipeName)
        {
            return new NamedPipeServerStream(
                    pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);
        }

        private void ensureOnlyOne()
        {
            var pipeName = string.Format("{0}.{1}", this.GetType().FullName, winServiceInstaller.ServiceName);
            try
            {
                var serverStream = createNewNamedPipedServerStream(pipeName);
                AsyncCallback ac = null;
                ac = ar =>
                {
                    showForm();
                    serverStream.Close();
                    serverStream = createNewNamedPipedServerStream(pipeName);
                    serverStream.BeginWaitForConnection(ac, null);
                };
                serverStream.BeginWaitForConnection(ac, null);
            }
            catch
            {
                try
                {
                    var clientStream = new NamedPipeClientStream(pipeName);
                    clientStream.Connect();
                    clientStream.Close();
                }
                finally
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    Environment.Exit(0);
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

        private void showForm()
        {
            this.Invoke(new Action(() =>
            {
                if (this.CanFocus)
                {
                    if (this.Location == HideLocation)
                        this.Location = BeforeHideLocation;
                    this.Activate();
                }
                else
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.Visible && form.CanFocus)
                            form.Activate();
                    }
                }
            }));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if(Entrance.Parameter.GetControlConfigFunc==null)
                return;
            var controlConfig = Entrance.Parameter.GetControlConfigFunc.Invoke();
            if (controlConfig == null
                || controlConfig.Length == 0)
                return;

            currentToolStripItemCollection = cmsMain.Items;
            foreach (var keyValuePair in controlConfig)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;
                if (value == null)
                {
                    handleGroup(key);
                }
                else
                {
                    Control control = null;
                    if (value is string)
                    {
                        control = new Label() { Text = (string)value };
                    }
                    else if (value is Action)
                    {
                        var action = (Action)value;
                        var btn = new Button() { Text = key };
                        btn.Click += (sender2, e2) =>
                        {
                            disableForm();
                            action();
                            enableForm();
                        };
                        control = btn;
                        addNotifyIconButton(btn);
                    }
                    else if (value is Button)
                    {
                        Button sourceButton = (Button)value;
                        var btn = new Button() { Text = key };
                        btn.Click += (sender2, e2) =>
                        {
                            disableForm();
                            sourceButton.PerformClick();
                            enableForm();
                        };
                        sourceButton.EnabledChanged += (sender2, e2) =>
                        {
                            if (btn.InvokeRequired)
                                btn.Invoke(new Action(() => btn.Enabled = sourceButton.Enabled));
                            else
                                btn.Enabled = sourceButton.Enabled;
                        };
                        btn.Enabled = sourceButton.Enabled;
                        control = btn;
                        addNotifyIconButton(btn);
                    }
                    else if (value is Control)
                    {
                        flpTools.Controls.Add(new Label() { Text = key, Width = 56 });
                        control = (Control)value;
                    }
                    flpTools.Controls.Add(control);
                }
            }

            this.CenterToScreen();
        }

        //处理分组
        private void handleGroup(string groupName)
        {
            //回到最外层
            if (groupName == null)
            {
                currentToolStripItemCollection = cmsMain.Items;
                if (!Entrance.Parameter.SplitTopControl)
                    return;
            }
            else
            {
                flpTools.Controls.Add(new Label()
                {
                    Text = groupName,
                    Font = new System.Drawing.Font(Font.FontFamily, Font.Size, System.Drawing.FontStyle.Bold),
                    Margin = new Padding(0),
                    Width = flpTools.Width,
                    TextAlign = System.Drawing.ContentAlignment.BottomCenter
                });
                var dropDown = new ToolStripMenuItem();
                dropDown.Text = groupName;
                cmsMain.Items.Add(dropDown);
                currentToolStripItemCollection = dropDown.DropDownItems;
            }
            flpTools.Controls.Add(new GroupBox()
            {
                Width = flpTools.Width,
                Height = 0,
                Margin = new Padding(0),
            });
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


        private Point HideLocation = new Point(-1000, -1000);
        private Point BeforeHideLocation;

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing
                && this.DialogResult == DialogResult.None)
            {
                e.Cancel = true;
                BeforeHideLocation = this.Location;
                this.Location = HideLocation;
                niMain.ShowBalloonTip(5000, this.Text, "已经最小化到托盘图标，单击此图标可以显示面板窗体。", ToolTipIcon.Info);
                return;
            }
            niMain.Visible = false;
        }

        private void niMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                showForm();
        }
    }
}
