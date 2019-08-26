using Owin.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace SvnManage.WebSockets
{
    public class NowTime : WebSocketConnection
    {
        public override void OnOpen()
        {
            base.OnOpen();
            SendText(Encoding.UTF8.GetBytes("Welcome." + this.GetHashCode()), true);
            SendText(Encoding.UTF8.GetBytes(DateTime.Now.ToString()), true);
        }

        public override Task OnMessageReceived(ArraySegment<byte> message, WebSocketMessageType type)
        {
            return SendText(Encoding.UTF8.GetBytes(DateTime.Now.ToString()), true);
        }
    }
}
