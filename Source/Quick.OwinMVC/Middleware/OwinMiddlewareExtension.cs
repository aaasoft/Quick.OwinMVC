using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public static class OwinMiddlewareExtension
    {
        private static PropertyInfo nextProperty = typeof(OwinMiddleware).GetProperty("Next", BindingFlags.Instance | BindingFlags.NonPublic);
        public static void SetNext(this OwinMiddleware middleware, OwinMiddleware next)
        {
            nextProperty.SetValue(middleware, next);
        }
    }
}
