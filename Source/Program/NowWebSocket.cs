using Owin.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Quick.OwinMVC.Program
{
    public class NowWebSocket : WebSocketConnection
    {
        public override void OnOpen()
        {
            SendText(Encoding.UTF8.GetBytes(DateTime.Now.ToString()), true);
        }

        public override Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type)
        {
            var req = Context.Request;
            Console.WriteLine("Path:" + req.Uri);
            return SendText(Encoding.UTF8.GetBytes(DateTime.Now.ToString()), true);
        }
    }
}
