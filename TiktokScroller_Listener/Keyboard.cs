using TiktokScroller_Listener.Connection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TiktokScroller_Listener
{
    public static class Keyboard
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        private static bool isAKey = false;
        private static bool isBKey = false;

        private static IntPtr hookID = IntPtr.Zero;

        public static void Run()
        {
            hookID = SetHook(KeyboardHookCallback);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                //Configs.MainWindow.AddLogs($"Key {((Keys)vkCode)}");

                if (((Keys)vkCode).ToString() == Configs.AKey)
                {
                    Configs.MainWindow.AddLogs($"Send Next");
                    Thread sendToClient = new Thread(() =>
                    {
                        foreach (var client in Configs.Clients)
                        {
                            client.sendData("Next");
                        }
                    });
                    sendToClient.IsBackground = true;
                    sendToClient.Start();
                }
                if (((Keys)vkCode).ToString() == Configs.BKey)
                {
                    Configs.MainWindow.AddLogs($"Send Like");
                    Thread sendToClient = new Thread(() =>
                    {
                        foreach (var client in Configs.Clients)
                        {
                            client.sendData("Like");
                        }
                    });
                    sendToClient.IsBackground = true;
                    sendToClient.Start();
                }

                //Thread R = new Thread(() =>
                //{
                //    if (((Keys)vkCode).ToString() == Configs.AKey)
                //    {
                //        isAKey = true;
                //        if (isBKey && isAKey)
                //        {
                //            Configs.MainWindow.AddLogs($"Send Trash");
                //            Thread sendToClient = new Thread(() =>
                //            {
                //                foreach (var client in Configs.Clients)
                //                {
                //                    client.sendData("Trash");
                //                }
                //            });
                //            sendToClient.IsBackground = true;
                //            sendToClient.Start();

                //            isBKey = false;
                //            isAKey = false;
                //        }
                //        else
                //        {
                //            Thread.Sleep(500);
                //            if (isAKey)
                //            {
                //                Configs.MainWindow.AddLogs($"Send Next");
                //                Thread sendToClient = new Thread(() =>
                //                {
                //                    foreach (var client in Configs.Clients)
                //                    {
                //                        client.sendData("Next");
                //                    }
                //                });
                //                sendToClient.IsBackground = true;
                //                sendToClient.Start();
                //                isAKey = false;
                //            }
                //        }
                //    }
                //    if (((Keys)vkCode).ToString() == Configs.BKey)
                //    {
                //        isBKey = true;
                //        if (isBKey && isAKey)
                //        {
                //            Configs.MainWindow.AddLogs($"Send Trash");
                //            Thread sendToClient = new Thread(() =>
                //            {
                //                foreach (var client in Configs.Clients)
                //                {
                //                    client.sendData("Trash");
                //                }
                //            });
                //            sendToClient.IsBackground = true;
                //            sendToClient.Start();

                //            isBKey = false;
                //            isAKey = false;
                //        }
                //        else
                //        {
                //            Thread.Sleep(500);
                //            if (isBKey)
                //            {
                //                Configs.MainWindow.AddLogs($"Send Like");
                //                Thread sendToClient = new Thread(() =>
                //                {
                //                    foreach (var client in Configs.Clients)
                //                    {
                //                        client.sendData("Like");
                //                    }
                //                });
                //                sendToClient.IsBackground = true;
                //                sendToClient.Start();
                //                isBKey = false;
                //            }
                //        }
                //    }
                //});
                //R.IsBackground = true;
                //R.Start();
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
