﻿using Microsoft.Owin;
using Quick.OwinMVC.Hunter;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Middleware
{
    public class SessionMiddleware : OwinMiddleware, IPropertyHunter
    {
        private static readonly String SESSION_KEY = typeof(SessionMiddleware).FullName + ".SESSION_KEY";

        //Session的ID键
        private String IdKey = "sid";
        //Session过期时间，单位：秒。默认为10分钟
        private Int32 Expires = 10 * 60;
        //检查过期Session间隔，单位：秒。默认为10秒
        private Int32 CheckExpirePeriods = 10;

        private static ConcurrentDictionary<String, SessionInfo> allSessionDict;
        private static Timer checkSessionExpiresTimer;

        internal class SessionInfo : Dictionary<String, Object>, IDictionary<String, Object>
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

        public void Hunt(string key, string value)
        {
            switch (key)
            {
                case "IdKey":
                    IdKey = value;
                    break;
                case "Expires":
                    Expires = Int32.Parse(value);
                    break;
                case "CheckExpirePeriods":
                    CheckExpirePeriods = Int32.Parse(value);
                    break;
            }
        }

        public static string GetSessionId(IOwinContext context)
        {
            var sessionInfo = context.Get<SessionInfo>(SESSION_KEY);
            if(sessionInfo!=null)
                return sessionInfo.SessionId;
            return null;
        }

        public static IDictionary<String, Object> GetSession(IOwinContext context)
        {
            return context.Get<IDictionary<String, Object>>(SESSION_KEY);
        }

        public SessionMiddleware(OwinMiddleware next) : base(next)
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
            checkSessionExpiresTimer.Change(0, CheckExpirePeriods * 1000);
        }

        public override Task Invoke(IOwinContext context)
        {
            //先从URL参数中获取
            var sessionId  = context.Request.Query.Get("SessionId");
            //然后从Cookie中获取
            if (string.IsNullOrEmpty(sessionId))
                sessionId = context.Request.Cookies.Where(t => t.Key == IdKey).SingleOrDefault().Value;
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
            session.Expires = DateTime.Now.AddSeconds(Expires);
            //设置Cookie
            context.Response.Cookies.Append(IdKey, session.SessionId);

            context.Set<SessionInfo>(SESSION_KEY, session);
            return Next.Invoke(context);
        }
    }
}
