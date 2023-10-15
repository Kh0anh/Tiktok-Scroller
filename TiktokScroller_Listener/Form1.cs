using TiktokScroller_Listener.Connection;
using TiktokScroller_Listener.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TiktokScroller_Listener
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Listener.Start();
            Keyboard.Run();
        }
        public void AddLogs(string text)
        {
            Invoke((MethodInvoker)(() =>
            {
                LogsTextBox.AppendText($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {text}" + Environment.NewLine);
            }));
        }
    }
}
