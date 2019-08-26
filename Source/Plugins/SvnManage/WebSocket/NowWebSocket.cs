using Owin.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace SvnManage.WebSocket
{
    public class NowWebSocket : WebSocketConnection
    {
        public override void OnOpen()
        {
            SendText(Encoding.UTF8.GetBytes(DateTime.Now.ToString()), true);
        }

        public override Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type)
        {
            return SendText(Encoding.UTF8.GetBytes(DateTime.Now.ToString()), true);
        }
    }
}
