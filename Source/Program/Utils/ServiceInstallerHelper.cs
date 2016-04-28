using Quick.OwinMVC.Localization;
using System;
using System.Collections;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;

namespace Quick.OwinMVC.Program.Utils
{
    #region 服务安装辅助类
    /// <summary>
    /// 服务安装辅助类
    /// </summary>
    public class ServiceInstallerHelper : Installer
    {
        [TextResource]
        public enum Texts
        {
            [Text("“stateSaver”不能为空引用.")]
            ServiceInstallerHelper_01
        }

        #region Field Define
        private ServiceInstaller _installer = new ServiceInstaller() { Context = new InstallContext() };
        private ServiceProcessInstaller _serviceProcessInstaller = new ServiceProcessInstaller();
        #endregion

        #region Properties Define

        #region 服务显示名称
        /// <summary>
        /// 服务显示名称
        /// </summary>
        public String DisplayName
        {
            get
            {
                return String.IsNullOrEmpty(this._installer.DisplayName) ? this._installer.ServiceName : this._installer.DisplayName;
            }
            set
            {
                this._installer.DisplayName = value;
            }
        }
        #endregion

        #region 服务描述
        /// <summary>
        /// 服务描述
        /// </summary>
        public String Description
        {
            get
            {
                return String.IsNullOrEmpty(this._installer.Description) ? this._installer.ServiceName : this._installer.Description;
            }
            set
            {
                this._installer.Description = value;
            }
        }
        #endregion

        #region 服务逻辑名称
        /// <summary>
        /// 服务逻辑名称
        /// </summary>
        public String ServiceName
        {
            get
            {
                return this._installer.ServiceName;
            }
            set
            {
                this._installer.ServiceName = value;
            }
        }
        #endregion

        #region 服务启动类型
        /// <summary>
        /// 服务启动类型
        /// </summary>
        public ServiceStartMode StartType
        {
            get
            {
                return this._installer.StartType;
            }
            set
            {
                this._installer.StartType = value;
            }
        }
        #endregion

        #region 是否延时自动加载
        /// <summary>
        /// 是否延时自动加载
        /// </summary>
        public Boolean IfDelayedAutoStart
        {
            get
            {
                return this._installer.DelayedAutoStart;
            }
            set
            {
                this._installer.DelayedAutoStart = value;
            }
        }
        #endregion

        #region 服务依耐项
        /// <summary>
        /// 服务依耐项
        /// </summary>
        public String[] ServicesDependedOn
        {
            get
            {
                return this._installer.ServicesDependedOn;
            }
            set
            {
                this._installer.ServicesDependedOn = value;
            }
        }
        #endregion

        #region 服务物理文件位置
        /// <summary>
        /// 服务物理文件位置
        /// </summary>
        public String ServiceFilePath
        {
            get
            {
                if (this._installer.Context == null)
                    this._installer.Context = new InstallContext();
                if (!this._installer.Context.Parameters.ContainsKey("assemblypath"))
                    this._installer.Context.Parameters["assemblypath"] = String.Empty;
                return this._installer.Context.Parameters["assemblypath"];
            }
            set
            {
                if (this._installer.Context == null)
                    this._installer.Context = new InstallContext();
                this._installer.Context.Parameters["assemblypath"] = value;
            }
        }
        #endregion

        #region 服务安装日志记录位置
        /// <summary>
        /// 服务安装日志记录位置
        /// </summary>
        public String InstallLogFilePath
        {
            get
            {
                if (this._installer.Context == null)
                    this._installer.Context = new InstallContext();
                if (!this._installer.Context.Parameters.ContainsKey("logfile"))
                    return String.Empty;
                else
                    return this._installer.Context.Parameters["logfile"];
            }
            set
            {
                if (this._installer.Context == null)
                    this._installer.Context = new InstallContext();
                if (String.IsNullOrEmpty(value))
                {
                    this._installer.Context.Parameters.Remove("logfile");
                }
                else
                {
                    this._installer.Context.Parameters["logfile"] = Path.GetFullPath(value);
                }
            }
        }
        #endregion

        #region 服务启动帐户
        /// <summary>
        /// 服务启动帐户
        /// </summary>
        public ServiceAccount ServiceAccount
        {
            get
            {
                return this._serviceProcessInstaller.Account;
            }
            set
            {
                this._serviceProcessInstaller.Account = value;
            }
        }
        #endregion

        #region 当用本地用户帐户启动时,该帐户的用户名
        /// <summary>
        /// 当用本地用户帐户启动时,该帐户的用户名
        /// </summary>
        public String UserName
        {
            get
            {
                return this._serviceProcessInstaller.Username;
            }
            set
            {
                this._serviceProcessInstaller.Username = value;
            }
        }
        #endregion

        #region 当用本地用户帐户启动时,该帐户的密码
        /// <summary>
        /// 当用本地用户帐户启动时,该帐户的密码
        /// </summary>
        public String Password
        {
            get
            {
                return this._serviceProcessInstaller.Password;
            }
            set
            {
                this._serviceProcessInstaller.Password = value;
            }
        }
        #endregion

        #endregion

        #region Override Methods

        #region 安装服务
        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="stateSaver"></param>
        public override void Install(IDictionary stateSaver)
        {
            if (stateSaver == null)
                throw new ArgumentNullException("stateSaver"
                                               , TextManager.DefaultInstance.GetTextWithTail(Texts.ServiceInstallerHelper_01));
            this._serviceProcessInstaller.Install(stateSaver);
            this.Installers.Add(this._serviceProcessInstaller);
            this._installer.Parent = this;
            Hashtable hashTable = new Hashtable();
            try
            {
                this._installer.Install(hashTable);
            }
            catch
            {
                this._installer.Rollback(stateSaver);
                throw;
            }
        }
        #endregion

        #region 卸载服务
        /// <summary>
        /// 卸载服务
        /// </summary>
        /// <param name="savedState"></param>
        public override void Uninstall(IDictionary savedState)
        {
            this._installer.Uninstall(null);
        }
        #endregion

        #endregion
    }
    #endregion
}
