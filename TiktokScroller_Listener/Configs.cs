using TiktokScroller_Listener.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TiktokScroller_Listener
{
    public static class Configs
    {
        public static int PORT = 8572;

        public static string AKey = "RShiftKey";
        public static string BKey = "RControlKey";

        public static Form1 MainWindow;

        public static List<Client> Clients = new List<Client>();
    }
}
