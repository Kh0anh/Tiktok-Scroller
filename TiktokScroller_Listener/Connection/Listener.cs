using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TiktokScroller_Listener.Connection
{
    public static class Listener
    {
        private static TcpListener _Listener = null;
        public static void Start()
        {
            _Listener = new TcpListener(IPAddress.Any, Configs.PORT);
            _Listener.Start();

            Configs.MainWindow.AddLogs($"Listen {Configs.PORT}");
            
            Thread ListenThread = new Thread(() => { Listen(); });
            ListenThread.IsBackground = true;
            ListenThread.Start();
        }
        public static void Listen()
        {
            Socket socket = _Listener.AcceptSocket();
            Thread ClientThread = new Thread(() => { new Client(socket); });
            ClientThread.IsBackground = true;
            ClientThread.Start();
            Listen();
        }
    }
}