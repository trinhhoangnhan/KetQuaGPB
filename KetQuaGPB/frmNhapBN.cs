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
using System.Collections;
using DataColection;
using System.IO;

namespace KetQuaGPB
{
    public partial class frmNhapBN : DevExpress.XtraEditors.XtraForm
    {
        public frmNhapBN()
        {
            InitializeComponent();
        }

        public frmNhapBN(string strConn)
        {
            InitializeComponent();
            ConnStr = strConn;
        }

        int MaPK = 102;
        string ConnStr = "";
        Configuration Config = null;
        MySqlConnection Conn = null;
        DataTable dtCode = null;

        private ArrayList arrControlsKQ = new ArrayList();
        private ArrayList arrControlsKey = new ArrayList();
        private ArrayList arrControl = new ArrayList();

        private void frmNhapBN_Load(object sender, EventArgs e)
        {
            Config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            //MySqlConnection Conn = Uit.it_MySql.OpenConnect(ConnStr);
            
            //arrControlsKQ.Clear();
            //arrControlsKey.Clear();
            arrControl.Clear();
            AddControlsBN();
            radGioiTinh.EditValue = 1;
            //Conn.Close();
        }

        void AddControlsBN()
        {
            this.arrControl.Add(txtMaBN);
            this.arrControl.Add(txtHo);
            this.arrControl.Add(txtTen);
            this.arrControl.Add(txtNamSinh);
            this.arrControl.Add(radGioiTinh);
            this.arrControl.Add(txtDiaChi);
            this.arrControl.Add(txtDienThoai);

        }

        long GetMaxNum()
        {
            MySqlConnection Conn = Uit.it_MySql.OpenConnect(ConnStr);
            long Num = BUS.GetData.getMaxNum(MaPK,HangSo.SOHS, Conn);
            
            Conn.Close();
            return Num;
        }
        //get new id
        private void btnNew_Click(object sender, EventArgs e)
        {
           
            long Num = GetMaxNum();
            txtMaBN.Text = Num.ToString();
            MySqlConnection Conn = Uit.it_MySql.OpenConnect(ConnStr);
            DataTable dtBN = BUS.GetData.getBNByMa(Num, Conn);
            if (dtBN != null)
            {
                if (dtBN.Rows.Count > 0)
                {
                    SetBNToForm(dtBN.Rows[0]);
                }
                else
                {
                    SetBNToForm(dtBN.NewRow());
                }
            }
            Conn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtHo.Text.Trim() == "")
            {
                Uit.it_Msg.Error("Bạn phải nhập Họ và tên lót của bệnh nhân!");
                txtHo.Focus();
                txtHo.SelectAll();
                return;
            }
            if (txtTen.Text.Trim() == "")
            {
                Uit.it_Msg.Error("Bạn phải nhập tên của bệnh nhân!");
                txtTen.Focus();
                txtTen.SelectAll();
                return;
            }
            if (txtNamSinh.Text.Trim() == "")
            {
                Uit.it_Msg.Error("Bạn phải nhập năm sinh của bệnh nhân!");
                txtNamSinh.Focus();
                txtNamSinh.SelectAll();
                return;
            }

            DbSql ole = new DbSql(ConnStr);
          
            arrControl.Clear();
            MySqlConnection Conn = Uit.it_MySql.OpenConnect(ConnStr);
            
            AddControlsBN();
            bool rs = ole.InsertOneKey("GPB_BenhNhan", arrControl, ConnStr);

            if (rs)
            {
                Uit.it_Msg.Information("Lưu kết quả thành công!");
                int NewNum = Uit.it_Parse.ToInteger(txtMaBN.Text.Substring(5,5));
                NewNum++;
                BUS.UpdateData.updSoMax(MaPK, 99,NewNum, Conn);
            }

            DataTable dtBn = BUS.GetData.getAllBN(Conn);
            gridDSBN.DataSource = dtBn;
            Conn.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            
            long Num = 0;
            if (txtMaBN.Text.Trim() == "")
            {
                Num = GetMaxNum();
                txtMaBN.Text = Num.ToString();
            }
            else
            {
                Num = Uit.it_Parse.ToLong(txtMaBN.Text) + 1;
                txtMaBN.Text = Num.ToString();
            }
            MySqlConnection Conn = Uit.it_MySql.OpenConnect(ConnStr);

            long NumMax = BUS.GetData.get_CurMaxNumBN(Conn);            
            long CurNum = Uit.it_Parse.ToLong(txtMaBN.Text);
            if (CurNum >= NumMax)
            {
                MessageBox.Show("Bạn đã chọn bệnh nhân có ID lớn nhất!");
                return;
            }

            DataTable dtBN = BUS.GetData.getBNByMa(Num, Conn);
            if (dtBN != null)
            {
                if (dtBN.Rows.Count > 0)
                {
                    SetBNToForm(dtBN.Rows[0]);
                }
                else
                {
                    SetBNToForm(dtBN.NewRow());
                }
            }
            Conn.Close();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
           
            long Num = 0;
            if (txtMaBN.Text.Trim() == "")
            {
                Num = GetMaxNum();
                txtMaBN.Text = Num.ToString();
            }
            else
            {
                Num = Uit.it_Parse.ToLong(txtMaBN.Text) - 1;
                txtMaBN.Text = Num.ToString();
            }
            MySqlConnection Conn = Uit.it_MySql.OpenConnect(ConnStr);

            long NumMax = BUS.GetData.get_CurMinNumBN(Conn);
            long CurNum = Uit.it_Parse.ToLong(txtMaBN.Text);
            if (CurNum <= NumMax)
            {
                MessageBox.Show("Bạn đã chọn bệnh nhân có ID nhỏ nhất!");
                return;
            }

            DataTable dtBN = BUS.GetData.getBNByMa(Num, Conn);
            if (dtBN != null)
            {
                if (dtBN.Rows.Count > 0)
                {
                    SetBNToForm(dtBN.Rows[0]);
                }
                else
                {
                    SetBNToForm(dtBN.NewRow());
                }
            }
            Conn.Close();
        }

        void SetBNToForm(DataRow dr)
        {
            for (int i = 1; i < arrControl.Count; i++)
            {
                TextEdit box7;
                PictureBox box10;
                string str6;
                string NameControl = "";
                switch (arrControl[i].GetType().ToString().Trim())
                {
                    case "System.Windows.Forms.NumericUpDown":
                        {
                            NumericUpDown down = (NumericUpDown)arrControl[i];
                            NameControl = down.Name.Substring(3);
                            down.Value = Uit.it_Parse.ToInteger(dr[NameControl]);
                            break;
                        }
                    case "DevExpress.XtraEditors.MemoEdit":
                        {
                            MemoEdit box4 = (MemoEdit)arrControl[i];
                            NameControl = box4.Name.Substring(3);
                            box4.Text = dr[NameControl].ToString();
                            break;
                        }
                    case "DevExpress.XtraEditors.GroupControl":
                        {
                            GroupControl box5 = (GroupControl)arrControl[i];
                            NameControl = box5.Name.Substring(3);
                            box5.Text = dr[NameControl].ToString();
                            break;
                        }
                    case "System.Windows.Forms.DateTimePicker":
                        {
                            DateTimePicker picker = (DateTimePicker)arrControl[i];
                            NameControl = picker.Name.Substring(3);
                            picker.Value = Uit.it_Parse.ToDateTime(dr[NameControl]);
                            break;
                        }
                    case "DevExpress.XtraEditors.CheckEdit":
                        {
                            CheckEdit box6 = (CheckEdit)arrControl[i];
                            NameControl = box6.Name.Substring(3);
                            box6.Text = dr[NameControl].ToString();
                            break;
                        }
                    case "DevExpress.XtraEditors.RadioGroup":
                        {
                            RadioGroup button = (RadioGroup)arrControl[i];
                            NameControl = button.Name.Substring(3);
                            button.EditValue = Uit.it_Parse.ToInteger( dr[NameControl]);
                            break;
                        }
                    case "DevExpress.XtraEditors.TextEdit":
                        {
                            box7 = (TextEdit)arrControl[i];
                            NameControl = box7.Name.Substring(3);
                            box7.Text = dr[NameControl].ToString();
                            break;
                        }
                    case "DevExpress.XtraEditors.GridLookUpEdit":
                        {
                            GridLookUpEdit box8 = (GridLookUpEdit)arrControl[i];
                            NameControl = box8.Name.Substring(3);
                            box8.EditValue = Uit.it_Parse.ToInteger(dr[NameControl]);
                            break;
                        }
                    case "System.Windows.Forms.MaskedTextBox":
                        {
                            MaskedTextBox box9 = (MaskedTextBox)arrControl[i];
                            NameControl = box9.Name.Substring(3);
                            box9.Text = Uit.it_Parse.ToString(dr[NameControl]);
                            break;
                        }
                    case "DevExpress.XtraEditors.PictureEdit":
                        {
                            //box10 = (PictureBox)arrControl[i];
                            //NameControl = box9.Name.Substring(3);

                            //MemoryStream stream = new MemoryStream();
                            //box10.Image.Save(stream, ImageFormat.Jpeg);
                            //byte[] buffer = stream.ToArray();
                            //ole.DBCmd.Parameters.AddWithValue("@" + str2, buffer);
                            //goto Label_0614;
                            break;
                        }
                    default:
                        break;
                }
                
            }
        }

        private void btnLasted_Click(object sender, EventArgs e)
        {
           
            MySqlConnection Conn = Uit.it_MySql.OpenConnect(ConnStr);
            long Num = BUS.GetData.getMaxNumBN(Conn);
            txtMaBN.Text = Num.ToString();

            DataTable dtBN = BUS.GetData.getBNByMa(Num, Conn);
            if (dtBN != null)
            {
                if (dtBN.Rows.Count > 0)
                {
                    SetBNToForm(dtBN.Rows[0]);
                }
                else
                {
                    SetBNToForm(dtBN.NewRow());
                }
            }
           
            Conn.Close();
        }

        private void btnLoadBN_Click(object sender, EventArgs e)
        {
            MySqlConnection Conn = Uit.it_MySql.OpenConnect(ConnStr);
            DataTable dt = BUS.GetData.getAllBN(Conn);

            gridDSBN.DataSource = dt;
            Conn.Close();
        }

        private void gridDSBN_Click(object sender, EventArgs e)
        {
            DataRow dr;
            dr = viewDSBN.GetFocusedDataRow();
            if (gridDSBN.DataSource == null)
                return;
            if (dr!=null)
            {
                SetBNToForm(dr);
            }
            else
            {
                SetBNToForm(dr.Table.NewRow());
            }
        }

        private void gridDSBN_DoubleClick(object sender, EventArgs e)
        {
            DataRow dr;
            dr = viewDSBN.GetFocusedDataRow();
        }

    }
}