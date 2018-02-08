using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Frm.Module;
using Frm.Common;

namespace Frm
{
    public partial class MainFrm : DevExpress.XtraEditors.XtraForm
    {
        public MainFrm()
        {
            InitializeComponent();
        }

        private void tileItem5_ItemClick(object sender, TileItemEventArgs e)
        {
            FrmSettings settings = new FrmSettings();
            if (settings.ShowDialog()==DialogResult.Cancel)
            {
                settings.Dispose();
                Utility.ClearMemory();
            }
        }

        private void tileItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            TestManagement test = new TestManagement();
            if (test.ShowDialog() == DialogResult.Cancel)
            {
                test.Dispose();
                Utility.ClearMemory();
            }
            
            
            
        }

        private void tileItem6_ItemClick(object sender, TileItemEventArgs e)
        {
            FrmDetailsInformation Data = new FrmDetailsInformation();
            if (Data.ShowDialog()==DialogResult.Cancel)
            {
                Data.Dispose();
                Utility.ClearMemory(); 

            };
        }
    }
}