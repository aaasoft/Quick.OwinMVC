using Microsoft.Owin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Controller
{
    public static class IOwinContextSessionExtension
    {
        private const Int32 SESSION_EXPIRES_SECONDS = 10 * 60;
        private const String SESSION_ID_KEY = "sid";

        private static ConcurrentDictionary<String, IDictionary<String, Object>> allSessionDict;

        static IOwinContextSessionExtension()
        {
            allSessionDict = new ConcurrentDictionary<string, IDictionary<string, object>>();
        }

        /// <summary>
        /// 得到Session信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IDictionary<String, Object> GetSession(this IOwinContext context)
        {
            String sessionId = context.Request.Cookies.Where(t => t.Key == SESSION_ID_KEY).SingleOrDefault().Value;
            IDictionary<String, Object> sessionDict = null;
            if (sessionId != null)
                allSessionDict.TryGetValue(sessionId, out sessionDict);
            if (sessionDict == null)
                sessionId = null;

            if (sessionId == null)
            {
                sessionId = Guid.NewGuid().ToString();
                sessionDict = new ExpandoObject();
                allSessionDict.TryAdd(sessionId, sessionDict);
                context.Response.Cookies.Append(SESSION_ID_KEY, sessionId, new CookieOptions() { Expires = DateTime.Now.AddSeconds(SESSION_EXPIRES_SECONDS) });
            }
            return sessionDict;
        }
    }
}
