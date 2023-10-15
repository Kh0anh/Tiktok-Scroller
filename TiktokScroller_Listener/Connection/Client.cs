using System;
using System.Net.Sockets;
using System.Text;

namespace TiktokScroller_Listener.Connection
{
    public class Client
    {
        private Socket TcpClient;
        private byte[] Buffer;
        private bool isAuth = false;
        public Client(Socket TcpClient)
        {
            this.TcpClient = TcpClient;
            Buffer = new byte[1024];
            Handshake();
        }
        public void Handshake()
        {
            try
            {
                int bytesRead = TcpClient.Receive(Buffer);
                string Message = Encoding.UTF8.GetString(Buffer, 0, bytesRead);

                if (Message.Contains("Sec-WebSocket-Key:"))
                {
                    byte[] HandshakeMsg = Encoding.UTF8.GetBytes($"HTTP/1.1 101 Switching Protocols\r\nUpgrade: websocket\r\nConnection: Upgrade\r\nSec-WebSocket-Accept: {Common.ComputeWebSocketHandshakeResponse(Message)}\r\n\r\n");
                    TcpClient.Send(HandshakeMsg);

                    bytesRead = TcpClient.Receive(Buffer);
                    byte[] receivedData = new byte[bytesRead];
                    Array.Copy(Buffer, receivedData, bytesRead);
                    if(Common.DecodeWebSocketMessage(receivedData) == "TiktokScroll")
                    {
                        isAuth = true;
                        Configs.Clients.Add(this);
                        Configs.MainWindow.AddLogs($"New connect {Configs.Clients.Count}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
            catch
            {
                Disconnect();
            }
        }
        public void sendData(string message)
        {
            try
            {
                TcpClient.Send(Common.EncodeWebSocketMessage(message));
            }
            catch { Disconnect(); }
        }
        public void Disconnect()
        {
            try
            {
                if (isAuth)
                {
                    Configs.MainWindow.AddLogs($"Remove Client {Configs.Clients.Count}");
                    Configs.Clients.Remove(this);
                }
                TcpClient.Dispose();
            }
            catch { }
        }
    }
}
