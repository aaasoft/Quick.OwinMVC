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
using Quick.OwinMVC.Resource;
using Quick.OwinMVC.Localization;
using Quick.OwinMVC.Plugin;
using Quick.OwinMVC.Service;
using Owin.WebSocket.Extensions;

namespace Quick.OwinMVC
{
    public class Server : IPropertyHunter
    {
        internal static Server Instance { get; private set; }

        internal IDictionary<String, String> properties;
        //所有的中间件
        private IEnumerable<OwinMiddleware> Middlewares;

        private IPEndPoint endpoint;
        private String url;
        private IWebServer server;

        //WEB服务器转接器
        private String Wrapper;
        /// <summary>
        /// 上下文路径
        /// </summary>
        public string ContextPath { get; private set; }

        /// <summary>
        /// 上下文路径是否是根路径
        /// </summary>
        public bool IsRootContextPath
        {
            get { return string.IsNullOrEmpty(ContextPath) || ContextPath == "/"; }
        }

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
            if (!IsRootContextPath)
                url += ContextPath;
            return url;
        }

        static Server()
        {
            //注册resource:前缀URI处理程序
            WebRequest.RegisterPrefix("embed:", new EmbedWebRequestFactory());
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

            Server.Instance = this;
            HunterUtils.TryHunt(this, properties);
            server = (IWebServer)AssemblyUtils.CreateObject(Wrapper);
            HunterUtils.TryHunt(server, properties);
        }

        void IPropertyHunter.Hunt(string key, string value)
        {
            switch (key)
            {
                case nameof(ContextPath):
                    var contextPath = string.Empty;
                    if (string.IsNullOrEmpty(value)
                        || value == "/")
                        contextPath = "/";
                    else
                        contextPath = $"/{value}/";
                    ContextPath = contextPath;
                    break;
                case nameof(Middlewares):
                    value.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList().ForEach(t => RegisterMiddleware(AssemblyUtils.GetType(t)));
                    break;
                case nameof(Wrapper):
                    Wrapper = value;
                    break;
                case nameof(TextManager.DefaultLanguage):
                    TextManager.DefaultLanguage = value;
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
            return Middlewares.Where(m => m is T).Cast<T>();
        }

        public OwinMiddleware GetFirstMiddlewareInstance()
        {
            return Middlewares.First();
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
            //启动服务器
            server.Start(app =>
            {
                //注册WebSocket连接
                IDictionary<string, Type> webSocketConnectionDict = WebSocket.WebSocketManager.Instance.GetConnectionTypeDict();
                if (webSocketConnectionDict.Count > 0)
                {
                    var OwinExtensionType = typeof(OwinExtension);
                    var MapWebSocketRouteMethod = OwinExtensionType.GetMethod(nameof(OwinExtension.MapWebSocketRoute), new Type[] { typeof(IAppBuilder), typeof(string), typeof(Microsoft.Practices.ServiceLocation.IServiceLocator) });

                    foreach (var item in webSocketConnectionDict)
                    {
                        var route = item.Key;
                        var connectionType = item.Value;

                        var method = MapWebSocketRouteMethod.MakeGenericMethod(connectionType);
                        method.Invoke(null, new object[] { app, route, null });
                    }
                }
                //中间件上下文
                app.Use<MiddlewareContext>();
                //注册所有的中间件
                foreach (var register in middlewareRegisterActionList)
                    register.Invoke(app);
            }, endpoint);
            //初始化所有的中间件
            this.Middlewares = MiddlewareContext.Instance.Middlewares;
            HunterUtils.TryHunt(this.Middlewares, properties);
            MiddlewareContext.Instance.IsReady = true;
        }

        public void Stop()
        {
            server.Dispose();
        }
    }
}