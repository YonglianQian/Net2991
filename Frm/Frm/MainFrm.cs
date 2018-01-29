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
            settings.ShowDialog();
        }

        private void tileItem1_ItemClick(object sender, TileItemEventArgs e)
        {
            Test test = new Test();
            test.ShowDialog();
           
            
            
        }

        private void tileItem6_ItemClick(object sender, TileItemEventArgs e)
        {
            FrmDetailsInformation Data = new FrmDetailsInformation();
            Data.ShowDialog();
        }
    }
}