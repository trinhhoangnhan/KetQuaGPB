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
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        public frmMain()
        {
            InitializeComponent();
        }
        bool isLogin = false;
        /// <summary>
        /// hàm load form MDI
        /// </summary>
        private void AddWindows(Form f)
        {
            if (!ExistForm(f.Name))
            {
                f.MdiParent = this;                
                f.Show();
            }
            else
            {
                f.Dispose();

                Active_Form(f.Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formname"></param>
        /// <returns></returns>
        private bool ExistForm(string formname)
        {
            foreach (Form f in MdiChildren)
            {
                if (f.Name == formname)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formname"></param>
        public void Active_Form(string formname)
        {
            foreach (Form f in MdiChildren)
            {
                if (f.Name == formname)
                {
                    f.Select();
                    return;
                }
            }
        }
        
        string ConnStr = "";
        Configuration config = null;
        MySqlConnection Conn = null;

        private void frmMain_Load(object sender, EventArgs e)
        {
            ribbonControl1.Minimized = true;

            Configuration config
                = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            ConnStr = Uit.it_XML.GetConnString("MainConnStr", config);
            if (ConnStr != "")
            {
                ConnStr = Uit.it_Encryt.DecryptMD5(ConnStr, true);
            }

            //Conn = Uit.it_MySql.OpenConnect(ConnStr);
            ////frmLogin frm = new frmLogin();
            ////frm.ShowDialog();
            //Conn.Clone();
        }

        private void btnSetting_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmCauHinhHeThong frm = new frmCauHinhHeThong();
            frm.ShowDialog();
        }

        private void btnLogin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmLogin frm = new frmLogin();
            frm.ShowDialog();
        }

        private void btnNhapKQGPB_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmNhapKQGPB frm = new frmNhapKQGPB(ConnStr);
           
            AddWindows(frm);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if(Conn.State == ConnectionState.Open)
            //    Conn.Clone();
        }

        private void btnNhapBN_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmNhapBN frm = new frmNhapBN(ConnStr);

            AddWindows(frm);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmChupHinh_DS frm = new frmChupHinh_DS();
            frm.Show();
        }

        
    }
}