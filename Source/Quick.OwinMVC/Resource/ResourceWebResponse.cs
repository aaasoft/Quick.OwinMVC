using Quick.OwinMVC.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace Quick.OwinMVC.Resource
{
    public class ResourceWebResponse : WebResponse
    {
        private Uri uri;
        private FileInfo fileInfo;
        private ManifestResourceInfo resourceInfo;
        private String resourceName;
        private Assembly assembly;

        /// <summary>
        /// 关联的Uri对象
        /// </summary>
        public Uri Uri { get { return this.uri; } }

        public ResourceWebResponse(Uri uri, IDictionary<String, Assembly> assemblyMap, IDictionary<string, string> pluginAliasMap, String staticFileFolder)
        {
            this.uri = uri;
            //得到插件名
            var pluginName = uri.Host;
            if (pluginName == "0")
                pluginName = ".";
            //得到资源名
            resourceName = uri.LocalPath;
            while (resourceName.StartsWith("/"))
                resourceName = resourceName.Substring(1);
            resourceName = resourceName.Replace("/", ".");
            
            //设置资源搜索目录
            List<String> searchFolderList = new List<string>();
            searchFolderList.Add(Path.Combine(staticFileFolder, pluginName));
            if (pluginAliasMap != null && pluginAliasMap.ContainsKey(pluginName))
            {
                pluginName = pluginAliasMap[pluginName];
                searchFolderList.Add(Path.Combine(staticFileFolder, pluginName));
            }
            pluginName = pluginName.ToLower();
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
                if (!assemblyMap.ContainsKey(pluginName))
                    return;
                assembly = assemblyMap[pluginName];
                String assemblyName = assembly.GetName().Name;
                resourceName = $"{assemblyName}.{resourceName}";
                resourceInfo = assembly.GetManifestResourceInfo(resourceName);
            }
        }

        public override System.IO.Stream GetResponseStream()
        {
            if (fileInfo != null)
                return fileInfo.OpenRead();
            if (resourceInfo != null)
                return assembly.GetManifestResourceStream(resourceName);
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
                    return File.GetLastWriteTimeUtc(assembly.Location);
                return DateTime.MinValue;
            }
        }
    }
}
