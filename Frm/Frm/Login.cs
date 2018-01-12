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
    public partial class Login : Form
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
    }
}
