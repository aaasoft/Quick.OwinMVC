using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using Quick.OwinMVC.Middleware;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Manager
{
    public class PreMiddlewareManager : AbstractManager<PreMiddlewareManager, OwinMiddleware>
    {
        public OwinMiddleware HeadMiddleware
        {
            get
            {
                return GetItems().FirstOrDefault() ?? TailMiddleware;
            }
        }
        public OwinMiddleware TailMiddleware { get; set; }

        public override void Register(OwinMiddleware item)
        {
            var preLastMiddleware = GetItems().LastOrDefault();
            base.Register(item);
            if (preLastMiddleware != null)
                preLastMiddleware.SetNext(item);
            item.SetNext(TailMiddleware);
        }

        public override void Unregister(OwinMiddleware item)
        {
            base.Unregister(item);
            var preLastMiddleware = GetItems().LastOrDefault();
            if (preLastMiddleware != null)
                preLastMiddleware.SetNext(TailMiddleware);
        }
    }
}
