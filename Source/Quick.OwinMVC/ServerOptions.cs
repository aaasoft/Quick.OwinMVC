using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC
{
    [Serializable]
    public class ServerOptions
    {
        public string Name { get; set; }
        public IPEndPoint Endpoint { get; set; }
        public string Wrapper { get; set; }
        public string DefaultLanguage { get; set; } = "zh-CN";
        public string Middlewares { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public ServerOptions(
            string name,
            IPEndPoint endpoint,
            string wrapper,
            string middlewares,
            IDictionary<string, string> properties)
        {
            this.Name = name;
            this.Endpoint = endpoint;
            this.Wrapper = wrapper;
            this.Middlewares = middlewares;
            this.Properties = properties;
        }

        public String GetUrl()
        {
            var protocol = "http";
            var defaultPort = 80;

            string url = $"{protocol}://{Endpoint.Address.ToString()}";
            if (Endpoint.Port != defaultPort)
                url += $":{Endpoint.Port}";
            return url;
        }
    }
}
