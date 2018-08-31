using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Quick.OwinMVC.Startup.Utils
{
    public class ProgramUtils
    {
        #region 判断当前运行系统平台是否为Windows
        /// <summary>
        /// 判断当前运行系统平台是否为Windows
        /// </summary>
        /// <returns></returns>
        public static Boolean IsRuningOnWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
        }
        #endregion

        /// <summary>
        /// 检测是否是Mono运行环境
        /// </summary>
        /// <returns></returns>
        public static bool IsMonoRuntime()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        #region 分配控制台
        /// <summary>
        /// 分配控制台
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();
        #endregion

        /// <summary>
        /// 设置Dll目录
        /// </summary>
        /// <param name="lpPathName"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        /// <summary>
        /// 设置库文件(dll或so文件目录)路径
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public static bool SetLibDirectory(String pathName)
        {
            if (IsRuningOnWindows())
                return SetDllDirectory(pathName);
            return false;
        }

        public static Process StartSelfProcess(String arguments, bool needAdmin, bool hideConsole = true)
        {
            var fileName = Application.ExecutablePath;
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fileName, arguments);
            if (hideConsole)
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (needAdmin && System.Environment.OSVersion.Version.Major >= 6)
            {
                processStartInfo.Verb = "runas";
            }
            return Process.Start(processStartInfo);
        }
        public static string GetProgramTitle()
        {
            StringBuilder titleSb = new StringBuilder();
            //添加服务名称
            var assembly = Assembly.GetEntryAssembly();
            var productAttr = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            if (productAttr != null)
                titleSb.Append(productAttr.Product);
            //添加信息版本
            var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (informationalVersion != null)
                titleSb.Append(informationalVersion.InformationalVersion);
            return titleSb.ToString();
        }

        public static string GetProgramBuildTime()
        {
            var assembly = Assembly.GetEntryAssembly();
            return AssemblyUtils.GetLinkerTime(assembly).ToString("F");
        }

        public static string GetProgramVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            return assembly.GetName().Version.ToString();
        }

        public static string GetProgramTitleAndVersionAndBuildTime()
        {
            return $"{GetProgramTitle()} Ver:{GetProgramVersion()} BuildTime:{GetProgramBuildTime()}";
        }
    }
}
