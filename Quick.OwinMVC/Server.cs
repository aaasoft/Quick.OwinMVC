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

namespace Quick.OwinMVC
{
    public class Server
    {
        private IDictionary<String, String> properties;
        private EndPoint endpoint;
        private String url;

        private IDictionary<String, String> redirectDict;
        private IDictionary<String, String> rewriteDict;
        private IDisposable webApp;
        //头部中间件堆栈
        private Stack<Action<IAppBuilder>> headMiddlewareRegisterActionStack = new Stack<Action<IAppBuilder>>();
        //中部中间件堆栈
        private Stack<Action<IAppBuilder>> middleMiddlewareRegisterActionStack = new Stack<Action<IAppBuilder>>();
        //尾部中间件堆栈
        private Stack<Action<IAppBuilder>> tailMiddlewareRegisterActionStack = new Stack<Action<IAppBuilder>>();

        /// <summary>
        /// 是否输出错误信息到响应中
        /// </summary>
        public Boolean OutputErrorToResponse { get; set; } = false;

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

            redirectDict = new Dictionary<String, String>();
            rewriteDict = new Dictionary<String, String>();

            //注册Session中间件
            registerMiddleware<SessionMiddleware>(headMiddlewareRegisterActionStack, properties);

            //注册Quick.OwinMVC中间件
            registerMiddleware<MvcMiddleware>(tailMiddlewareRegisterActionStack, properties);
            //注册URL重定向中间件
            registerMiddleware<RedirectMiddleware>(tailMiddlewareRegisterActionStack, redirectDict);
            //注册URL重写中间件
            registerMiddleware<RewriteMiddleware>(tailMiddlewareRegisterActionStack, rewriteDict);
        }

        private void registerMiddleware(Stack<Action<IAppBuilder>> middlewareRegisterActionStack, Type middlewareClass, params object[] args)
        {
            Action<IAppBuilder> action = app =>
            {
                app.Use(middlewareClass, args);
            };
            middlewareRegisterActionStack.Push(action);
        }
        private void registerMiddleware<T>(Stack<Action<IAppBuilder>> middlewareRegisterActionStack, params object[] args)
            where T : OwinMiddleware
        {
            registerMiddleware(middlewareRegisterActionStack, typeof(T), args);
        }

        /// <summary>
        /// 注册用户中间件
        /// </summary>
        /// <param name="middlewareClass"></param>
        /// <param name="args"></param>
        public void RegisterMiddleware(Type middlewareClass, params object[] args)
        {
            registerMiddleware(middleMiddlewareRegisterActionStack, middlewareClass, args);
        }

        /// <summary>
        /// 注册用户中间件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void RegisterMiddleware<T>(params object[] args)
            where T : OwinMiddleware
        {
            registerMiddleware<T>(middleMiddlewareRegisterActionStack, args);
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

            if (OutputErrorToResponse)
                app.Use<ErrorMiddleware>();

            //加载头部的中间件
            while (headMiddlewareRegisterActionStack.Count > 0)
                headMiddlewareRegisterActionStack.Pop().Invoke(app);
            //加载中部的中间件
            while (middleMiddlewareRegisterActionStack.Count > 0)
                middleMiddlewareRegisterActionStack.Pop().Invoke(app);
            //加载尾部的中间件
            while (tailMiddlewareRegisterActionStack.Count > 0)
                tailMiddlewareRegisterActionStack.Pop().Invoke(app);
            webApp = new Firefly.Http.ServerFactory().Create(app.Build(), endpoint);
        }

        public void Stop()
        {
            webApp.Dispose();
        }
    }
}