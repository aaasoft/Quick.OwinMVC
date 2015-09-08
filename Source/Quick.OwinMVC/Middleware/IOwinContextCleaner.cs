using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.OwinMVC.Middleware
{
    public interface IOwinContextCleaner
    {
        void Clean(IOwinContext context);
    }
}
