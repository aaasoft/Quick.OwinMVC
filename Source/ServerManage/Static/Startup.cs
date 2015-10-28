using Quick.OwinMVC;
using Quick.OwinMVC.Localization;
using Quick.OwinMVC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerManage.Static
{
    public class Startup
    {
        [TextResource]
        public enum Texts
        {
            [Text("服务器管理器的WEB服务已经开启，地址：{0}")]
            WEBSERVER_STARTED
        }

        public static void Start()
        {
            IDictionary<String, String> properties = PropertyUtils.LoadFile("app.properties");
            String httpUrl = properties["http.url"];
            Server server = new Server(properties, new Uri(httpUrl));
            //启动服务
            server.Start();
            Console.WriteLine(String.Format(TextManager.DefaultInstance.GetTextWithTail(Texts.WEBSERVER_STARTED), server.GetUrl()));
        }
    }
}
