using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Resource
{
    public class EmbedWebRequest : WebRequest
    {
        private Uri uri;
        public EmbedWebRequest(Uri uri)
        {
            this.uri = uri;
        }

        public override WebResponse GetResponse()
        {
            //embed://{0}/{1}
            return new EmbedWebResponse(uri);
        }
    }

    public class EmbedWebResponse : WebResponse
    {
        private Uri uri;
        private Assembly assembly;
        private String resourceName;
        public EmbedWebResponse(Uri uri)
        {
            this.uri = uri;
            assembly = Assembly.Load(uri.Host);
            resourceName = uri.LocalPath;
            while (resourceName.StartsWith("/"))
                resourceName = resourceName.Substring(1);
        }
        private ManifestResourceInfo GetResourceInfo()
        {
            return assembly.GetManifestResourceInfo(resourceName);
        }
        public override System.IO.Stream GetResponseStream()
        {
            var resourceInfo = GetResourceInfo();
            if (resourceInfo == null)
                return null;
            return assembly.GetManifestResourceStream(resourceName);
        }

        public override Uri ResponseUri { get { return uri; } }

        public override long ContentLength
        {
            get
            {
                var resourceInfo = GetResourceInfo();
                if (resourceInfo == null)
                    return -1;
                using (var stream = GetResponseStream())
                {
                    return stream.Length;
                }
            }
            set { base.ContentLength = value; }
        }

        public override string ContentType
        {
            get { return "application/octet-stream"; }
            set { base.ContentType = value; }
        }
    }

    public class EmbedWebRequestFactory : IWebRequestCreate
    {
        public WebRequest Create(Uri uri)
        {
            return new EmbedWebRequest(uri);
        }
    }
}
