using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;

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
            
            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.Skins.SkinManager.EnableMdiFormSkins();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //禁止运行多个应用程式实例
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (var item in processes)
            {
                if (item.Id != current.Id)
                {
                    if (item.MainModule.FileName == current.MainModule.FileName)
                    {
                        XtraMessageBox.Show("程序已经运行", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
            }
            //登录验证
            //Login login = new Login();
            //if (login.ShowDialog()==DialogResult.OK)
            //{
                Application.Run(new MainFrm());
            //}
            
            
        }
    }
}
