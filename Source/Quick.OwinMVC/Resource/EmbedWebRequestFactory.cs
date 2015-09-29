using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

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
            Assembly assembly = Assembly.Load(uri.Host);
            String resourceName = uri.LocalPath;
            while (resourceName.StartsWith("/"))
                resourceName = resourceName.Substring(1);
            return new EmbedWebResponse(assembly, resourceName);
        }
    }

    public class EmbedWebResponse: WebResponse
    {
        private Assembly assembly;
        private String resourceName;
        public EmbedWebResponse(Assembly assembly, String resourceName)
        {
            this.assembly = assembly;
            this.resourceName = resourceName;
        }

        public override System.IO.Stream GetResponseStream()
        {
            return assembly.GetManifestResourceStream(resourceName);
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
