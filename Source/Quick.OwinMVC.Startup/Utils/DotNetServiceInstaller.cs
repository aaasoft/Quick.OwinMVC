using Microsoft.Win32;
using Quick.OwinMVC.Localization;
using Quick.OwinMVC.Startup.Static;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace Quick.OwinMVC.Startup.Utils
{
    #region .NET 服务安装类
    /// <summary>
    /// .NET 服务安装类
    /// </summary>
    public class DotNetServiceInstaller
    {
        private static DotNetServiceInstaller _Instance = new DotNetServiceInstaller();
        public static DotNetServiceInstaller Instance { get { return _Instance; } }

        [TextResource]
        public enum Texts
        {
            [Text("服务[{0}]的启动类型信息已损坏.")]
            DotNetServiceInstaller_01,
            [Text("“installInfo”中包含服务安装信息不完整.")]
            DotNetServiceInstaller_02,
            [Text("“installInfo”中包含的服务文件不存在.")]
            DotNetServiceInstaller_03,
            [Text("“installInfo”中的程序集不包含服务基类，无法安装.")]
            DotNetServiceInstaller_04,
            [Text("“installInfos”中未包含服务安装所需信息.")]
            DotNetServiceInstaller_05,
            [Text("“serviceInfo”中未包含需要卸载的服务名称.")]
            DotNetServiceInstaller_06,
            [Text("“serviceInfos”中未包含服务卸载信息.")]
            DotNetServiceInstaller_07,
            [Text("“serviceName”服务名称不能为空.")]
            DotNetServiceInstaller_08,
            [Text("“serviceName”服务[{0}]不存在，无法卸载.")]
            DotNetServiceInstaller_09
        }

        #region 加载服务安装信息
        /// <summary>
        /// 加载服务安装信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public ServiceInstallInfoListModel LoadInstallInfosFromAssembly(Assembly assembly)
        {
            ServiceInstallInfoListModel retVal = new ServiceInstallInfoListModel();
            List<Module> moduleList = new List<Module>(assembly.GetModules());
            List<ServiceInstaller> serviceInstallerList;
            List<ServiceProcessInstaller> serviceProcessInstallerList;
            this.LoadInstallInfosFromModuleList(moduleList, out serviceInstallerList, out serviceProcessInstallerList);
            List<ServiceInstallInfo> installInfoList = this.GetInstallInfoList(assembly, serviceInstallerList, serviceProcessInstallerList);
            retVal.ServiceInstallInfos = installInfoList.ToArray();
            return retVal;
        }

        /// <summary>
        /// 加载服务安装信息
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <returns></returns>
        public ServiceInstallInfoListModel LoadInstallInfosFromAssembly(String assemblyFile)
        {
            ServiceInstallInfoListModel retVal;
            Assembly assembly = null;
            try
            {
                assembly = Assembly.LoadFrom(assemblyFile);
            }
            catch
            {
                throw;
            }
            retVal = LoadInstallInfosFromAssembly(assembly);
            return retVal;
        }
        #endregion

        #region 验证是否可以安装
        /// <summary>
        /// 验证是否可以安装
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public Boolean CheckAssemblyCanBeInstalled(Assembly assembly)
        {
            Boolean retVal = false;
            List<Module> moduleList = new List<Module>(assembly.GetModules());
            List<Type> typeList = new List<Type>();
            foreach (Module moduleItem in moduleList)
            {
                typeList.AddRange(moduleItem.GetTypes());
                foreach (Type typeItem in typeList)
                {
                    if (typeof(ServiceBase).IsAssignableFrom(typeItem)
                        && typeItem.IsPublic && !typeItem.IsAbstract)
                    {
                        retVal = true;
                        break;
                    }
                }
                if (retVal)
                    break;
                typeList.Clear();
            }
            return retVal;
        }

        /// <summary>
        /// 验证是否可以安装
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <returns></returns>
        public Boolean CheckAssemblyCanBeInstalled(String assemblyPath)
        {
            Boolean retVal = false;
            Assembly assembly = null;
            try
            {
                assembly = Assembly.LoadFrom(assemblyPath);
            }
            catch
            {
                return retVal;
            }
            retVal = CheckAssemblyCanBeInstalled(assembly);
            return retVal;
        }
        #endregion

        #region 根据名称检查服务状态
        /// <summary>
        /// 根据名称检查服务状态
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public Hashtable CheckServiceState(String serviceName)
        {
            Hashtable retVal = new Hashtable();
            retVal["IfExists"] = false;
            retVal["ServiceName"] = String.Empty;
            retVal["DisplayName"] = String.Empty;
            retVal["RunState"] = String.Empty;
            retVal["StartType"] = String.Empty;
            ServiceController svcController = new ServiceController(serviceName);
            try
            {
                retVal["RunState"] = svcController.Status.ToString();
                retVal["ServiceName"] = svcController.ServiceName;
                retVal["DisplayName"] = svcController.DisplayName;
                retVal["IfExists"] = true;
            }
            catch (InvalidOperationException ex)
            {
                Win32Exception innerException = ex.InnerException as Win32Exception;
                if (innerException != null
                    && innerException.ErrorCode == -2147467259
                    && innerException.NativeErrorCode == 1060)
                {
                    retVal["IfExists"] = false;
                    retVal["ServiceName"] = String.Empty;
                    retVal["DispalyName"] = String.Empty;
                    retVal["RunState"] = String.Empty;
                    retVal["StartType"] = String.Empty;
                    return retVal;
                }
                else
                    throw new Exception(ex.Message, ex);
            }
            RegistryKey regist = Registry.LocalMachine;
            RegistryKey sysReg = regist.OpenSubKey("SYSTEM");
            RegistryKey currentControlSet = sysReg.OpenSubKey("CurrentControlSet");
            RegistryKey services = currentControlSet.OpenSubKey("Services");
            RegistryKey servicesName = services.OpenSubKey(serviceName, true);
            if (servicesName != null)
            {
                Int32 startType = -1024;
                if (!Int32.TryParse(servicesName.GetValue("Start").ToString(), out startType))
                    startType = -1024;
                if (Enum.IsDefined(typeof(ServiceStartMode), startType))
                    retVal["StartType"] = (ServiceStartMode)startType;
                else
                    retVal["StartType"] = servicesName.GetValue("Start").ToString();
            }
            else
                retVal["StartType"] = TextManager.DefaultInstance.GetTextWithTail(Texts.DotNetServiceInstaller_01
                                                                       , false
                                                                       , serviceName);
            return retVal;
        }
        #endregion

        #region 服务安装
        /// <summary>
        /// 服务安装
        /// </summary>
        /// <param name="installInfo"></param>
        public void Install(ServiceInstallInfo installInfo)
        {
            if (installInfo == null
                || String.IsNullOrWhiteSpace(installInfo.ServiceName)
                || String.IsNullOrWhiteSpace(installInfo.ServiceFilePath))
                throw new ArgumentNullException("installInfo"
                                               , TextManager.DefaultInstance.GetTextWithTail(Texts.DotNetServiceInstaller_02));

            ServiceInstallerHelper installerHelp = new ServiceInstallerHelper();
            try
            {
                installerHelp.ServiceFilePath = installInfo.ServiceFilePath;
                installerHelp.ServiceName = installInfo.ServiceName;
                installerHelp.DisplayName = installInfo.DisplayName;
                installerHelp.Description = installInfo.Description;
                installerHelp.StartType = installInfo.StartType;
                installerHelp.IfDelayedAutoStart = installInfo.IfDelayedAutoStart;
                installerHelp.ServicesDependedOn = installInfo.ServicesDependedOn;
                installerHelp.InstallLogFilePath = installInfo.InstallLogFilePath;
                installerHelp.ServiceAccount = installInfo.ServiceAccount.Account;
                installerHelp.UserName = installInfo.ServiceAccount.UserName;
                installerHelp.Password = installInfo.ServiceAccount.Password;
                installerHelp.Install(new Hashtable());
                if (installInfo.IfStartAfterInstall)
                {
                    try
                    {
                        using (ServiceController controller = new ServiceController(installInfo.ServiceName))
                        {
                            controller.Start();
                            controller.Close();
                        }
                    }
                    catch { }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 服务安装
        /// </summary>
        /// <param name="serviceInfo"></param>
        public void Install(ServiceInstallInfoListModel installInfos)
        {
            if (installInfos == null
                || installInfos.ServiceInstallInfos == null
                || installInfos.ServiceInstallInfos.Length == 0)
                throw new ArgumentNullException("installInfos"
                                               , TextManager.DefaultInstance.GetTextWithTail(Texts.DotNetServiceInstaller_05));
            ServiceInstallerHelper installerHelp = new ServiceInstallerHelper();
            for (Int32 i = 0; i < installInfos.ServiceInstallInfos.Length; i++)
            {
                try
                {
                    this.Install(installInfos.ServiceInstallInfos[i]);
                }
                catch
                {
                    throw;
                }
            }
        }
        #endregion

        #region 服务卸载
        /// <summary>
        /// 服务卸载
        /// </summary>
        /// <param name="serviceInfos"></param>
        public void UnInstall(ServiceInstallInfo serviceInfo)
        {
            if (serviceInfo == null
                || String.IsNullOrWhiteSpace(serviceInfo.ServiceName))
                throw new ArgumentNullException("serviceInfo"
                                               , TextManager.DefaultInstance.GetTextWithTail(Texts.DotNetServiceInstaller_06));
            this.UnInstall(serviceInfo.ServiceName, serviceInfo.InstallLogFilePath);
        }

        /// <summary>
        /// 服务卸载
        /// </summary>
        /// <param name="serviceInfos"></param>
        public void UnInstall(ServiceInstallInfoListModel serviceInfos)
        {

            if (serviceInfos == null
                || serviceInfos.ServiceInstallInfos == null
                || serviceInfos.ServiceInstallInfos.Length == 0)
                throw new ArgumentNullException("serviceInfos"
                                               , TextManager.DefaultInstance.GetTextWithTail(Texts.DotNetServiceInstaller_07));
            for (Int32 i = 0; i < serviceInfos.ServiceInstallInfos.Length; i++)
                this.UnInstall(serviceInfos.ServiceInstallInfos[i].ServiceName,
                    serviceInfos.ServiceInstallInfos[i].InstallLogFilePath);
        }

        /// <summary>
        /// 服务卸载
        /// </summary>
        /// <param name="serviceName"></param>
        public void UnInstall(String serviceName, String logFilePath)
        {
            if (String.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException("serviceName"
                                               , TextManager.DefaultInstance.GetTextWithTail(Texts.DotNetServiceInstaller_08));
            if (!((Boolean)CheckServiceState(serviceName)["IfExists"]))
                throw new InvalidOperationException(TextManager.DefaultInstance.GetTextWithTail(Texts.DotNetServiceInstaller_09
                                                                                      , serviceName));

            if (!String.IsNullOrEmpty(logFilePath))
                logFilePath = Path.GetFullPath(logFilePath);
            ServiceInstallerHelper installerHelp = new ServiceInstallerHelper();
            installerHelp.ServiceName = serviceName;
            installerHelp.InstallLogFilePath = logFilePath;
            installerHelp.Uninstall(null);
        }
        #endregion

        #region Private Methods

        #region 从Module列表加载安装信息
        /// <summary>
        /// 从Module列表加载安装信息
        /// </summary>
        /// <param name="moduleList"></param>
        /// <param name="serviceInstallerList"></param>
        /// <param name="serviceProcessInstallerList"></param>
        private void LoadInstallInfosFromModuleList(List<Module> moduleList, out List<ServiceInstaller> serviceInstallerList, out List<ServiceProcessInstaller> serviceProcessInstallerList)
        {
            serviceInstallerList = new List<ServiceInstaller>();
            serviceProcessInstallerList = new List<ServiceProcessInstaller>();
            List<Type> typeList = new List<Type>();
            List<FieldInfo> fieldInfoList = new List<FieldInfo>();
            Installer installerInstance;
            ServiceInstaller serviceInstaller;
            ServiceProcessInstaller serviceProcessInstaller;
            foreach (Module moduleItem in moduleList)
            {
                typeList.AddRange(moduleItem.GetTypes());
                foreach (Type typeItem in typeList)
                {
                    if (typeof(Installer).IsAssignableFrom(typeItem)
                        && typeItem.IsPublic && !typeItem.IsAbstract
                        && ((RunInstallerAttribute)TypeDescriptor.GetAttributes(typeItem)[typeof(RunInstallerAttribute)]).RunInstaller)
                    {
                        fieldInfoList.AddRange(typeItem.GetFields(BindingFlags.Public
                                                                 | BindingFlags.NonPublic
                                                                 | BindingFlags.Static
                                                                 | BindingFlags.Instance));
                        foreach (FieldInfo fieldInfoItem in fieldInfoList)
                        {
                            if (typeof(ServiceInstaller).IsAssignableFrom(fieldInfoItem.FieldType))
                            {
                                installerInstance = (Installer)Activator.CreateInstance(typeItem);
                                serviceInstaller = (ServiceInstaller)fieldInfoItem.GetValue(installerInstance);
                                serviceInstallerList.Add(serviceInstaller);
                            }
                            else if (typeof(ServiceProcessInstaller).IsAssignableFrom(fieldInfoItem.FieldType))
                            {
                                installerInstance = (Installer)Activator.CreateInstance(typeItem);
                                serviceProcessInstaller = (ServiceProcessInstaller)fieldInfoItem.GetValue(installerInstance);
                                serviceProcessInstallerList.Add(serviceProcessInstaller);
                            }
                        }
                        fieldInfoList.Clear();
                    }
                }
                typeList.Clear();
            }
        }
        #endregion

        #region 获取安装信息
        /// <summary>
        /// 获取安装信息
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="serviceInstallerList"></param>
        /// <param name="serviceProcessInstallerList"></param>
        /// <returns></returns>
        private List<ServiceInstallInfo> GetInstallInfoList(Assembly assembly, List<ServiceInstaller> serviceInstallerList, List<ServiceProcessInstaller> serviceProcessInstallerList)
        {
            Int32 loopTimes = Math.Min(serviceInstallerList.Count, serviceProcessInstallerList.Count);
            List<ServiceInstallInfo> retVal = new List<ServiceInstallInfo>();
            ServiceInstallInfo installInfo;
            for (Int32 i = 0; i < loopTimes; i++)
            {
                if (serviceInstallerList[i] == null || serviceProcessInstallerList[i] == null)
                    continue;
                installInfo = new ServiceInstallInfo();
                installInfo.ServiceFilePath = Path.GetFullPath(assembly.Location);
                installInfo.ServiceName = serviceInstallerList[i].ServiceName;
                installInfo.DisplayName = serviceInstallerList[i].DisplayName;
                installInfo.Description = serviceInstallerList[i].Description;
                installInfo.StartType = serviceInstallerList[i].StartType;
                installInfo.IfDelayedAutoStart = serviceInstallerList[i].DelayedAutoStart;
                installInfo.ServicesDependedOn = serviceInstallerList[i].ServicesDependedOn;
                installInfo.InstallLogFilePath = Path.GetFullPath(String.Format("{0}\\{1}.InstallLog"
                                                                               , Path.GetDirectoryName(Path.GetFullPath(assembly.Location))
                                                                               , Path.GetFileName(assembly.Location)));
                installInfo.ServiceAccount.Account = serviceProcessInstallerList[i].Account;
                installInfo.ServiceAccount.UserName = String.IsNullOrEmpty(serviceProcessInstallerList[i].Username) ? String.Empty : serviceProcessInstallerList[i].Username;
                installInfo.ServiceAccount.Password = String.IsNullOrEmpty(serviceProcessInstallerList[i].Password) ? String.Empty : serviceProcessInstallerList[i].Password;
                retVal.Add(installInfo);
            }
            return retVal;
        }
        #endregion

        #endregion
    }
    #endregion
}