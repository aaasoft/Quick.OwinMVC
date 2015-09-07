using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Quick.OwinMVC.Middleware
{
    public interface IAssemblyHunter
    {
        void Hunt(Assembly assembly);
    }
}
