using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;

namespace Quick.OwinMVC.Resource
{
    public class ResourceUtils
    {
        //搜索文件
        internal static String findFilePath(String baseFolder, String fileName)
        {
            String fullFileName = Path.Combine(baseFolder, fileName);
            if (File.Exists(fullFileName))
                return fullFileName;

            String[] nameArray = fileName.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < nameArray.Length; i++)
            {
                String folderName = String.Join(".", nameArray, 0, i);
                String fullFolderPath = Path.Combine(baseFolder, folderName);
                if (!Directory.Exists(fullFolderPath))
                    continue;
                String subFileName = String.Join(".", nameArray, i, nameArray.Length - i);

                fullFileName = findFilePath(fullFolderPath, subFileName);
                if (fullFileName != null)
                    return fullFileName;
            }
            return null;
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="fileNameList"></param>
        /// <param name="assembly"></param>
        /// <param name="baseFolder"></param>
        /// <param name="fullFileNameTemplate"></param>
        /// <param name="pathParts"></param>
        /// <returns></returns>
        public static Stream GetResource(List<String> fileNameList, Assembly assembly, String baseFolder, params Object[] pathParts)
        {
            String findedResourcePath;
            return GetResource(fileNameList, assembly, baseFolder, out findedResourcePath, pathParts);
        }

        public static Stream GetResource(List<String> fileNameList, Assembly assembly, String baseFolder, out String findedResourcePath, params Object[] pathParts)
        {
            findedResourcePath = null;
            Uri uri = GetResourceUri(fileNameList, assembly, baseFolder, pathParts);
            if (uri == null)
                return null;
            findedResourcePath = uri.ToString();
            return WebRequest.Create(uri).GetResponse().GetResponseStream();
        }

        public static Uri GetResourceUri(List<String> fileNameList, Assembly assembly, String baseFolder, params Object[] pathParts)
        {
            //文件是否存在
            Boolean isFileExists = false;
            String filePath = null;
            foreach (String fileName in fileNameList)
            {
                //先判断全名文件是否存在
                filePath = findFilePath(baseFolder, fileName);
                isFileExists = filePath != null;
                if (isFileExists)
                    break;
            }

            //先尝试从目录加载
            if (isFileExists)
            {
                return new Uri(filePath);
            }
            //然后尝试从程序集资源中加载
            else
            {
                String assemblyName = assembly.GetName().Name;

                //先寻找嵌入的资源
                foreach (String fileName in fileNameList)
                {
                    String resourceName = String.Join(".", pathParts);
                    resourceName = resourceName.Replace("[fileName]", fileName);
                    resourceName = resourceName.Replace("-", "_");
                    if (IsEmbedResourceExist(assembly, resourceName))
                        return new Uri(String.Format("embed://{0}/{1}", assemblyName, resourceName));
                }
                //然后寻找Resource资源
                if (IsEmbedResourceExist(assembly, assemblyName + ".g.resources"))
                {
                    foreach (String fileName in fileNameList)
                    {
                        String resourceName = String.Join("/", pathParts);
                        resourceName = resourceName.Replace("[fileName]", fileName);

                        String assemblyNamePrefix = assemblyName + "/";
                        if (resourceName.StartsWith(assemblyNamePrefix))
                            resourceName = resourceName.Substring(assemblyNamePrefix.Length);
                        resourceName = GetResourceResourcePath(assembly, resourceName);
                        if (!String.IsNullOrEmpty(resourceName))
                            return new Uri(String.Format("pack://application:,,,/{0};component/{1}", assemblyName, resourceName));
                    }
                }
            }
            return null;
        }

        //程序集的嵌入的资源字典
        private static Dictionary<Assembly, String[]> assemblyEmbedResourceDict = new Dictionary<Assembly, string[]>();
        //程序集的Resource编译的资源字典
        private static Dictionary<Assembly, Dictionary<String, String>> assemblyResourceResourceDict = new Dictionary<Assembly, Dictionary<String, String>>();

        //确保程序集的资源元数据信息都已加载
        private static void makeSureLoadAssemblyResourceMetaData(Assembly assembly)
        {
            String assemblyName = assembly.GetName().Name;
            lock (typeof(ResourceUtils))
            {
                //嵌入的资源
                if (!assemblyEmbedResourceDict.ContainsKey(assembly))
                {
                    assemblyEmbedResourceDict[assembly] = assembly.GetManifestResourceNames();
                }
                //Resource的资源
                string resBaseName = assemblyName + ".g.resources";
                if (!assemblyResourceResourceDict.ContainsKey(assembly))
                {
                    assemblyResourceResourceDict[assembly] = new Dictionary<string, string>();
                    if (assemblyEmbedResourceDict[assembly].Contains(resBaseName))
                    {
                        String[] resourceList = new String[0];
                        using (var stream = assembly.GetManifestResourceStream(resBaseName))
                        {
                            if (stream != null)
                            {
                                using (var reader = new System.Resources.ResourceReader(stream))
                                {
                                    resourceList = reader.Cast<DictionaryEntry>().Select(entry =>
                                             (string)entry.Key).ToArray();
                                }
                            }
                        }
                        foreach (var resource in resourceList)
                        {
                            assemblyResourceResourceDict[assembly][resource.Replace('/', '.')] = resource;
                        }
                    }
                }
            }
        }

        public static Boolean IsEmbedResourceExist(Assembly assembly, String resourceName)
        {
            makeSureLoadAssemblyResourceMetaData(assembly);
            return assemblyEmbedResourceDict[assembly].Contains(resourceName);
        }

        public static String GetResourceResourcePath(Assembly assembly, String resourceName)
        {
            makeSureLoadAssemblyResourceMetaData(assembly);
            resourceName = resourceName.Replace('/', '.');
            resourceName = resourceName.ToLower();
            if (assemblyResourceResourceDict[assembly].ContainsKey(resourceName))
                return assemblyResourceResourceDict[assembly][resourceName];
            return null;
        }

        private static String takePathPart(String resourcePath, String separator, Int32 startIndex = -1, Int32 count = 0)
        {
            var resourcePathParts = resourcePath.Split(new String[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            if (startIndex >= resourcePathParts.Length)
                startIndex = resourcePathParts.Length - 1;
            if (count > resourcePathParts.Length)
                count = resourcePathParts.Length;
            if (startIndex < 0)
                startIndex = resourcePathParts.Length - count - 1;
            if (count <= 0)
                count = resourcePathParts.Length - startIndex;

            if (startIndex < 0 && count <= 0)
                return resourcePath;

            return String.Join(separator, resourcePathParts, startIndex, count);
        }

        public static String[] GetResourcePaths(Assembly assembly, String baseFolder, String themePathInAssembly)
        {
            String assemblyName = assembly.GetName().Name;
            makeSureLoadAssemblyResourceMetaData(assembly);
            List<String> resourcePathList = new List<String>();
            //先搜索目录的资源
            if (Directory.Exists(baseFolder))
            {
                DirectoryInfo di = new DirectoryInfo(baseFolder);
                String dirFullName = di.FullName;
                foreach (var fi in di.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    var parts = fi.FullName.Substring(dirFullName.Length).Split(new Char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
                    resourcePathList.Add(String.Join(".", parts));
                }
            }
            //然后搜索程序集中的嵌入资源
            foreach (var resourcePath in assemblyEmbedResourceDict[assembly]
                .Where(t => t.StartsWith(String.Format("{0}.{1}", assemblyName, themePathInAssembly))))
                resourcePathList.Add(takePathPart(resourcePath, ".", 2));

            //然后搜索程序集中的Resource资源
            foreach (var resourcePath in assemblyResourceResourceDict[assembly].Keys
                .Where(t => t.StartsWith(themePathInAssembly.ToLower())))
                resourcePathList.Add(takePathPart(resourcePath, ".", 1));

            return resourcePathList.Select(t => String.Format("resource://{0}/{1}", assemblyName, t)).ToArray();
        }

        public static String GetResourceText(List<String> fileNameList, Assembly assembly, String baseFolder, params Object[] pathParts)
        {
            String findedResourcePath;
            return GetResourceText(fileNameList, assembly, baseFolder, out findedResourcePath, pathParts);
        }

        public static String GetResourceText(List<String> fileNameList, Assembly assembly, String baseFolder, out String findedResourcePath, params Object[] pathParts)
        {
            Stream resourceStream = GetResource(fileNameList, assembly, baseFolder, out findedResourcePath, pathParts);
            if (resourceStream == null)
                return null;

            using (resourceStream)
            {
                String fileContent = null;
                StreamReader streamReader = new StreamReader(resourceStream);
                fileContent = streamReader.ReadToEnd();
                streamReader.Close();
                resourceStream.Close();
                return fileContent;
            }
        }
    }
}
