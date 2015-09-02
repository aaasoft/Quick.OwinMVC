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
        public MiddlewareContext(OwinMiddleware next) : base(next)
        {
            List<IPropertyHunter> propertyHunterList = new List<IPropertyHunter>();
            List<IAssemblyHunter> assemblyHunterList = new List<IAssemblyHunter>();
            List<ITypeHunter> typeHunterList = new List<ITypeHunter>();

            var nextProperty = typeof(OwinMiddleware).GetProperty("Next", BindingFlags.Instance | BindingFlags.NonPublic);
            var currentMiddleware = next;
            while (currentMiddleware != null)
            {
                Server.Instance.middlewareInstanceDict[currentMiddleware.GetType()] = currentMiddleware;
                if (currentMiddleware is IPropertyHunter)
                    propertyHunterList.Add((IPropertyHunter)currentMiddleware);
                if (currentMiddleware is IAssemblyHunter)
                    assemblyHunterList.Add((IAssemblyHunter)currentMiddleware);
                if (currentMiddleware is ITypeHunter)
                    typeHunterList.Add((ITypeHunter)currentMiddleware);
                currentMiddleware = nextProperty.GetValue(currentMiddleware, null) as OwinMiddleware;
            }

            //扫描配置
            foreach (String key in Server.Instance.properties.Keys)
                propertyHunterList.ForEach(t => t.Hunt(key, Server.Instance.properties[key]));
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
            return Next.Invoke(context);
        }
    }
}
