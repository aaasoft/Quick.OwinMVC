using NVelocity.Runtime.Resource.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons.Collections;
using NVelocity.Runtime.Resource;
using System.IO;

namespace Quick.OwinMVC.View.NVelocity
{
    public class MyResourceLoader : ResourceLoader
    {
        public override long GetLastModified(Resource resource)
        {
            throw new NotImplementedException();
        }

        public override Stream GetResourceStream(string source)
        {
            throw new NotImplementedException();
        }

        public override void Init(ExtendedProperties configuration)
        {
            throw new NotImplementedException();
        }

        public override bool IsSourceModified(Resource resource)
        {
            throw new NotImplementedException();
        }
    }
}
