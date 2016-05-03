using Quick.OwinMVC.Startup.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Quick.OwinMVC.Startup.Static
{
    public class AssemblyAutoSearcher
    {
        private static String baseDirectory;
        private static String[] assemblySearchPathArray;

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);
            return TryLoadAssemblyFromFolders(assemblyName, assemblySearchPathArray);
        }

        /// <summary>
        /// 初始化
        /// </summary> 
        public static void Init()
        {
            baseDirectory = Path.GetDirectoryName(typeof(AssemblyAutoSearcher).Assembly.Location);
            Environment.CurrentDirectory = baseDirectory;

            List<String> pathList = new List<string>();
            pathList.Add(baseDirectory);
            pathList.Add(Path.Combine(baseDirectory, "Libs"));
            if (Environment.Is64BitProcess)
            {
                var path = Path.Combine(baseDirectory, "Libs", "x64");
                pathList.Add(path);
                ProgramUtils.SetLibDirectory(path);
            }
            else
            {
                var path = Path.Combine(baseDirectory, "Libs", "x86");
                ProgramUtils.SetLibDirectory(path);
            }

            var pluginDirectory = new DirectoryInfo(Path.Combine(baseDirectory, "Plugins"));
            if (pluginDirectory.Exists)
            {
                foreach (var pluginDi in pluginDirectory.GetDirectories())
                {
                    pathList.Add(pluginDi.FullName);

                    String pluginName = pluginDi.Name;
                    String pluginFilePath = Path.Combine(pluginDi.FullName, pluginName + ".dll");
                    if (!File.Exists(pluginFilePath)) continue;
                    Assembly assembly = Assembly.UnsafeLoadFrom(pluginFilePath);
                }
            }
            assemblySearchPathArray = pathList.ToArray();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        public static Assembly TryLoadAssemblyFromFolders(AssemblyName assemblyName, params String[] folderPaths)
        {
            Assembly assemblyRet = null;
            foreach (String folderPath in folderPaths)
            {
                assemblyRet = TryLoadAssemblyFromFolder(assemblyName, folderPath);
                if (assemblyRet != null) return assemblyRet;
            }
            return null;
        }

        public static Assembly TryLoadAssemblyFromFolder(AssemblyName assemblyName, String folderPath)
        {
            String filePath = Path.Combine(folderPath, assemblyName.Name + ".dll");
            if (!File.Exists(filePath)) return null;
            try
            {
                return Assembly.UnsafeLoadFrom(filePath);
            }
            catch { return null; }
        }
    }
}
