using Quick.OwinMVC.Program.Forms;
using Quick.OwinMVC.Program.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Quick.OwinMVC.Program.Static
{
    public static class WinFormLauncher
    {
        public static void Launch()
        {
            Application.ThreadException += ThreadExceptionCallbackFun;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm form = new MainForm();
            Application.Run(form);
            if (form.DialogResult == DialogResult.None)
                return;
            DebugLauncher.Launch();
        }

        /// <summary>
        /// 线程异常回掉函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ThreadExceptionCallbackFun(object sender, ThreadExceptionEventArgs e)
        {
            Console.WriteLine($"发生严重未处理线程异常.异常:{Environment.NewLine}{e.Exception.ToString()}");
        }
    }
}
