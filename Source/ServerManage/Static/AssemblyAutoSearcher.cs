using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerManage.Static
{
    public class AssemblyAutoSearcher
    {
        private static String baseDirectory;
        private static String[] assemblySearchPathArray;

        static AssemblyAutoSearcher()
        {
            baseDirectory = Path.GetDirectoryName(typeof(AssemblyAutoSearcher).Assembly.Location);

            List<String> pathList = new List<string>();
            pathList.Add(baseDirectory);
            pathList.Add(Path.Combine(baseDirectory, "Libs"));
            if (Environment.Is64BitProcess)
                pathList.Add(Path.Combine(baseDirectory, "Libs", "x64"));
            else
                pathList.Add(Path.Combine(baseDirectory, "Libs", "x86"));
            pathList.AddRange(Directory.GetDirectories(Path.Combine(baseDirectory, "Plugins")));
            assemblySearchPathArray = pathList.ToArray();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);


            return TryLoadAssemblyFromFolders(assemblyName, assemblySearchPathArray);
        }

        public static void Init()
        {
            DirectoryInfo di = null;
            //加载Modules目录下所有的程序集
            di = new DirectoryInfo(Path.Combine(baseDirectory, "Plugins"));
            if (di.Exists)
            {
                foreach (DirectoryInfo pluginDi in di.GetDirectories())
                {
                    String pageName = pluginDi.Name;
                    String pageFilePath = Path.Combine(pluginDi.FullName, pageName + ".dll");
                    if (!File.Exists(pageFilePath)) continue;
                    Assembly assembly = Assembly.UnsafeLoadFrom(pageFilePath);
                }
            }

            //加载Libs目录下所有的程序集
            di = new DirectoryInfo(Path.Combine(baseDirectory, "Libs"));
            foreach (FileInfo fi in di.GetFiles("*.dll"))
            {
                Assembly.UnsafeLoadFrom(fi.FullName);
            }
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
