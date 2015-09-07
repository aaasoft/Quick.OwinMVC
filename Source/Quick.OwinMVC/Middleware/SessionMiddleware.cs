using Microsoft.Owin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class SessionMiddleware : OwinMiddleware
    {
        public const String QUICK_OWINMVC_SESSION_EXPIRES_SECONDS_KEY = "Quick.OwinMVC.Session.expiresSeconds";
        public const String QUICK_OWINMVC_SESSION_CHECK_EXPIRES_PERIOD_KEY = "Quick.OwinMVC.Session.checkExpiresPeriodSecond";
        public const String QUICK_OWINMVC_SESSION_KEY = "Quick.OwinMVC.Session";
        private const String SESSION_ID_KEY = "sid";

        private IDictionary<String, String> properties;
        private Int32 expiresSeconds = 10 * 60;
        private Int32 checkExpiresPeriodSecond = 10;

        private static ConcurrentDictionary<String, SessionInfo> allSessionDict;
        private static Timer checkSessionExpiresTimer;

        private class SessionInfo : Dictionary<String, Object>, IDictionary<String, Object>
        {
            public String SessionId { get; private set; }

            public SessionInfo(string sessionId)
            {
                this.SessionId = sessionId;
            }

            /// <summary>
            /// Session过期时间
            /// </summary>
            public DateTime Expires { get; set; }
        }

        public SessionMiddleware(OwinMiddleware next) : base(next)
        {
            this.properties = Server.Instance.properties;
            if (properties.ContainsKey(QUICK_OWINMVC_SESSION_EXPIRES_SECONDS_KEY))
                expiresSeconds = Int32.Parse(properties[QUICK_OWINMVC_SESSION_EXPIRES_SECONDS_KEY]);
            if (properties.ContainsKey(QUICK_OWINMVC_SESSION_CHECK_EXPIRES_PERIOD_KEY))
                checkExpiresPeriodSecond = Int32.Parse(properties[QUICK_OWINMVC_SESSION_CHECK_EXPIRES_PERIOD_KEY]);

            allSessionDict = new ConcurrentDictionary<string, SessionInfo>();
            TimerCallback checkSessionExpiresAction = state =>
            {
                DateTime nowTime = DateTime.Now;
                foreach (var key in allSessionDict.Keys)
                {
                    SessionInfo session;
                    if (!allSessionDict.TryGetValue(key, out session))
                        continue;
                    if (nowTime > session.Expires)
                    {
                        allSessionDict.TryRemove(key, out session);
                    }
                }
            };
            checkSessionExpiresTimer = new Timer(checkSessionExpiresAction);
            checkSessionExpiresTimer.Change(0, checkExpiresPeriodSecond * 1000);
        }

        private void resetSessionExpires(SessionInfo session, ResponseCookieCollection cookies)
        {
            cookies.Append(SESSION_ID_KEY, session.SessionId, new CookieOptions() { Expires = DateTime.Now.ToUniversalTime().AddSeconds(expiresSeconds) });
            session.Expires = DateTime.Now.AddSeconds(expiresSeconds);
        }

        public override Task Invoke(IOwinContext context)
        {
            String sessionId = context.Request.Cookies.Where(t => t.Key == SESSION_ID_KEY).SingleOrDefault().Value;
            SessionInfo session = null;
            if (sessionId != null)
                allSessionDict.TryGetValue(sessionId, out session);
            if (session == null)
                sessionId = null;
            //如果没有Session，则创建Session
            if (sessionId == null)
            {
                sessionId = Guid.NewGuid().ToString().Replace("-", "");
                session = new SessionInfo(sessionId);
                allSessionDict.TryAdd(sessionId, session);
            }
            //重新设置Session的过期时间
            resetSessionExpires(session, context.Response.Cookies);
            context.Set<SessionInfo>(QUICK_OWINMVC_SESSION_KEY, session);
            return Next.Invoke(context);
        }
    }
}
