﻿using Microsoft.Owin;
using Quick.OwinMVC.Hunter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class MiddlewareContext : OwinMiddleware
    {
        public static MiddlewareContext Instance { get; private set; }
        /// <summary>
        /// 所有中间件对象列表
        /// </summary>
        public IEnumerable<OwinMiddleware> Middlewares { get; private set; }

        public MiddlewareContext(OwinMiddleware next) : base(next)
        {
            Instance = this;
            var list = new List<OwinMiddleware>();

            var nextProperty = typeof(OwinMiddleware).GetProperty("Next", BindingFlags.Instance | BindingFlags.NonPublic);
            var currentMiddleware = next;
            while (currentMiddleware != null)
            {
                list.Add(currentMiddleware);
                currentMiddleware = nextProperty.GetValue(currentMiddleware, null) as OwinMiddleware;
            }
            Middlewares = list;
        }

        public override Task Invoke(IOwinContext context)
        {
            String path = context.Get<String>("owin.RequestPath");
            //设置原始请求路径
            context.Set("Quick.OwinMVC.SourceRequestPath", path);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < path.Split('/').Length - 2; i++)
            {
                if (i > 0)
                    sb.Append("/");
                sb.Append("..");
            }
            var contextPath = sb.ToString();
            if (String.IsNullOrEmpty(contextPath))
                contextPath = ".";

            context.Set("ContextPath", contextPath);
            return Next.Invoke(context);
        }
    }
}
