using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Frm
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(SplashScreen1));
            //System.Threading.Thread.Sleep(5000);
            Login login = new Login();
            if (login.ShowDialog()==DialogResult.OK)
            {
                Application.Run(new Form1());
            }
            else
            {
                
            }
            
        }
    }
}
