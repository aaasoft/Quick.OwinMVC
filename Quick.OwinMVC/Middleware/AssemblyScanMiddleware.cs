using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class AssemblyScanMiddleware : OwinMiddleware
    {
        public AssemblyScanMiddleware(OwinMiddleware next) : base(next)
        {
            List<IAssemblyHunter> assemblyHunterList = new List<IAssemblyHunter>();
            List<ITypeHunter> typeHunterList = new List<ITypeHunter>();


            var nextProperty = typeof(OwinMiddleware).GetProperty("Next", BindingFlags.Instance | BindingFlags.NonPublic);
            var currentMiddleware = next;
            while (currentMiddleware != null)
            {
                if (currentMiddleware is IAssemblyHunter)
                    assemblyHunterList.Add((IAssemblyHunter)currentMiddleware);
                if (currentMiddleware is ITypeHunter)
                    typeHunterList.Add((ITypeHunter)currentMiddleware);
                currentMiddleware = nextProperty.GetValue(currentMiddleware, null) as OwinMiddleware;
            }

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
