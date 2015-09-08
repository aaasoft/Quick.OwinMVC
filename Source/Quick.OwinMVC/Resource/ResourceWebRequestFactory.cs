using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Resource
{
    public class ResourceWebRequest : WebRequest
    {
        private Uri uri;
        private String staticFileFolder;

        public ResourceWebRequest(Uri uri, String staticFileFolder)
        {
            this.uri = uri;
            this.staticFileFolder = staticFileFolder;
        }

        public override WebResponse GetResponse()
        {
            return new ResourceWebResponse(uri, staticFileFolder);
        }
    }

    public class ResourceWebResponse : WebResponse
    {
        private Uri uri;
        private Assembly assembly;
        private String resourceName;
        private FileInfo resourceFile;

        public ResourceWebResponse(Uri uri, String staticFileFolder)
        {
            this.uri = uri;
            assembly = Assembly.Load(uri.Host);
            String assemblyName = assembly.GetName().Name;
            resourceName = uri.LocalPath;
            while (resourceName.StartsWith("/"))
                resourceName = resourceName.Substring(1);

            String fullFilePath = Path.Combine(staticFileFolder, assemblyName, resourceName);
            if (File.Exists(fullFilePath))
                resourceFile = new FileInfo(fullFilePath);
            else
            {
                resourceName = resourceName.Replace("/", ".");
                resourceName = $"{assemblyName}.{resourceName}";
            }
        }
        private ManifestResourceInfo GetResourceInfo()
        {
            return assembly.GetManifestResourceInfo(resourceName);
        }
        public override System.IO.Stream GetResponseStream()
        {
            if (resourceFile == null)
            {
                var resourceInfo = GetResourceInfo();
                if (resourceInfo == null)
                    return null;
                return assembly.GetManifestResourceStream(resourceName);
            }
            else
            {
                return resourceFile.OpenRead();
            }
        }

        public override Uri ResponseUri { get { return uri; } }

        public override long ContentLength
        {
            get
            {
                if (resourceFile == null)
                {
                    var resourceInfo = GetResourceInfo();
                    if (resourceInfo == null)
                        return -1;
                    using (var stream = GetResponseStream())
                    {
                        return stream.Length;
                    }
                }
                else
                    return resourceFile.Length;
            }
            set { base.ContentLength = value; }
        }

        public override string ContentType
        {
            get { return "application/octet-stream"; }
            set { base.ContentType = value; }
        }

        /// <summary>
        /// 最后修改时间(UTC时间)
        /// </summary>
        public DateTime LastModified
        {
            get
            {
                return File.GetLastWriteTimeUtc(assembly.Location);
            }
        }
    }

    public class ResourceWebRequestFactory : IWebRequestCreate
    {
        public String StaticFileFolder { get; set; } = String.Empty;

        public WebRequest Create(Uri uri)
        {
            return new ResourceWebRequest(uri, StaticFileFolder);
        }
    }
}
