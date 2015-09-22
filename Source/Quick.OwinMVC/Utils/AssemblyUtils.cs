using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Utils
{
    public class AssemblyUtils
    {
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

        /// <summary>
        /// {类名},{程序集名}
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public static Type GetType(String typeInfo)
        {
            String[] tmpArray = typeInfo.Split(new Char[] { ',', ';' });
            String className = tmpArray[0].Trim();
            String assemblyName = tmpArray[1].Trim();
            return Assembly.Load(assemblyName).GetType(className);
        }

        public static Object CreateObject(String typeInfo, params Object[] args)
        {
            Type type = GetType(typeInfo);
            return Activator.CreateInstance(type, args);
        }
    }
}
