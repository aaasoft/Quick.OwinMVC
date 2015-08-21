using NVelocity.Runtime.Resource.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons.Collections;
using NVelocity.Runtime.Resource;
using System.IO;
using System.Net;
using NVelocity.Exception;

namespace Quick.OwinMVC.View.NVelocity.ResourceLoaders
{
    public class EmbedResourceLoader : ResourceLoader
    {
        private String pluginNameAndPathSplitString;
        private String viewNameSuffix;
        private String viewNamePrefix;

        public override long GetLastModified(global::NVelocity.Runtime.Resource.Resource resource)
        {
            return resource.LastModified;
        }

        public override Stream GetResourceStream(string name)
        {
            if ("VM_global_library.vm".Equals(name))
                return null;

            String[] tmpArray = name.Split(new String[] { pluginNameAndPathSplitString }, StringSplitOptions.RemoveEmptyEntries);
            if (tmpArray.Length < 2)
            {
                throw new VelocityException("视图名称[" + name + "]不符合规则：“[插件名]"
                        + pluginNameAndPathSplitString + "[路径]”");
            }
            String pluginName = tmpArray[0];
            String path = tmpArray[1];
            // 对视图名称进行处理(添加前后缀)
            path = viewNamePrefix + path + viewNameSuffix;

            var resourceResponse = WebRequest.Create($"resource://{pluginName}/{path}").GetResponse();
            return resourceResponse?.GetResponseStream();
        }

        public override void Init(ExtendedProperties configuration)
        {
            this.pluginNameAndPathSplitString = configuration.GetString("class.pluginNameAndPathSplitString");
            if (this.pluginNameAndPathSplitString == null)
                this.pluginNameAndPathSplitString = ":";
            this.viewNamePrefix = configuration.GetString("class.viewNamePrefix");
            if (this.viewNamePrefix == null)
                this.viewNamePrefix = "View/";
            this.viewNameSuffix = configuration.GetString("class.viewNameSuffix");
            if (this.viewNameSuffix == null)
                this.viewNameSuffix = ".html";
        }

        public override bool IsSourceModified(global::NVelocity.Runtime.Resource.Resource resource)
        {
            return resource.IsSourceModified();
        }
    }
}
