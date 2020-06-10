using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chuyende
{
    public partial class FrmDangNhap : Form
    {
        public FrmDangNhap()
        {
            InitializeComponent();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Trim() == "")
            {
                MessageBox.Show("Tên tài khoản không được để trống", "Lỗi", MessageBoxButtons.OK);
                return;
            }
            if (txtPassword.Text.Trim() == "")
            {
                MessageBox.Show("Mật khẩu không được để trống", "Lỗi", MessageBoxButtons.OK);
                return;
            }
            Program.mlogin = txtUsername.Text.Trim();
            Program.password = txtPassword.Text.Trim();
            if (Program.KetNoi() == 0)
            {
                return;
            }
            //SqlDataReader myReader;
            Program.mloginDN = Program.mlogin;
            Program.passwordDN = Program.password;

            Program.conn.Close();

            MessageBox.Show("Đăng Nhập Thành Công", "", MessageBoxButtons.OK);

            this.Hide();

            FrmGiaoDich frm = new FrmGiaoDich();
            frm.ShowDialog();
            this.Close();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
