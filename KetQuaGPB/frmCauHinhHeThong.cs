using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Security;
using System.Configuration;
using System.Reflection;
using System.Xml;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;
using DTO;

namespace KetQuaGPB
{
    public partial class frmCauHinhHeThong : DevExpress.XtraEditors.XtraForm
    {
        public frmCauHinhHeThong()
        {
            InitializeComponent();
        }

        private void frmCauHinhHeThong_Load(object sender, EventArgs e)
        {
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            txtServer.Text = Uit.it_Encryt.DecryptMD5(Uit.it_XML.GetSetting("Server", config), true);
            txtUserName.Text = Uit.it_Encryt.DecryptMD5(Uit.it_XML.GetSetting("UserName", config), true);
            txtPass.Text = Uit.it_Encryt.DecryptMD5(Uit.it_XML.GetSetting("Pass", config), true);
            txtDBName.Text = Uit.it_Encryt.DecryptMD5(Uit.it_XML.GetSetting("DBName", config), true);

            lblRs.Text = "";
        }

        public static bool checkDB_Conn()
        {
            var conn_info = "Server=address;Port=3306;Database=dbhere;Uid=admin;Pwd=123";
            bool isConn = false;
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(conn_info);
                conn.Open();
                isConn = true;
            }           
            catch (MySqlException ex)
            {               
                isConn = false;
                switch (ex.Number)
                {
                    //http://dev.mysql.com/doc/refman/5.0/en/error-messages-server.html
                    case 1042: // Unable to connect to any of the specified MySQL hosts (Check Server,Port)
                        break;
                    case 0: // Access denied (Check DB name,username,password)
                        break;
                    default:
                        break;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return isConn;
        }

        //private static void AddSetting(string key, string value, string tag)
        //{
        //    Configuration configuration = ConfigurationManager.
        //        OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

        //    configuration.AppSettings.Settings.Remove(key);
        //    configuration.AppSettings.Settings.Add(key,value);
            
        //    configuration.Save();

        //    ConfigurationManager.RefreshSection(tag);
        //}

        //private static void UpdateSetting(string key, string value, string tag)
        //{
        //    Configuration configuration = ConfigurationManager.
        //        OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        //    configuration.AppSettings.Settings[key].Value = value;
        //    configuration.Save();

        //    ConfigurationManager.RefreshSection(tag);
        //}

        //private static void RemoveSetting(string key, string tag)
        //{
        //    Configuration configuration = ConfigurationManager.
        //        OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        //    configuration.AppSettings.Settings.Remove(key);
        //    configuration.Save();

        //    ConfigurationManager.RefreshSection(tag);
        //}

        //private static string  GetSetting(string key)
        //{
        //    string Value="";          
        //    Value = ConfigurationManager.AppSettings[key];           

        //    return Value;
        //}

             
        private void btnTestConnet_Click(object sender, EventArgs e)
        {                   
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
            conn_string.Server = txtServer.Text.Trim();
            conn_string.UserID = txtUserName.Text.Trim();
            conn_string.Password = txtPass.Text.Trim();
            conn_string.Database = txtDBName.Text.Trim();
            string connString = conn_string.ToString();

            MySqlConnection con = new MySqlConnection(connString);

            con.Open();
            if (con.State == ConnectionState.Open)
                lblRs.Text = "Kết nối thành công!";
            else
                lblRs.Text = "Kết nối thất bại!";
            con.Close();
        }

        private static byte[] KhoaMaHoa;
        private static byte[] KhoaNgauNhien;
        private static void TaoKhoaMaHoa(RijndaelManaged rdProvider)
        {
            if (KhoaMaHoa == null)
            {
                KhoaMaHoa = Encoding.ASCII.GetBytes("tisasofttisasofttisasofttisasoft");
            }
        }

        private static void TaoKhoaNgauNhien(RijndaelManaged rdProvider)
        {
            if (KhoaNgauNhien == null)
            {
                KhoaNgauNhien = Encoding.ASCII.GetBytes("tisasofttisasofttisasofttisasoft");
            }
        }

        public static string GiaiMa(string ChuoiDaMaHoa)
        {
            byte[] buffer = Convert.FromBase64String(ChuoiDaMaHoa);
            byte[] buffer2 = new byte[buffer.Length];
            RijndaelManaged rdProvider = new RijndaelManaged();
            MemoryStream stream = new MemoryStream(buffer);
            rdProvider.KeySize = 0x100;
            rdProvider.BlockSize = 0x100;
            TaoKhoaMaHoa(rdProvider);
            TaoKhoaNgauNhien(rdProvider);
            if ((KhoaMaHoa == null) || (KhoaNgauNhien == null))
            {
                throw new NullReferenceException("saveKey and savedIV must be non null");
            }
            ICryptoTransform transform = rdProvider.CreateDecryptor((byte[])KhoaMaHoa.Clone(), (byte[])KhoaNgauNhien.Clone());
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            stream2.Read(buffer2, 0, buffer2.Length);
            stream.Close();
            stream2.Close();
            transform.Dispose();
            rdProvider.Clear();
            return Encoding.ASCII.GetString(buffer2).ToString();
        }

        public static string MaHoa(string originalStr)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(originalStr);
            byte[] inArray = new byte[0];
            MemoryStream stream = new MemoryStream(bytes.Length);
            RijndaelManaged rdProvider = new RijndaelManaged
            {
                KeySize = 0x100,
                BlockSize = 0x100
            };
            TaoKhoaMaHoa(rdProvider);
            TaoKhoaNgauNhien(rdProvider);
            if ((KhoaMaHoa == null) || (KhoaNgauNhien == null))
            {
                throw new NullReferenceException("saveKey and savedIV must be non-null");
            }
            ICryptoTransform transform = rdProvider.CreateEncryptor(KhoaMaHoa, KhoaNgauNhien);
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            inArray = stream.ToArray();
            stream.Close();
            stream2.Close();
            transform.Dispose();
            rdProvider.Clear();
            return Convert.ToBase64String(inArray);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            string Server_En = Uit.it_Encryt.EncryptMD5(txtServer.Text.Trim(),true);
            string UserName_En = Uit.it_Encryt.EncryptMD5(txtUserName.Text.Trim(), true);
            string Pass_En = Uit.it_Encryt.EncryptMD5(txtPass.Text.Trim(), true);
            string DBName_En = Uit.it_Encryt.EncryptMD5(txtDBName.Text.Trim(), true);
            Configuration config 
                = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            Uit.it_XML.AddSetting("Server", Server_En, config);
            Uit.it_XML.AddSetting("UserName", UserName_En, config);
            Uit.it_XML.AddSetting("Pass", Pass_En, config);
            Uit.it_XML.AddSetting("DBName", DBName_En, config);
            Uit.it_XML.AddSetting("Character", "Character Set=utf8", config);
            

            MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
            conn_string.Server = txtServer.Text.Trim();
            conn_string.UserID = txtUserName.Text.Trim();
            conn_string.Password = txtPass.Text.Trim();
            conn_string.Database = txtDBName.Text.Trim();
            conn_string.CharacterSet = "utf8";
            string connString = Uit.it_Encryt.EncryptMD5(conn_string.ToString(),true);

            string extConnStr = Uit.it_XML.GetConnString("MainConnStr", config);
            if (extConnStr == "")
            {
                Uit.it_XML.AddConnString("MainConnStr", connString, config);
            }
            else
            {
                Uit.it_XML.UpdateConnString("MainConnStr", connString, config);
            }
        }
    }
}