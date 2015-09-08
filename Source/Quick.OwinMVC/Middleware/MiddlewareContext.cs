using Microsoft.Owin;
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
        private Server server;

        public MiddlewareContext(OwinMiddleware next) : base(next)
        {
            server = Server.Instance;

            List<IPropertyHunter> propertyHunterList = new List<IPropertyHunter>();
            List<IAssemblyHunter> assemblyHunterList = new List<IAssemblyHunter>();
            List<ITypeHunter> typeHunterList = new List<ITypeHunter>();

            var nextProperty = typeof(OwinMiddleware).GetProperty("Next", BindingFlags.Instance | BindingFlags.NonPublic);
            var currentMiddleware = next;
            while (currentMiddleware != null)
            {
                server.middlewareInstanceList.Add(currentMiddleware);
                if (currentMiddleware is IPropertyHunter)
                    propertyHunterList.Add((IPropertyHunter)currentMiddleware);
                if (currentMiddleware is IAssemblyHunter)
                    assemblyHunterList.Add((IAssemblyHunter)currentMiddleware);
                if (currentMiddleware is ITypeHunter)
                    typeHunterList.Add((ITypeHunter)currentMiddleware);
                currentMiddleware = nextProperty.GetValue(currentMiddleware, null) as OwinMiddleware;
            }

            //扫描配置
            propertyHunterList.ForEach(hunter =>
            {
                var prefix = hunter.GetType().FullName + ".";
                foreach (String key in server.properties.Keys.Where(t => t.StartsWith(prefix)))
                    hunter.Hunt(key.Substring(prefix.Length), server.properties[key]);
            });
            //扫描程序集和类
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(t => !t.IsDynamic && !t.GlobalAssemblyCache))
            {
                assemblyHunterList.ForEach(t => t.Hunt(assembly));
                foreach (Type type in assembly.GetTypes())
                {
                    typeHunterList.ForEach(t => t.Hunt(assembly, type));
                }
            }
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
