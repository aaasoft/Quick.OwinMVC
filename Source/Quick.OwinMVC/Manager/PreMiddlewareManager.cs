using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using Quick.OwinMVC.Middleware;
using System.Text;
using System.Threading.Tasks;
using Quick.OwinMVC.Hunter;

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

        private OwinMiddleware _TailMiddleware;
        public OwinMiddleware TailMiddleware
        {
            get { return _TailMiddleware; }
            set
            {
                _TailMiddleware = value;
                OwinMiddleware preMiddleware = null;
                foreach (var middleware in GetItems())
                {

                    if (Server.Instance!=null && Server.Instance.properties != null)
                        HunterUtils.TryHunt(middleware, Server.Instance.properties);

                    if (preMiddleware != null)
                    {
                        preMiddleware.SetNext(middleware);
                    }
                    preMiddleware = middleware;
                }
                if (preMiddleware != null)
                    preMiddleware.SetNext(TailMiddleware);
            }
        }

        public override void Register(OwinMiddleware item)
        {
            if (Server.Instance!=null && Server.Instance.properties != null)
                HunterUtils.TryHunt(item, Server.Instance.properties);

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
