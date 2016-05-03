using Quick.OwinMVC.Startup.Utils;
using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Quick.OwinMVC.Startup.Static
{
    public class WinServiceInstaller
    {
        public const String SERVICE_NAME = "DCIM_3.0";
        public const String DISPLAY_ERVICE_NAME = "数据中心智能管理系统 3.0";

        public static void Install()
        {
            DotNetServiceInstaller installer = new DotNetServiceInstaller();
            ServiceInstallInfo installInfo = new ServiceInstallInfo();
            // -service
            installInfo.ServiceFilePath = $"\"{typeof(WinServiceInstaller).Assembly.Location}\" -service";
            installInfo.ServiceName = SERVICE_NAME;
            installInfo.DisplayName = SERVICE_NAME;
            installInfo.Description = DISPLAY_ERVICE_NAME;
            installInfo.StartType = ServiceStartMode.Automatic;

            installInfo.IfDelayedAutoStart = true;
            installInfo.ServiceAccount.Account = ServiceAccount.LocalSystem;
            try
            {
                installer.Install(installInfo);
                MessageBox.Show("安装完成!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("对不起,安装失败!\r\n原因:{0}", ex.ToString()));
            }
        }

        public static void Uninstall()
        {
            DotNetServiceInstaller installer = new DotNetServiceInstaller();
            ServiceInstallInfo installInfo = new ServiceInstallInfo();

            installInfo.ServiceName = SERVICE_NAME;
            installInfo.DisplayName = SERVICE_NAME;
            installInfo.Description = DISPLAY_ERVICE_NAME;

            try
            {
                installer.UnInstall(installInfo);
                MessageBox.Show("卸载完成!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("对不起,卸载失败!\r\n原因:{0}", ex.ToString()));
            }
        }

        internal static ServiceController getService()
        {
            return getService(false);
        }
        internal static ServiceController getService(bool showMsg)
        {
            try { return ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == SERVICE_NAME); }
            catch
            {
                if (showMsg)
                    MessageBox.Show($"服务[{SERVICE_NAME}]不存在！");
                return null;
            }
        }

        public static void Start()
        {
            var service = getService(true);
            if (service == null)
                return;
            service.Start();
        }

        public static void Stop()
        {
            var service = getService(true);
            if (service == null)
                return;
            service.Stop();
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "ServiceInstallInfoList", IsNullable = false)]
    public partial class ServiceInstallInfoListModel
    {

        private ServiceInstallInfo[] serviceInstallInfoField = new ServiceInstallInfo[0];

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ServiceInstallInfo")]
        public ServiceInstallInfo[] ServiceInstallInfos
        {
            get
            {
                return this.serviceInstallInfoField;
            }
            set
            {
                this.serviceInstallInfoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ServiceInstallInfo
    {

        private String serviceNameField = String.Empty;

        private String displayNameField = String.Empty;

        private String descriptionField = String.Empty;

        private ServiceStartMode startTypeField = ServiceStartMode.Manual;

        private Boolean ifDelayedAutoStartField = false;

        private String[] servicesDependedOnField = new String[0];

        private String installLogFilePathField = String.Empty;

        private AccountInfo serviceAccountField = new AccountInfo();

        private Boolean ifStartAfterInstallField = false;

        private String serviceFilePathField = String.Empty;

        /// <remarks/>
        public String ServiceName
        {
            get
            {
                return this.serviceNameField;
            }
            set
            {
                this.serviceNameField = value;
            }
        }

        /// <remarks/>
        public String DisplayName
        {
            get
            {
                return String.IsNullOrEmpty(this.displayNameField) ? this.serviceNameField : this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        public String Description
        {
            get
            {
                return String.IsNullOrEmpty(this.descriptionField) ? this.serviceNameField : this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public ServiceStartMode StartType
        {
            get
            {
                return this.startTypeField;
            }
            set
            {
                this.startTypeField = value;
            }
        }

        /// <remarks/>
        public Boolean IfDelayedAutoStart
        {
            get
            {
                return this.ifDelayedAutoStartField;
            }
            set
            {
                this.ifDelayedAutoStartField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ServicesDependedOn")]
        public String[] ServicesDependedOn
        {
            get
            {
                return this.servicesDependedOnField;
            }
            set
            {
                this.servicesDependedOnField = value;
            }
        }

        /// <remarks/>
        public String InstallLogFilePath
        {
            get
            {
                return this.installLogFilePathField;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    this.installLogFilePathField = String.Empty;
                else
                    this.installLogFilePathField = Path.GetFullPath(value);
            }
        }

        /// <remarks/>
        public AccountInfo ServiceAccount
        {
            get
            {
                return this.serviceAccountField;
            }
            set
            {
                this.serviceAccountField = value;
            }
        }

        /// <remarks/>
        public Boolean IfStartAfterInstall
        {
            get
            {
                return this.ifStartAfterInstallField;
            }
            set
            {
                this.ifStartAfterInstallField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public String ServiceFilePath
        {
            get
            {
                return this.serviceFilePathField;
            }
            set
            {
                this.serviceFilePathField = value;
            }
        }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AccountInfo
    {

        private String userNameField = String.Empty;

        private String passwordField = String.Empty;

        private ServiceAccount valueField = ServiceAccount.LocalSystem;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public String UserName
        {
            get
            {
                return this.userNameField;
            }
            set
            {
                this.userNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public String Password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public ServiceAccount Account
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}
