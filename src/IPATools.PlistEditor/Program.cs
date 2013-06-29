using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IPATools.PlistEditor
{
    static class Program
    {
        public const string TITLE = "IPATools PropertyList Editor";
        static Mutex mutex = new Mutex(true, "17e9b227-2cb5-4ccf-bae0-b0a0ba7127f0");
        [STAThread]
        static void Main(params string[] args)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain(args)); 
                mutex.ReleaseMutex();
            }
            else
            {
                if (args != null && args.Length > 0)
                {
                    IntPtr hWnd = Win32API.FindWindow(null, TITLE);
                    foreach (var item in args)
                    {
                        File.WriteAllText(Path.Combine(Application.StartupPath, "Open.tmp"), item, Encoding.UTF8);
                        //MessageBox.Show(item + " " + hWnd);

                        //GCHandle GCH = GCHandle.Alloc(item, GCHandleType.Pinned);
                        //IntPtr ptr = GCH.AddrOfPinnedObject();
                        ////IntPtr ptr = Marshal.StringToHGlobalUni(item);
                        ////SendMessage(this.Handle, 0X400, IntPtr.Zero, pS);
                        //int r = Win32API.SendMessage(hWnd, 0X400, IntPtr.Zero, ptr);
                        //ptr = Marshal.StringToHGlobalUni(item);
                        Win32API.SendMessage(hWnd, 0X400, IntPtr.Zero, IntPtr.Zero);
                        //GCH.Free();
                        //MessageBox.Show(r.ToString());
                    }                    
                }

            }
        }
    }
}
