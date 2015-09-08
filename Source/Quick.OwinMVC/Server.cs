using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Owin;
using Quick.OwinMVC.Resource;
using System.Net;
using Microsoft.Owin;
using Quick.OwinMVC.Middleware;
using Microsoft.Owin.Builder;
using Quick.OwinMVC.Utils;
using System.Security.Cryptography.X509Certificates;

namespace Quick.OwinMVC
{
    public class Server
    {
        internal static Server Instance { get; private set; }

        internal IDictionary<String, String> properties;
        internal IList<OwinMiddleware> middlewareInstanceList;

        private X509Certificate cert;
        private IPEndPoint endpoint;
        private String url;
        private IDisposable webApp;

        //中间件队列
        private List<Action<IAppBuilder>> middlewareRegisterActionList = new List<Action<IAppBuilder>>();

        static Server()
        {
            //注册resource:前缀URI处理程序
            WebRequest.RegisterPrefix("resource:", new ResourceWebRequestFactory());
        }

        /// <summary>
        /// 设置证书
        /// </summary>
        /// <param name="cert"></param>
        public void SetCertificate(X509Certificate cert)
        {
            this.cert = cert;
        }

        public String GetUrl()
        {
            var protocol = cert == null ? "http" : "https";
            var defaultPort = cert == null ? 80 : 443;

            if (url == null)
                url = $"{protocol}://{endpoint.Address.ToString()}";
            if (endpoint.Port != defaultPort)
                url += $":{endpoint.Port}";
            return url;
        }

        public Server(IDictionary<String, String> properties, Uri url) : this(properties, url.Port, url.Host) { }
        public Server(IDictionary<String, String> properties, int port) : this(properties, new IPEndPoint(IPAddress.Any, port)) { }
        public Server(IDictionary<String, String> properties, int port, string hostname)
        {
            IPEndPoint endpoint = null;
            switch (hostname)
            {
                case "*":
                case "+":
                case "0.0.0.0":
                    endpoint = new IPEndPoint(IPAddress.Any, port);
                    break;
                default:
                    endpoint = new IPEndPoint(Dns.GetHostAddresses(hostname).FirstOrDefault(), port);
                    break;
            }
            init(properties, endpoint);
        }

        public Server(IDictionary<String, String> properties, IPEndPoint endpoint)
        {
            init(properties, endpoint);
        }

        private void init(IDictionary<String, String> properties, IPEndPoint endpoint)
        {
            this.properties = properties;
            this.endpoint = endpoint;
            middlewareInstanceList = new List<OwinMiddleware>();

            Server.Instance = this;

            var prefix = this.GetType().FullName + ".";
            foreach (var key in properties.Keys.Where(t => t.StartsWith(prefix)))
            {
                var serverKey = key.Substring(prefix.Length);

                switch (serverKey)
                {
                    //如果是中间件定义
                    case "Middlewares":
                        var value = properties[key];
                        value.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList().ForEach(t => RegisterMiddleware(AssemblyUtils.GetType(t)));
                        break;
                    case "Cert":
                        var enableCert = bool.Parse(properties[key]);
                        if (!enableCert)
                            continue;
                        var certFile = properties[prefix + "Cert.File"];
                        var certPassword = properties[prefix + "Cert.Password"];
                        SetCertificate(new X509Certificate2(certFile, certPassword));
                        break;
                }
            }
        }

        /// <summary>
        /// 注册用户中间件
        /// </summary>
        /// <param name="middlewareClass"></param>
        /// <param name="args"></param>
        public void RegisterMiddleware(Type middlewareClass, params object[] args)
        {
            if (middlewareClass == null)
                throw new ArgumentNullException("Argument 'middlewareClass' must not be null.");
            Action<IAppBuilder> action = app =>
            {
                app.Use(middlewareClass, args);
            };
            middlewareRegisterActionList.Add(action);
        }

        public T GetMiddlewareInstance<T>()
            where T : OwinMiddleware
        {
            if (webApp == null)
                throw new ApplicationException("Can't invoke this method before Server.Start() method invoded.");
            Type type = typeof(T);
            return middlewareInstanceList.Where(m => m.GetType() == type).FirstOrDefault() as T;
        }

        public OwinMiddleware GetFirstMiddlewareInstance()
        {
            return middlewareInstanceList.First();
        }

        /// <summary>
        /// 注册用户中间件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void RegisterMiddleware<T>(params object[] args)
            where T : OwinMiddleware
        {
            RegisterMiddleware(typeof(T), args);
        }

        public void Start()
        {
            var app = new AppBuilder();

            //加载中部的中间件
            foreach (var register in middlewareRegisterActionList)
                register.Invoke(app);

            var builder = Nowin.ServerBuilder.New().SetEndPoint(endpoint).SetOwinApp(app.Build());
            //如果配置了证书，则设置证书
            if (cert != null)
                builder.SetCertificate(cert);
            webApp = builder.Start();
        }

        public void Stop()
        {
            webApp.Dispose();
        }
    }
}