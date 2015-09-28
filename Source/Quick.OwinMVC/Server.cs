using System;
using System.Collections.Generic;
using System.Linq;
using Owin;
using System.Net;
using Microsoft.Owin;
using Quick.OwinMVC.Middleware;
using Microsoft.Owin.Builder;
using Quick.OwinMVC.Utils;
using System.Security.Cryptography.X509Certificates;
using Quick.OwinMVC.WebServer;
using Quick.OwinMVC.Hunter;

namespace Quick.OwinMVC
{
    public class Server : IPropertyHunter
    {
        internal static Server Instance { get; private set; }

        internal IDictionary<String, String> properties;
        internal IList<OwinMiddleware> middlewareInstanceList;

        private IPEndPoint endpoint;
        private String url;
        private IWebServer server;
        //WEB服务器转接器
        private String Wrapper;
        //中间件队列
        private List<Action<IAppBuilder>> middlewareRegisterActionList = new List<Action<IAppBuilder>>();

        public String GetUrl()
        {
            var protocol = "http";
            var defaultPort = 80;

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
            HunterUtils.TryHunt(this, properties);
            server = (IWebServer)AssemblyUtils.CreateObject(Wrapper);
            HunterUtils.TryHunt(server, properties);
        }

        void IPropertyHunter.Hunt(string key, string value)
        {
            switch (key)
            {
                case "Middlewares":
                    value.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList().ForEach(t => RegisterMiddleware(AssemblyUtils.GetType(t)));
                    break;
                case nameof(Wrapper):
                    Wrapper = value;
                    break;
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

        public T GetMiddleware<T>()
        {
            return GetMiddlewares<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetMiddlewares<T>()
        {
            if (!server.IsRuning)
                throw new ApplicationException("Can't invoke this method before Server.Start() method invoded.");
            return middlewareInstanceList.Where(m => m is T).Cast<T>();
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

            //中间件上下文
            app.Use<MiddlewareContext>();
            //加载中部的中间件
            foreach (var register in middlewareRegisterActionList)
                register.Invoke(app);

            server.Start(app.Build(), endpoint);
        }

        public void Stop()
        {
            server.Dispose();
        }
    }
}