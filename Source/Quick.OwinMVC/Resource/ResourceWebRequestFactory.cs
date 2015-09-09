using System;
using System.Collections.Generic;
using System.Net;

namespace Quick.OwinMVC.Resource
{
    public class ResourceWebRequestFactory : IWebRequestCreate
    {
        public IDictionary<string, string> PluginAliasMap { get; internal set; }
        public String StaticFileFolder { get; set; } = String.Empty;

        public WebRequest Create(Uri uri)
        {
            return new ResourceWebRequest(uri, PluginAliasMap, StaticFileFolder);
        }
    }
}
