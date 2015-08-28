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
        //头部中间件堆栈
        private Stack<Action<IAppBuilder>> headMiddlewareRegisterActionStack = new Stack<Action<IAppBuilder>>();
        //中部中间件堆栈
        private Stack<Action<IAppBuilder>> middleMiddlewareRegisterActionStack = new Stack<Action<IAppBuilder>>();
        //尾部中间件堆栈
        private Stack<Action<IAppBuilder>> tailMiddlewareRegisterActionStack = new Stack<Action<IAppBuilder>>();

        static Server()
        {
            //注册resource:前缀URI处理程序
            WebRequest.RegisterPrefix("resource:", new ResourceWebRequestFactory());
        }

        public Server(String url, IDictionary<String, String> properties)
        {
            this.properties = properties;
            this.url = url;

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
            webApp = WebApp.Start(url, app =>
            {
#if DEBUG
                app.UseErrorPage();
#endif
                //加载头部的中间件
                while (headMiddlewareRegisterActionStack.Count > 0)
                    headMiddlewareRegisterActionStack.Pop().Invoke(app);
                //加载中部的中间件
                while (middleMiddlewareRegisterActionStack.Count > 0)
                    middleMiddlewareRegisterActionStack.Pop().Invoke(app);
                //加载尾部的中间件
                while (tailMiddlewareRegisterActionStack.Count > 0)
                    tailMiddlewareRegisterActionStack.Pop().Invoke(app);
            }
            );
        }

        public void Stop()
        {
            webApp.Dispose();
        }
    }
}
