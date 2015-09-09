using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Quick.OwinMVC.Resource
{
    public class ResourceWebRequest : WebRequest
    {
        private Uri uri;
        private String staticFileFolder;
        private IDictionary<string, string> pluginAliasMap;

        public ResourceWebRequest(Uri uri, IDictionary<string, string> pluginAliasMap, String staticFileFolder)
        {
            this.uri = uri;
            this.pluginAliasMap = pluginAliasMap;
            this.staticFileFolder = staticFileFolder;
        }

        public override WebResponse GetResponse()
        {
            return new ResourceWebResponse(uri, pluginAliasMap, staticFileFolder);
        }
    }
}
