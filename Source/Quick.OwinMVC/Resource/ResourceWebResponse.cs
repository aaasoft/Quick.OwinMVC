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

        public ResourceWebResponse(Uri uri, IDictionary<string, string> pluginAliasMap, String staticFileFolder)
        {
            this.uri = uri;
            var pluginName = uri.Host;
            if (pluginAliasMap != null && pluginAliasMap.ContainsKey(pluginName))
                pluginName = pluginAliasMap[pluginName];

            resourceName = uri.LocalPath;
            while (resourceName.StartsWith("/"))
                resourceName = resourceName.Substring(1);

            String fullFilePath = Path.Combine(staticFileFolder, pluginName, resourceName);
            if (File.Exists(fullFilePath))
                fileInfo = new FileInfo(fullFilePath);
            else if (pluginName == ".") { }
            else
            {
                assembly = Assembly.Load(pluginName);
                String assemblyName = assembly.GetName().Name;

                resourceName = resourceName.Replace("/", ".");
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
