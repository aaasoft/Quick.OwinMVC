using System;
using System.Collections.Generic;
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
        public ResourceWebRequest(Uri uri)
        {
            this.uri = uri;
        }

        public override WebResponse GetResponse()
        {
            //resource://{0}/{1}
            Assembly assembly = Assembly.Load(uri.Host);
            String assemblyName = assembly.GetName().Name;
            String resourceName = uri.LocalPath;
            while (resourceName.StartsWith("/"))
                resourceName = resourceName.Substring(1);

            resourceName = resourceName.Replace("/", ".");
            resourceName = $"{assemblyName}.{resourceName}";

            //从嵌入资源中搜索
            var resourceInfo = assembly.GetManifestResourceInfo(resourceName);
            if (resourceInfo == null)
                return null;
            return new EmbedWebResponse(assembly, resourceName);
        }
    }

    public class ResourceWebRequestFactory : IWebRequestCreate
    {
        public WebRequest Create(Uri uri)
        {
            return new ResourceWebRequest(uri);
        }
    }
}
