using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Quick.OwinMVC.Resource
{
    public class ResourceWebRequest : WebRequest
    {
        private Uri uri;
        private String staticFileFolder;
        private IDictionary<String, Assembly> assemblyMap;
        private IDictionary<string, string> pluginAliasMap;

        public ResourceWebRequest(Uri uri, IDictionary<String, Assembly> assemblyMap, IDictionary<string, string> pluginAliasMap, String staticFileFolder)
        {
            this.uri = uri;
            this.assemblyMap = assemblyMap;
            this.pluginAliasMap = pluginAliasMap;
            this.staticFileFolder = staticFileFolder;
        }

        public override WebResponse GetResponse()
        {
            return new ResourceWebResponse(uri, assemblyMap, pluginAliasMap, staticFileFolder);
        }
    }
}
