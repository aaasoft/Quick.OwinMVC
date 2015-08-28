using Microsoft.Owin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Threading;

namespace Quick.OwinMVC.Controller
{
    public static class IOwinContextSessionExtension
    {
        public const String QUICK_OWINMVC_SESSION_EXPIRES_SECONDS_KEY = "Quick.OwinMVC.Session.expiresSeconds";
        public const String QUICK_OWINMVC_SESSION_CHECK_EXPIRES_PERIOD_KEY = "Quick.OwinMVC.Session.checkExpiresPeriodSecond";

        private static Int32 expiresSeconds = 10 * 60;
        private static Int32 checkExpiresPeriodSecond = 10;
        
        private const String SESSION_ID_KEY = "sid";
        private static ConcurrentDictionary<String, SessionInfo> allSessionDict;
        private static Timer checkSessionExpiresTimer;

        static IOwinContextSessionExtension()
        {
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
            checkSessionExpiresTimer.Change(0, checkExpiresPeriodSecond);
        }

        public class SessionInfo : Dictionary<String, Object>, IDictionary<String, Object>
        {
            public  String SessionId { get; private set; }

            public SessionInfo(string sessionId)
            {
                this.SessionId = sessionId;
            }

            /// <summary>
            /// Session过期时间
            /// </summary>
            public DateTime Expires { get; set; }
        }

        /// <summary>
        /// 得到Session信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IDictionary<String, Object> GetSession(this IOwinContext context)
        {
            String sessionId = context.Request.Cookies.Where(t => t.Key == SESSION_ID_KEY).SingleOrDefault().Value;
            SessionInfo session = null;
            if (sessionId != null)
                allSessionDict.TryGetValue(sessionId, out session);
            if (session == null)
                sessionId = null;

            if (sessionId == null)
            {
                sessionId = Guid.NewGuid().ToString();
                session = new SessionInfo(sessionId);
                allSessionDict.TryAdd(sessionId, session);
            }
            resetSessionExpires(session, context.Response.Cookies);
            return session;
        }

        private static void resetSessionExpires(SessionInfo session, ResponseCookieCollection cookies)
        {
            cookies.Append(SESSION_ID_KEY, session.SessionId, new CookieOptions() { Expires = DateTime.Now.AddSeconds(expiresSeconds) });
            session.Expires = DateTime.Now.AddSeconds(expiresSeconds);
        }

        /// <summary>
        /// 获取POST提交的表单数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IFormCollection GetFormData(this IOwinContext context)
        {
            return context.Request.ReadFormAsync().Result;
        }
    }
}
