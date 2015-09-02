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

namespace Quick.OwinMVC
{
    public class Server
    {
        public const String MIDDLEWARE_PREFIX = "Quick.OwinMVC.Server.Middleware.";

        internal static Server Instance { get; private set; }

        internal IDictionary<String, String> properties;
        internal IDictionary<String, String> redirectDict;
        internal IDictionary<String, String> rewriteDict;

        private EndPoint endpoint;
        private String url;
        private IDisposable webApp;

        //中间件队列
        private List<Action<IAppBuilder>> middlewareRegisterActionList = new List<Action<IAppBuilder>>();

        static Server()
        {
            //注册resource:前缀URI处理程序
            WebRequest.RegisterPrefix("resource:", new ResourceWebRequestFactory());
        }

        public String GetUrl()
        {
            if (url == null)
                url = $"http://{endpoint.ToString()}";
            return url;
        }

        public Server(IDictionary<String, String> properties, Uri url) : this(properties, url.Port, url.Host) { }
        public Server(IDictionary<String, String> properties, int port) : this(properties, new IPEndPoint(IPAddress.Any, port)) { }
        public Server(IDictionary<String, String> properties, int port, string hostname)
        {
            url = $"http://{hostname}:{port}";
            EndPoint endpoint = null;
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

        public Server(IDictionary<String, String> properties, EndPoint endpoint)
        {
            init(properties, endpoint);
        }

        private void init(IDictionary<String, String> properties, EndPoint endpoint)
        {
            this.properties = properties;
            this.endpoint = endpoint;

            Server.Instance = this;

            redirectDict = new Dictionary<String, String>();
            rewriteDict = new Dictionary<String, String>();


            foreach (var key in properties.Keys)
            {
                if (key.StartsWith(MIDDLEWARE_PREFIX))
                {
                    RegisterMiddleware(AssemblyUtils.GetType(properties[key]));
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

        /// <summary>
        /// 注册重定向
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="desPath"></param>
        public void RegisterRedirect(String srcPath, String desPath)
        {
            redirectDict[srcPath] = desPath;
        }

        /// <summary>
        /// 注册重写
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="desPath"></param>
        public void RegisterRewrite(String srcPath, String desPath)
        {
            rewriteDict[srcPath] = desPath;
        }

        public void Start()
        {
            var app = new AppBuilder();

            //加载中部的中间件
            foreach (var register in middlewareRegisterActionList)
                register.Invoke(app);

            webApp = new Firefly.Http.ServerFactory().Create(app.Build(), endpoint);
        }

        public void Stop()
        {
            webApp.Dispose();
        }
    }
}