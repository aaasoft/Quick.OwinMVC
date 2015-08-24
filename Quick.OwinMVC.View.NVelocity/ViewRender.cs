using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;
using Quick.OwinMVC.View.NVelocity.ResourceLoaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.View.NVelocity
{
    public class ViewRender : IViewRender
    {
        private VelocityEngine engine;
        public ViewRender()
        {
            //初始化NVelocity引擎
            var properties = new Commons.Collections.ExtendedProperties();
            properties.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, "Views");
            properties.SetProperty(RuntimeConstants.RESOURCE_LOADER, "class");
            properties.SetProperty($"class.{RuntimeConstants.RESOURCE_LOADER}.class", $"{typeof(EmbedResourceLoader).FullName};{typeof(EmbedResourceLoader).Assembly.GetName().Name}");
            engine = new VelocityEngine(properties);
            engine.Init();
        }


        public string Render(string viewName, IDictionary<string, object> viewData)
        {
            //得到模板                
            Template template = engine.GetTemplate(viewName);
            String content = null;
            using (StringWriter writer = new StringWriter())
            {
                template.Merge(new ViewData(viewData), writer);
                content = writer.ToString();
            }
            return content;
        }


        private class ViewData : IContext
        {
            private IDictionary<string, object> data;

            public ViewData(IDictionary<String, Object> data)
            {
                this.data = data;
            }

            int IContext.Count { get { return data.Count; } }

            object[] IContext.Keys { get { return data.Keys.ToArray(); } }

            bool IContext.ContainsKey(object key)
            {
                return data.ContainsKey(key.ToString());
            }

            object IContext.Get(string key)
            {
                if (data.ContainsKey(key))
                    return data[key];
                return null;
            }

            object IContext.Put(string key, object value)
            {
                data[key] = value;
                return value;
            }

            object IContext.Remove(object key)
            {
                String strKey = key.ToString();
                object rtnObj = null;
                if (data.ContainsKey(strKey))
                {
                    rtnObj = data[strKey];
                    data.Remove(strKey);
                }
                return rtnObj;
            }
        }
    }
}
