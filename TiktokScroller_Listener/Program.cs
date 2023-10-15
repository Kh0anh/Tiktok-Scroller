using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TiktokScroller_Listener
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Configs.MainWindow = new Form1();
            Application.Run(Configs.MainWindow);
        }
    }
}
