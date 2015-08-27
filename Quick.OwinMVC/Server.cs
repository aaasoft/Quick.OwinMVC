using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Owin;
using Microsoft.Owin.Hosting;
using Quick.OwinMVC.View;
using Quick.OwinMVC.Resource;
using System.Net;
using Quick.OwinMVC.Utils;
using Microsoft.Owin;
using Quick.OwinMVC.Middleware;

namespace Quick.OwinMVC
{
    public class Server
    {
        private IDictionary<String, String> properties;
        private String url;
        private IDictionary<String, String> redirectDict;
        private IDictionary<String, String> rewriteDict;
        private IDisposable webApp;
        private Stack<Action<IAppBuilder>> middlewareRegisterActionStack = new Stack<Action<IAppBuilder>>();

        static Server()
        {
            //注册embed:前缀URI处理程序
            WebRequest.RegisterPrefix("embed:", new EmbedWebRequestFactory());
            //注册resource:前缀URI处理程序
            WebRequest.RegisterPrefix("resource:", new ResourceWebRequestFactory());
        }

        public Server(String url, IDictionary<String, String> properties)
        {
            this.properties = properties;
            this.url = url;

            redirectDict = new Dictionary<String, String>();
            rewriteDict = new Dictionary<String, String>();

            //注册Quick.OwinMVC中间件
            RegisterMiddleware<MvcMiddleware>(properties);
            //注册URL重定向中间件
            RegisterMiddleware<RedirectMiddleware>(redirectDict);
            //注册URL重写中间件
            RegisterMiddleware<RewriteMiddleware>(rewriteDict);
        }

        /// <summary>
        /// 注册中间件
        /// </summary>
        /// <param name="middlewareClass"></param>
        /// <param name="args"></param>
        public void RegisterMiddleware(Type middlewareClass, params object[] args)
        {
            Action<IAppBuilder> action = app =>
            {
                app.Use(middlewareClass, args);
            };
            middlewareRegisterActionStack.Push(action);
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

        /// <summary>
        /// 注册中间件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public void RegisterMiddleware<T>(params object[] args)
            where T : OwinMiddleware
        {
            Action<IAppBuilder> action = app =>
             {
                 app.Use<T>(args);
             };
            middlewareRegisterActionStack.Push(action);
        }

        public void Start()
        {
            webApp = WebApp.Start(url, app =>
            {
#if DEBUG
                app.UseErrorPage();
#endif
                //加载所有的中间件
                while (middlewareRegisterActionStack.Count > 0)
                {
                    middlewareRegisterActionStack.Pop().Invoke(app);
                }
            }
            );
        }

        public void Stop()
        {
            webApp.Dispose();
        }
    }
}
