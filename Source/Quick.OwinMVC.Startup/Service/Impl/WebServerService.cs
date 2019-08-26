using Quick.OwinMVC;
using Quick.OwinMVC.Hunter;
using Quick.OwinMVC.Plugin;
using Quick.OwinMVC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Startup.Service.Impl
{
    public class WebServerService : IService, IPropertyHunter, IHungryPropertyHunter
    {
        /// <summary>
        /// WEB服务URI
        /// </summary>
        public Uri WebServerUri { get; private set; }
        private IDictionary<string, string> properties;

        private Server server = null;

        public String Name { get; } = "WEB服务";

        public void Start()
        {
            server = new Server(properties, WebServerUri);
            server.Start();
            Console.Write("->地址：" + this.server.GetUrl());
        }

        public void Stop()
        {
            server.Stop();
            server = null;
        }

        public void Hunt(string key, string value)
        {
            switch (key)
            {
                case nameof(WebServerUri):
                    WebServerUri = new Uri(value);
                    break;
            }
        }

        public void Hunt(IDictionary<string, string> properties)
        {
            this.properties = properties;
        }
    }
}
