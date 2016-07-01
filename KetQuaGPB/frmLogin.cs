using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Net;

namespace KetQuaGPB
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        MySqlConnection Conn;
        private void btnLogin_Click(object sender, EventArgs e)
        {
            Configuration config 
                = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            string MainConn = Uit.it_XML.GetConnString("MainConnStr", config);
            if (MainConn != "")
            {
                MainConn = Uit.it_Encryt.DecryptMD5(MainConn, true);
            }

            Conn = Uit.it_MySql.OpenConnect(MainConn);
            
            string UserName = txtUserName.Text;
            string Pass = Uit.it_Encryt.EncryptMD5(txtPassword.Text,true);

            string sql = "SELECT\n" +
                        "	*\n" +
                        "FROM\n" +
                        "	gpb_user us\n" +
                        "WHERE\n" +
                        "	us.UserName = '" + UserName + "'\n" +
                        "AND us.Pass = '" + Pass + "'\n" +
                        "AND us.TinhTrang = 0";
            DataTable dt = Uit.it_MySql.getDataTable(sql, Conn);

            int NumRows = Uit.it_Parse.ToInteger( dt.Rows.Count);
           
            if (NumRows>0)
            {                
                string HostName = Dns.GetHostName().ToString();

                long Now = DateTime.Now.ToFileTime();
                string sqlLog = "INSERT INTO Login_Log (UserLogin, IPLogin, TimeLog)\n" +
                        "VALUES\n" +
                        "	('" + UserName + "', '" + HostName + "'," + Now + ")";
                bool rs = Uit.it_MySql.ExecuteNonQuery(sqlLog, true, Conn, true);
                lblMessage.Text = "Đăng nhập thành công!";
                lblMessage.ForeColor = System.Drawing.Color.BlueViolet;
                this.Close();
            }
            else
            {
                lblMessage.Text = "Đăng nhập không thành công!";
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }

            Uit.it_MySql.CloseConnect(Conn);
        }

        private void frmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {            
            this.Close();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";
        }

        private void txtUserName_Validated(object sender, EventArgs e)
        {
            if (txtUserName.Text.Trim() == "")
            {
                lblMessUserName.Text = "Bạn chưa nhập thông tin người dùng!";
                lblMessUserName.ForeColor = System.Drawing.Color.Red;
                txtUserName.Focus();
            }
            else
            {
                lblMessUserName.Text = "";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}