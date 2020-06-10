using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chuyende
{
    public partial class FrmGiaoDich : Form
    {
        public FrmGiaoDich()
        {
            InitializeComponent();
        }

        // --------------- ZONE NGOC CODE ----------------- //
        private Boolean canRequestNotifications()
        {
            try
            {
                SqlClientPermission permision = new SqlClientPermission(PermissionState.Unrestricted);
                permision.Demand();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        protected string connectionString()
        {
            return @"Data Source=NGOC-PC;Initial Catalog=CHUNGKHOAN;Persist Security Info=True;User ID=sa;Password=123";
        }

        public static string GetConnectionString()
        {
            return Program.connstr = "Data Source=" + Program.servername + ";Initial Catalog=" +
                                Program.database + ";User ID=" +
                                Program.mlogin + ";password=" + Program.password;
        }

        protected string sqlQuery()
        {
            return "SELECT MACP AS [MACP],  GIAMUA2 AS [GIA MUA 2], KHOILUONG_MUA2 AS [KLM2], GIAMUA1 AS [GIA MUA 1], KHOILUONG_MUA1 AS [KLM 1], GIAKHOP AS [GIÁ KHỚP], KL_KHOP AS [KL KHỚP], GIABAN1 AS [GIA BAN 1], KHOILUONG_BAN1 AS [KLB1], GIABAN2 AS [GIA BAN 2], KHOILUONG_BAN2 AS [KLB2] FROM dbo.BANGGIA";
        }

        private void GetStarted()
        {
            Program.changeCount = 0;
            SqlDependency.Stop(connectionString());
            try
            {
                SqlDependency.Start(connectionString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error during initial connection", MessageBoxButtons.OK);
                return;
            }
            if (Program.connection == null)
            {
                Program.connection = new SqlConnection(connectionString());
                Program.connection.Open();
            }
            if (Program.command == null)
                // GetSQL is a local procedure that returns
                // a paramaterized SQL string. You might want
                // to use a stored procedure in your application.
                Program.command = new SqlCommand(sqlQuery(), Program.connection);
             
            if (Program.dataToWatch == null)
                Program.dataToWatch = new DataSet();
            GetData();
        }

        private void GetData()
        {
            Program.dataToWatch.Clear();
            Program.command.Notification = null;

            SqlDependency dependency = new SqlDependency(Program.command);
            dependency.OnChange += dependency_OnChange;

            using (SqlDataAdapter adapter = new SqlDataAdapter(Program.command))
            {
                adapter.Fill(Program.dataToWatch, Program.tableName);

                this.dataGridView.DataSource = Program.dataToWatch;
                this.dataGridView.DataMember = Program.tableName;
                try
                {
                    this.dataGridView.ClearSelection();
                    this.dataGridView.Rows[Program.vitriRow].Cells[Program.vitriColumn].Selected = true;
                }
                catch (Exception)
                {
                    this.dataGridView.ClearSelection();
                }

            }
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            ISynchronizeInvoke i = (ISynchronizeInvoke)this;
            if (i.InvokeRequired)
            {
                OnChangeEventHandler tempDelegate = new OnChangeEventHandler(dependency_OnChange);

                object[] args = new[] { sender, e };
                i.BeginInvoke(tempDelegate, args);

                return;
            }
            SqlDependency dependency = (SqlDependency)sender;

            dependency.OnChange -= dependency_OnChange;
            GetData();
        }
        // --------------- ZONE NGOC CODE ----------------- //



        private void FrmGiaoDich_Load(object sender, EventArgs e)
        {
           //-----------------NGOC ADD----------//
            if (canRequestNotifications())
                GetStarted();
            else
                MessageBox.Show("Failue Start, May be you fogot started Bro", "", MessageBoxButtons.OK);
            //-----------------NGOC ADD----------//

            this.WindowState = FormWindowState.Normal;
            // TODO: This line of code loads data into the 'cHUNGKHOANDataSet.LENHKHOP' table. You can move, or remove it, as needed.
            this.lENHKHOPTableAdapter.Fill(this.cHUNGKHOANDataSet.LENHKHOP);
          
            // TODO: This line of code loads data into the 'cHUNGKHOANDataSet.LENHDAT' table. You can move, or remove it, as needed.
            this.lENHDATTableAdapter.Fill(this.cHUNGKHOANDataSet.LENHDAT);

            DateTime datetime = DateTime.Now; ;
            this.nGAYDATDateTimePicker.Text = datetime.ToString();
            this.datePickerLenhBan.Text = datetime.ToString();

            //handle loai lenh mua
            IDictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("LO", "Khớp lệnh liên tục(LO)");
            dict.Add("ATO", "Khớp lệnh định kỳ(ATO)");
            dict.Add("ATC", "Khớp lệnh định kỳ(ATC )");
            comboLoaiLenh.DataSource = new BindingSource(dict, null);
            comboLoaiLenh.DisplayMember = "Value";
            comboLoaiLenh.ValueMember = "Key";

          //lenh ban
            comboLoaiLenhBan.DataSource = new BindingSource(dict, null);
            comboLoaiLenhBan.DisplayMember = "Value";
            comboLoaiLenhBan.ValueMember = "Key";
        }

        private void lENHDATBindingNavigatorSaveItem_Click_1(object sender, EventArgs e)
        {
            this.Validate();
            this.lENHDATBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.cHUNGKHOANDataSet);

        }

        private void btnThanhTien_Click(object sender, EventArgs e)
        {
             try
            {

                if (Program.conn.State == ConnectionState.Closed)
                    Program.conn.Open();

                if(this.mACPTextBox.Text.Trim() == "")
                {
                    MessageBox.Show("Vui lòng nhập mã cổ phiếu!");
                    return;
                }
                if (this.sOLUONGTextBox.Text.Trim() == "")
                {
                    MessageBox.Show("Vui lòng nhập số lượng!");
                    return;
                }
                if (this.gIADATTextBox.Text.Trim() == "")
                {
                    MessageBox.Show("Vui lòng nhập giá đặt !");
                    return;
                }

                DateTime datetime = nGAYDATDateTimePicker.Value;
                String datetimeFormmat = datetime + "";
                String[] date = datetimeFormmat.Split(' ');
                String str = date[0];

                String[] tempsplit = str.Split('/');
                String joinstring = "-";
                String newdate = tempsplit[2] + joinstring + tempsplit[0] + joinstring + tempsplit[1];
                //MessageBox.Show("Now is " + aDateTime); 
                String strLenh = "SP_KHOPLENH_LO";
                Program.sqlcmd = Program.conn.CreateCommand();
                Program.sqlcmd.CommandType = CommandType.StoredProcedure;
                Program.sqlcmd.CommandText = strLenh;
                Program.sqlcmd.Parameters.Add("@macp", SqlDbType.NVarChar).Value = mACPTextBox.Text;
                //Program.sqlcmd.Parameters.Add("@Ngay", SqlDbType.NVarChar).Value = "2020-04-02";
                Program.sqlcmd.Parameters.Add("@Ngay", SqlDbType.NVarChar).Value = newdate;
                Program.sqlcmd.Parameters.Add("@LoaiGD", SqlDbType.Char).Value = 'M';
                Program.sqlcmd.Parameters.Add("@soluongMB", SqlDbType.Int).Value = sOLUONGTextBox.Text;
                Program.sqlcmd.Parameters.Add("@giadatMB", SqlDbType.Float).Value = gIADATTextBox.Text;
                Program.sqlcmd.ExecuteNonQuery();
                Program.conn.Close();
                MessageBox.Show("Đặt lệnh mua thành công", "THÔNG BÁO", MessageBoxButtons.OK);
                this.lENHKHOPTableAdapter.Fill(this.cHUNGKHOANDataSet.LENHKHOP);
                this.lENHDATTableAdapter.Fill(this.cHUNGKHOANDataSet.LENHDAT);
                this.resetM();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lệnh mua.\n" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboLoaiLenhBan_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void resetB()
        {
            this.txtMaCpLenhBan.Text = "";
            this.txtSoluongBan.Text = "";
            this.txtGiaDatBan.Text = "";
        }
        public void resetM()
        {
            this.mACPTextBox.Text = "";
            this.sOLUONGTextBox.Text = "";
            this.gIADATTextBox.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (Program.conn.State == ConnectionState.Closed)
                    Program.conn.Open();

                if (this.txtMaCpLenhBan.Text.Trim() == "")
                {
                    MessageBox.Show("Vui lòng nhập mã cổ phiếu!");
                    return;
                }
                if (this.txtSoluongBan.Text.Trim() == "")
                {
                    MessageBox.Show("Vui lòng nhập số lượng!");
                    return;
                }
                if (this.txtGiaDatBan.Text.Trim() == "")
                {
                    MessageBox.Show("Vui lòng nhập giá đặt !");
                    return;
                }

                DateTime datetime = datePickerLenhBan.Value;
                String datetimeFormmat = datetime + "";
                String[] date = datetimeFormmat.Split(' ');
                String str = date[0];

                String[] tempsplit = str.Split('/');
                String joinstring = "-";
                String newdate = tempsplit[2] + joinstring + tempsplit[0] + joinstring + tempsplit[1];
                //MessageBox.Show("Now is " + aDateTime); 
                String strLenh = "SP_KHOPLENH_LO";
                Program.sqlcmd = Program.conn.CreateCommand();
                Program.sqlcmd.CommandType = CommandType.StoredProcedure;
                Program.sqlcmd.CommandText = strLenh;
                Program.sqlcmd.Parameters.Add("@macp", SqlDbType.NVarChar).Value = txtMaCpLenhBan.Text;
                Program.sqlcmd.Parameters.Add("@Ngay", SqlDbType.NVarChar).Value = newdate;
                Program.sqlcmd.Parameters.Add("@LoaiGD", SqlDbType.Char).Value = 'B';
                Program.sqlcmd.Parameters.Add("@soluongMB", SqlDbType.Int).Value = txtSoluongBan.Text;
                Program.sqlcmd.Parameters.Add("@giadatMB", SqlDbType.Float).Value = txtGiaDatBan.Text;
                Program.sqlcmd.ExecuteNonQuery();
                Program.conn.Close();
                MessageBox.Show("Đặt lệnh mua thành công", "THÔNG BÁO", MessageBoxButtons.OK);
                this.lENHKHOPTableAdapter.Fill(this.cHUNGKHOANDataSet.LENHKHOP);
                this.lENHDATTableAdapter.Fill(this.cHUNGKHOANDataSet.LENHDAT);
                this.resetB();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lệnh bán.\n" + ex.Message, "", MessageBoxButtons.OK);
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
