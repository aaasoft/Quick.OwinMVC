using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Quick.OwinMVC.Resource
{
    public class ResourceWebRequestFactory : IWebRequestCreate
    {
        public IDictionary<String, Assembly> AssemblyMap { get; internal set; }
        public IDictionary<string, string> PluginAliasMap { get; internal set; }
        public String StaticFileFolder { get; set; } = String.Empty;

        public WebRequest Create(Uri uri)
        {
            return new ResourceWebRequest(uri, AssemblyMap, PluginAliasMap, StaticFileFolder);
        }
    }
}
