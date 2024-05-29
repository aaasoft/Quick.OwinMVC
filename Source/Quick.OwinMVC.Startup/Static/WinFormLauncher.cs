using Quick.OwinMVC.Startup.Forms;
using Quick.OwinMVC.Startup.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Quick.OwinMVC.Startup.Static
{
    public static class WinFormLauncher
    {
        public static void Launch()
        {
            if (ProgramUtils.IsRuningOnWindows())
                ProgramUtils.FreeConsole();

            Thread staThread = new Thread(obj =>
            {
                Application.ThreadException += ThreadExceptionCallbackFun;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                MainForm form = new MainForm();
                Application.Run(form);
            });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
        }

        /// <summary>
        /// 线程异常回掉函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ThreadExceptionCallbackFun(object sender, ThreadExceptionEventArgs e)
        {
            Console.WriteLine(string.Format("发生严重未处理线程异常.异常:{0}{1}", Environment.NewLine, e.Exception));
        }
    }
}
