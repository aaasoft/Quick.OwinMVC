using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Quick.OwinMVC.Hunter
{
    public class HunterUtils
    {
        private static List<Assembly> assemblyList;
        private static List<Type> typeList;

        public static List<Assembly> GetAssemblyList()
        {
            lock (typeof(HunterUtils))
            {
                if (assemblyList == null)
                    assemblyList = AppDomain.CurrentDomain.GetAssemblies().Where(t => !t.IsDynamic && !t.GlobalAssemblyCache).ToList();
                return assemblyList;
            }
        }

        public static List<Type> GetTypeList()
        {
            lock (typeof(HunterUtils))
            {
                if (typeList == null)
                {
                    typeList = new List<Type>();
                    foreach (Type[] assTypes in assemblyList.Select(t => t.GetTypes()))
                        typeList.AddRange(assTypes);
                }
                return typeList;
            }
        }

        public static void TryHunt(Object obj, IDictionary<String, String> properties)
        {
            if (obj is IHungryPropertyHunter)
            {
                IHungryPropertyHunter hunter = (IHungryPropertyHunter)obj;
                hunter.Hunt(properties);
            }
            if (obj is IPropertyHunter)
            {
                IPropertyHunter hunter = (IPropertyHunter)obj;
                var prefix = hunter.GetType().FullName + ".";
                foreach (String key in properties.Keys.Where(t => t.StartsWith(prefix)))
                    hunter.Hunt(key.Substring(prefix.Length), properties[key]);
            }
            if (obj is IAssemblyHunter)
            {
                IAssemblyHunter hunter = (IAssemblyHunter)obj;
                GetAssemblyList().ForEach(t => hunter.Hunt(t));
            }
            if (obj is ITypeHunter)
            {
                ITypeHunter hunter = (ITypeHunter)obj;
                GetTypeList().ForEach(t => hunter.Hunt(t));
            }
        }

        public static void TryHunt(IEnumerable objs, IDictionary<String, String> properties)
        {
            foreach (var obj in objs)
                TryHunt(obj, properties);
        }
    }
}
