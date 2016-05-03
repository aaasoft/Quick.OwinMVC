using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Quick.OwinMVC.Startup.Forms
{
    public partial class SettingForm : Form
    {
        private const String AllIPAddress = "0.0.0.0";
        private const String WebServerUrlKey = "Quick.OwinMVC.Startup.Service.Impl.WebServerService.WebServerUri";

        private const String regexTemplate = @"^(?!#)\s*{0}\s*=\s*(?'value'.*?)\s*$";
        private Regex webServerRegex = new Regex(String.Format(regexTemplate, WebServerUrlKey), RegexOptions.Multiline);

        public SettingForm()
        {
            InitializeComponent();
        }

        private void SettingForm_Load(object sender, EventArgs e)
        {
            loadSetting();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            var ip = getIPAddress();
            var port = Convert.ToInt32(nudWebServer_Port.Value);
            var tcpListener = new TcpListener(IPAddress.Parse(ip), port);
            try
            {
                tcpListener.Start();
                MessageBox.Show("检测通过！", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("检测失败，原因：" + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                tcpListener.Stop();
                tcpListener = null;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            saveSetting();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private String getConfigContent(String configFileName)
        {
            if (!File.Exists(configFileName))
                return null;
            String configContent;
            try
            {
                configContent = File.ReadAllText(configFileName);
            }
            catch { return null; }
            return configContent;
        }

        private void loadSetting()
        {
            //初始化获取本机的IP地址列表
            cbWebServer_IPAddress.Items.Add("全部未分配");
            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses
                    .Where(t => !t.Address.IsIPv6LinkLocal && t.Address.ToString() != "::1"))
                {
                    cbWebServer_IPAddress.Items.Add(addr.Address.ToString());
                }
            }
            cbWebServer_IPAddress.SelectedIndex = 0;

            //加载配置文件
            var configContent = getConfigContent(Entrance.ConfigFilePath);
            if (configContent == null)
            {
                MessageBox.Show($"配置文件[{Entrance.ConfigFilePath}]不存在或者无法打开！");
                this.Close();
                return;
            }
            var match = webServerRegex.Match(configContent);
            if (match == null)
            {
                MessageBox.Show($"配置文件[{Entrance.ConfigFilePath}]中缺少配置项[{WebServerUrlKey}]！");
                this.Close();
                return;
            }
            var url = match.Groups["value"].Value;
            Uri uri;
            try { uri = new Uri(url); }
            catch
            {
                MessageBox.Show($"配置文件[{Entrance.ConfigFilePath}]中配置项[{WebServerUrlKey}]的值格式不正确！");
                this.Close();
                return;
            }
            if (uri.Host != AllIPAddress)
                cbWebServer_IPAddress.Text = uri.Host;
            nudWebServer_Port.Value = uri.Port;
        }

        private String getIPAddress()
        {
            if (cbWebServer_IPAddress.SelectedIndex == 0)
                return AllIPAddress;
            return cbWebServer_IPAddress.Text.Trim();
        }

        private void saveSetting()
        {
            var configContent = getConfigContent(Entrance.ConfigFilePath);
            if (configContent == null)
            {
                MessageBox.Show($"配置文件[{Entrance.ConfigFilePath}]不存在或者无法打开！");
                this.Close();
                return;
            }
            configContent = webServerRegex.Replace(configContent, match =>
             {
                 var group = match.Groups["value"];
                 return $"{WebServerUrlKey} = net://{getIPAddress()}:{nudWebServer_Port.Value}";
             });
            File.WriteAllText(Entrance.ConfigFilePath, configContent);
        }
    }
}
