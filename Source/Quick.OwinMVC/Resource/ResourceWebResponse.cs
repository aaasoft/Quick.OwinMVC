using Quick.OwinMVC.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace Quick.OwinMVC.Resource
{
    public class ResourceWebResponse : WebResponse
    {
        private Uri uri;
        private FileInfo fileInfo;
        private ManifestResourceInfo resourceInfo;
        private String resourceName;
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// 关联的Uri对象
        /// </summary>
        public Uri Uri { get { return this.uri; } }

        public ResourceWebResponse(Uri uri, IDictionary<String, Assembly> assemblyMap, IDictionary<string, string> pluginAliasMap, String staticFileFolder)
        {
            this.uri = uri;
            //得到插件名
            var pluginName = uri.Host;
            if (pluginName == "0"
                || pluginName == "0.0.0.0")
                pluginName = ".";
            //得到资源名
            resourceName = uri.LocalPath;

            //设置资源搜索目录
            List<String> searchFolderList = new List<string>();
            searchFolderList.Add(Path.Combine(staticFileFolder, pluginName));
            if (pluginAliasMap != null && pluginAliasMap.ContainsKey(pluginName))
            {
                pluginName = pluginAliasMap[pluginName];
                searchFolderList.Add(Path.Combine(staticFileFolder, pluginName));
            }
            pluginName = pluginName.ToLower();
            //获取到程序集
            if (assemblyMap.ContainsKey(pluginName))
                Assembly = assemblyMap[pluginName];

            //开始在文件系统上搜索资源
            String fullFilePath = null;
            foreach (var searchFolder in searchFolderList)
            {
                fullFilePath = ResourceUtils.findFilePath(searchFolder, resourceName);
                if (fullFilePath != null)
                    break;
            }
            if (fullFilePath != null)
                fileInfo = new FileInfo(fullFilePath);
            else if (pluginName == ".") { }
            else
            {
                if (Assembly == null)
                    return;
                String assemblyName = Assembly.GetName().Name;
                while (resourceName.StartsWith("/"))
                    resourceName = resourceName.Substring(1);

                Func<string, string> dirHandler = t => t.Replace("-", "_");
                Func<string, string> fileNameHandler = t => t.Replace("/", ".");

                List<string> resourceNameList = new List<string>();
                resourceNameList.Add(fileNameHandler.Invoke(resourceName));

                var lastDotIndex = resourceName.LastIndexOf('.');
                if (lastDotIndex > 0)
                {
                    var dirName = Path.GetDirectoryName(resourceName);
                    var fileName = Path.GetFileName(resourceName);

                    dirName = dirName.Replace(Path.DirectorySeparatorChar, '/');
                    dirName = dirHandler(dirName);
                    dirName = dirName.Replace(".", "._");

                    resourceNameList.Add(fileNameHandler.Invoke($"{dirName}/{fileName}"));
                }
                resourceNameList.Add(resourceName);

                if (lastDotIndex < 0)
                    lastDotIndex = 0;
                resourceNameList.Add(fileNameHandler.Invoke(resourceName.Insert(lastDotIndex, ".")));

                foreach (var currentResourceName in resourceNameList)
                {
                    var fullResourceName = $"{assemblyName}.{currentResourceName}";
                    resourceInfo = Assembly.GetManifestResourceInfo(fullResourceName);
                    if (resourceInfo != null)
                    {
                        resourceName = fullResourceName;
                        break;
                    }
                }
            }
        }

        public override System.IO.Stream GetResponseStream()
        {
            if (fileInfo != null)
                return fileInfo.OpenRead();
            if (resourceInfo != null)
                return Assembly.GetManifestResourceStream(resourceName);
            return null;
        }

        public override Uri ResponseUri { get { return uri; } }

        public override long ContentLength
        {
            get
            {
                if (fileInfo != null)
                    return fileInfo.Length;
                if (resourceInfo != null)
                {
                    using (var stream = GetResponseStream())
                    {
                        var l = stream.Length;
                        stream.Close();
                        return l;
                    }

                }
                return -1;
            }
            set { base.ContentLength = value; }
        }

        public override string ContentType
        {
            get
            {
                return MimeUtils.GetMime(uri.LocalPath);
            }
            set { base.ContentType = value; }
        }

        /// <summary>
        /// 最后修改时间(UTC时间)
        /// </summary>
        public DateTime LastModified
        {
            get
            {
                if (fileInfo != null)
                    return fileInfo.LastWriteTimeUtc;
                if (resourceInfo != null)
                    return File.GetLastWriteTimeUtc(Assembly.Location);
                return DateTime.MinValue;
            }
        }
    }
}
