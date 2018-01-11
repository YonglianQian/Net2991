using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Frm
{
    public partial class Login : DevExpress.XtraEditors.XtraForm
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text=="admin" && textBox2.Text=="123")
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                //this.DialogResult = DialogResult.Cancel;
                MessageBox.Show("登录失败","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            this.textBox2.Text = "";
        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.textBox1.Focus();
        }

        private void Login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                button2_Click(sender, e);
            }
        }
    }
}
