using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SvnManage.Utils
{
    public class SystemInfoUtils
    {
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public class ReadFileAttribute : Attribute
        {
            public PlatformID PlatformID { get; set; }
            /// <summary>
            /// 文件
            /// </summary>
            public String File { get; set; }
            public Regex Regex { get; set; }
            public ReadFileAttribute(PlatformID platformID,String file, String regex = null)
            {
                this.PlatformID = platformID;
                File = file;
                if (regex != null)
                    Regex = new Regex(regex);
            }
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public class ShellCmdAttribute : Attribute
        {
            public PlatformID PlatformID { get; set; }
            /// <summary>
            /// 程序名称
            /// </summary>
            public String Program { get; set; }
            /// <summary>
            /// 参数
            /// </summary>
            public String Arguments { get; set; }
            public Regex Regex { get; set; }
            public ShellCmdAttribute(PlatformID platformID, String program, String arguments, String regex)
            {
                this.PlatformID = platformID;
                this.Program = program;
                this.Arguments = arguments;
                this.Regex = new Regex(regex);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
        private static PerformanceCounter cpuCounter = null;
        private static PerformanceCounter memFreeCounter = null;

        static SystemInfoUtils()
        {
            if (!IsRuningOnWindows())
                return;
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            memFreeCounter = new PerformanceCounter("Memory", "Available KBytes");
        }

        private static string readFile()
        {
            PlatformID currentPlatform = Environment.OSVersion.Platform;
            var stackFrame = new StackTrace(1).GetFrame(0);
            var method = stackFrame.GetMethod();
            var readFileAttr = method.GetCustomAttributes(typeof(ReadFileAttribute), false)
                .Cast<ReadFileAttribute>()
                .Where(t => t.PlatformID == currentPlatform).FirstOrDefault();
            if (!File.Exists(readFileAttr.File))
                return null;
            var content = File.ReadAllText(readFileAttr.File);
            if (readFileAttr.Regex == null)
                return content;

            var match = readFileAttr.Regex.Match(content);
            if (match == null || !match.Success)
                return null;
            var value = match.Groups["value"].Value;
            return value;
        }

        private static string executeShell()
        {
            PlatformID currentPlatform = Environment.OSVersion.Platform;
            var stackFrame = new StackTrace(1).GetFrame(0);
            var method = stackFrame.GetMethod();
            var shellCmdAttr = method.GetCustomAttributes(typeof(ShellCmdAttribute), false)
                .Cast<ShellCmdAttribute>()
                .Where(t => t.PlatformID == currentPlatform).FirstOrDefault();
            if (shellCmdAttr == null)
                return null;

            String programName = shellCmdAttr.Program;
            String arguments = shellCmdAttr.Arguments;

            var process = new Process();
            var psi = process.StartInfo;
            psi.FileName = programName;
            psi.Arguments = arguments;

            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;

            process.Start();
            var content = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var match = shellCmdAttr.Regex.Match(content);
            if (match == null || !match.Success)
                return null;
            var value = match.Groups["value"].Value;
            return value;
        }

        private static Boolean IsRuningOnWindows()
        {
            return Environment.OSVersion.Platform == PlatformID.Win32NT
                || Environment.OSVersion.Platform == PlatformID.Win32S
                || Environment.OSVersion.Platform == PlatformID.Win32Windows
                || Environment.OSVersion.Platform == PlatformID.WinCE;
        }

        /// <summary>
        /// 获取计算机名称
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Win32NT, "cmd", "/c wmic computersystem get Caption", @"^Caption\s*(?'value'.*?)\s*$")]
        [ShellCmd(PlatformID.Unix, "bash", "-c \"uname -n\"", @"^\s*(?'value'.*?)\s*$")]
        public static String GetComputerName()
        {
            return Environment.MachineName;
        }

        /// <summary>
        /// 获取操作系统名称
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Win32NT, "cmd", "/c wmic OS get Caption", @"^Caption\s*(?'value'.*?)\s*$")]
        [ShellCmd(PlatformID.Unix, "bash", "-c \"uname -s -r -v -m -o\"", @"^\s*(?'value'.*?)\s*$")]
        public static String GetOsName()
        {
            if (IsRuningOnWindows())
                return Environment.OSVersion.ToString();
            return executeShell();
        }

        private static Int32 preCpuIdle = 0;
        private static Int32 preCpuTotal = 0;
        /// <summary>
        /// 获取CPU使用率
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Win32NT, "cmd", "/c wmic cpu get LoadPercentage", @"^LoadPercentage\s*(?'value'.*?)\s*$")]
        [ShellCmd(PlatformID.Unix, "bash", "-c \"grep 'cpu ' /proc/stat| awk '{value=($2+$4)*100/($2+$4+$5)} END {print value}'\"", @"^\s*(?'value'.*?)\s*$")]
        [ReadFile(PlatformID.Unix, "/proc/stat", "cpu (?'value'.*)")]
        public static Double GetCpuUsage()
        {
            if (IsRuningOnWindows())
                return cpuCounter.NextValue();

            var value = readFile();
            if (String.IsNullOrEmpty(value))
                return 0;

            var intValues = value.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => Int32.Parse(t))
                .ToArray();

            var idle = intValues[3];
            var total = intValues.Sum();

            var spIdle = idle - preCpuIdle;
            var spTotal = total - preCpuTotal;

            preCpuIdle = idle;
            preCpuTotal = total;

            return (spTotal - spIdle) * 100F / spTotal;
        }

        /// <summary>
        /// 获取CPU温度
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Unix, "bash", "-c \"cat /sys/class/thermal/thermal_zone0/temp\"", @"^\s*(?'value'.*?)\s*$")]
        [ReadFile(PlatformID.Unix, "/sys/class/thermal/thermal_zone0/temp")]
        public static Double GetCpuTempature()
        {
            if (IsRuningOnWindows())
                return 0;
            var value = readFile();
            if (String.IsNullOrEmpty(value))
                return 0;
            return Double.Parse(value) / 1000;
        }

        /// <summary>
        /// 获取总内存数
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Win32NT, "cmd", "/c wmic OS get TotalVisibleMemorySize", @"^TotalVisibleMemorySize\s*(?'value'.*?)\s*$")]
        [ShellCmd(PlatformID.Unix, "bash", "-c \"free | awk 'NR==2{total= $2}END{print total}'\"", @"^\s*(?'value'.*?)\s*$")]
        [ReadFile(PlatformID.Unix, "/proc/meminfo", @"MemTotal:\s*(?'value'\d*)\s*kB")]
        public static long GetTotalMemory()
        {
            if (IsRuningOnWindows())
            {
                MEMORYSTATUSEX meminfo = new MEMORYSTATUSEX();
                GlobalMemoryStatusEx(meminfo);
                return Convert.ToInt64(meminfo.ullTotalPhys);
            }
            var value = readFile();
            if (String.IsNullOrEmpty(value))
                return 0;
            return Int64.Parse(value) * 1024;
        }

        /// <summary>
        /// 获取空闲内存数
        /// </summary>
        /// <returns></returns>
        [ShellCmd(PlatformID.Win32NT, "cmd", "/c wmic OS get FreePhysicalMemory", @"^FreePhysicalMemory\s*(?'value'.*?)\s*$")]
        [ShellCmd(PlatformID.Unix, "bash", "-c \"free | awk 'NR==3{free= $4}END{print free}'\"", @"^\s*(?'value'.*?)\s*$")]
        [ReadFile(PlatformID.Unix, "/proc/meminfo", @"MemAvailable:\s*(?'value'\d*)\s*kB")]
        public static long GetFreeMemory()
        {
            if (IsRuningOnWindows())
            {
                if (IsRuningOnWindows())
                {
                    MEMORYSTATUSEX meminfo = new MEMORYSTATUSEX();
                    GlobalMemoryStatusEx(meminfo);
                    return Convert.ToInt64(meminfo.ullAvailPhys);
                }
            }
            var value = readFile();
            if (String.IsNullOrEmpty(value))
                return 0;
            return Int64.Parse(value) * 1024;
        }
    }
}
