using System;
using System.Runtime.InteropServices;

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
    }
}
