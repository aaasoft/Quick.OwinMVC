using Quick.OwinMVC;
using Quick.OwinMVC.Hunter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Program.Service.Impl
{
    public class WebServerService : IService, IPropertyHunter, IHungryPropertyHunter
    {
        /// <summary>
        /// WEB服务URI
        /// </summary>
        private Uri WebServerUri { get; set; }
        private IDictionary<string, string> properties;

        private Server server = null;

        public String Name { get; } = "WEB服务";

        public void Start()
        {
            server = new Server(properties, WebServerUri);
            server.Start();
            Console.Write("->地址：" + this.server.GetUrl());
#if DEBUG
            var webServerUriKey = $"{this.GetType().FullName}.{nameof(WebServerUri)}";
            var url = properties[webServerUriKey].Replace("net://", "http://").Replace("http://0.0.0.0", "http://127.0.0.1");
            //System.Diagnostics.Process.Start(url);
#endif
        }

        public void Stop()
        {
            server.Stop();
            server = null;
        }

        public void Hunt(IDictionary<string, string> properties)
        {
            this.properties = properties;
#if DEBUG
            //修改调试的WEB服务端口为8094
            var webServerUriKey = $"{this.GetType().FullName}.{nameof(WebServerUri)}";
            if (properties.ContainsKey(webServerUriKey))
                properties[webServerUriKey] = "net://0.0.0.0:8094";
#endif
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
    }
}
