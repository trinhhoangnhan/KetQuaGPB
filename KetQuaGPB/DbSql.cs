using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Drawing.Imaging;
using System.IO;

namespace DataColection
{
    public class DbSql
    {
        private MySqlCommand dbCmd;
        private MySqlConnection dbCon;
        private MySqlDataAdapter dbDap;
        public MySqlCommand DBCmd
        {
            get
            {
                return this.dbCmd;
            }
        }

        public MySqlConnection DBCon
        {
            get
            {
                return this.dbCon;
            }
        }

        public MySqlDataAdapter DBDap
        {
            get
            {
                return this.dbDap;
            }
        }
        public DbSql(string ConnStr)
        {
            this.dbCon = new MySqlConnection(ConnStr);
            this.dbCon.Open();
            this.dbCmd = new MySqlCommand();
            this.dbCmd.Connection = this.dbCon;
            this.dbDap = new MySqlDataAdapter();
            this.dbDap.SelectCommand = this.dbCmd;
        }

        public DbSql(MySqlConnection Conn)
        {
            this.dbCon = Conn;
            if (this.dbCon.State != ConnectionState.Open)
                this.dbCon.Open();
            this.dbCmd = new MySqlCommand();
            this.dbCmd.Connection = dbCon;
            this.dbDap = new MySqlDataAdapter();
            this.dbDap.SelectCommand = this.dbCmd;
        }

        public void CloseConnect()
        {
            if (this.dbCmd != null)
            {
                this.dbCmd.Dispose();
            }
            if (this.dbDap != null)
            {
                this.dbDap.Dispose();
            }
            if ((this.dbCon != null) && (this.dbCon.State != ConnectionState.Closed))
            {
                this.dbCon.Close();
                this.dbCon.Dispose();
            }
        }

        public void DestroyDBCmd()
        {
            if (this.dbCmd != null)
            {
                this.dbCmd.Dispose();
            }
            if (this.dbDap != null)
            {
                this.dbDap.Dispose();
            }
        }

        public void GetNewDBCmd()
        {
            if (this.dbCmd != null)
            {
                this.DestroyDBCmd();
            }
            this.dbCmd = new MySqlCommand();
            this.dbCmd.Connection = this.dbCon;
            this.dbDap = new MySqlDataAdapter(this.dbCmd);
        }


        public bool InsertOneKey(string pcTable, ArrayList arrControl, string ConnStr)
        {
            bool flag = false;
            if (string.IsNullOrEmpty(pcTable.Trim()) || (arrControl.Count == 0))
            {
                MessageBox.Show("Tham số truyền cho phương thức 'InsertOneKey' kh\x00f4ng hợp lệ. \n Developer vui l\x00f2ng kiểm tra lại !", "Th\x00f4ng b\x00e1o", MessageBoxButtons.OK);
                return false;
            }
            string str = "";
            string str2 = "";
            string str3 = "";
            string str4 = "";
            string pcKeyColumn = "";
            string pcKeyValue = "";

            if (arrControl[0].GetType().ToString().Trim() == "System.Windows.Forms.MaskedTextBox")
            {
                MaskedTextBox box = (MaskedTextBox)arrControl[0];
                pcKeyColumn = box.Name.Substring(3);
                pcKeyValue = box.Text.Trim();
            }
            else if (arrControl[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.GridLookUpEdit")
            {
                GridLookUpEdit box2 = (GridLookUpEdit)arrControl[0];
                pcKeyColumn = box2.Name.Substring(3);
                pcKeyValue = box2.EditValue.ToString();
            }
            else if (arrControl[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.TextEdit")
            {
                TextEdit box3 = (TextEdit)arrControl[0];
                pcKeyColumn = box3.Name.Substring(3);
                pcKeyValue = box3.Text.Trim();
            }
            else if (arrControl[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.MemoEdit")
            {
                MemoEdit box4 = (MemoEdit)arrControl[0];
                pcKeyColumn = box4.Name.Substring(3);
                pcKeyValue = box4.EditValue.ToString();
            }
            else if (arrControl[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.GroupControl")
            {
                GroupBox box5 = (GroupBox)arrControl[0];
                pcKeyColumn = box5.Name.Substring(3);
                pcKeyValue = box5.Tag.ToString().Trim();
            }

            DbSql ole = new DbSql(ConnStr);

            if (!CheckExistItem(pcTable, pcKeyColumn, pcKeyValue, ConnStr))
            {
                str2 = "INSERT INTO " + pcTable;
                str3 = "VALUES";
                for (int i = 0; i < arrControl.Count; i++)
                {
                    PictureBox box12;
                    switch (arrControl[i].GetType().ToString().Trim())
                    {
                        case "DevExpress.XtraEditors.SpinEdit":
                            {
                                SpinEdit down = (SpinEdit)arrControl[i];
                                str4 = down.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, down.Value);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.MemoEdit":
                            {
                                MemoEdit box6 = (MemoEdit)arrControl[i];
                                str4 = box6.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, box6.Text);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.DateEdit":
                            {
                                DateEdit picker = (DateEdit)arrControl[i];
                                str4 = picker.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, picker.EditValue);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.CheckEdit":
                            {
                                CheckEdit box7 = (CheckEdit)arrControl[i];
                                str4 = box7.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, box7.EditValue);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.RadioGroup":
                            {
                                RadioGroup button = (RadioGroup)arrControl[i];
                                str4 = button.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, button.EditValue);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.TextEdit":
                            {
                                TextEdit box8 = (TextEdit)arrControl[i];
                                str4 = box8.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, box8.Text.Trim());
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.GridLookUpEdit":
                            {
                                GridLookUpEdit box9 = (GridLookUpEdit)arrControl[i];
                                str4 = box9.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, box9.EditValue);
                                goto Label_06A7;
                            }
                        case "DevExpress.XtraEditors.GroupControl":
                            {
                                GroupBox box10 = (GroupBox)arrControl[i];
                                str4 = box10.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, box10.Tag.ToString().Trim());
                                goto Label_06A7;
                            }

                        case "DevExpress.XtraEditors.PictureEdit":
                            {
                                box12 = (PictureBox)arrControl[i];
                                str4 = box12.Name.Substring(3);
                                if (!str4.Contains("file"))
                                {
                                    goto Label_0678;
                                }
                                if (box12.Image == null)
                                {
                                    break;
                                }
                                MemoryStream stream = new MemoryStream();
                                box12.Image.Save(stream, ImageFormat.Jpeg);
                                byte[] buffer = stream.ToArray();
                                ole.DBCmd.Parameters.AddWithValue("@" + str4, buffer);
                                goto Label_06A7;
                            }
                        default:
                            goto Label_06A7;
                    }
                    string str8 = "";
                    ole.DBCmd.Parameters.AddWithValue("@" + str4, str8);
                    goto Label_06A7;
                Label_0678:
                    ole.DBCmd.Parameters.AddWithValue("@" + str4, box12.Tag.ToString().Trim());
                Label_06A7:
                    str2 = str2 + (str2.Contains("(") ? (", " + str4) : (" (" + str4));
                    str3 = str3 + (str3.Contains("(") ? (", @" + str4) : (" (@" + str4));
                }
                str = str2 + ") " + str3 + ")";
                try
                {
                    ole.DBCmd.CommandText = str;
                    flag = ole.DBCmd.ExecuteNonQuery() > 0;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                ole.CloseConnect();
                return flag;
            }
            return UpdateOneKey(pcTable, arrControl, ConnStr);
        }

        public bool UpdateOneKey(string pcTable, ArrayList arrControl, string ConnStr)
        {
            bool flag = false;
            if (string.IsNullOrEmpty(pcTable.Trim()) || (arrControl.Count == 0))
            {
                MessageBox.Show("Tham số truyền cho phương thức 'UpdateOneKey' kh\x00f4ng hợp lệ. \n Developer vui l\x00f2ng kiểm tra lại !", "Th\x00f4ng b\x00e1o", MessageBoxButtons.OK);
                return false;
            }
            string str = "";
            string str2 = "";
            string pcKeyColumn = "";
            string pcKeyValue = "";
            if (arrControl[0].GetType().ToString().Trim() == "System.Windows.Forms.MaskedTextBox")
            {
                MaskedTextBox box = (MaskedTextBox)arrControl[0];
                pcKeyColumn = box.Name.Substring(3);
                pcKeyValue = box.Text.Trim();
            }
            else if (arrControl[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.GridLookUpEdit")
            {
                GridLookUpEdit box2 = (GridLookUpEdit)arrControl[0];
                pcKeyColumn = box2.Name.Substring(3);
                pcKeyValue = box2.EditValue.ToString();
            }
            else if (arrControl[0].GetType().ToString().Trim() == "DevExpress.XtraEditors.TextEdit")
            {
                TextEdit box3 = (TextEdit)arrControl[0];
                pcKeyColumn = box3.Name.Substring(3);
                pcKeyValue = box3.Text.Trim();
            }
            DbSql ole = new DbSql(ConnStr);

            if (CheckExistItem(pcTable, pcKeyColumn, pcKeyValue, ConnStr))
            {
                str = "UPDATE " + pcTable;
                for (int i = 1; i < arrControl.Count; i++)
                {
                    TextEdit box7;
                    PictureBox box10;
                    string str6;
                    switch (arrControl[i].GetType().ToString().Trim())
                    {
                        case "System.Windows.Forms.NumericUpDown":
                            {
                                NumericUpDown down = (NumericUpDown)arrControl[i];
                                str2 = down.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, down.Value);
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.MemoEdit":
                            {
                                MemoEdit box4 = (MemoEdit)arrControl[i];
                                str2 = box4.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, box4.Text);
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.GroupControl":
                            {
                                GroupControl box5 = (GroupControl)arrControl[i];
                                str2 = box5.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, box5.Tag.ToString().Trim());
                                goto Label_0614;
                            }
                        case "System.Windows.Forms.DateTimePicker":
                            {
                                DateTimePicker picker = (DateTimePicker)arrControl[i];
                                str2 = picker.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, picker.Value.ToShortDateString());
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.CheckEdit":
                            {
                                CheckEdit box6 = (CheckEdit)arrControl[i];
                                str2 = box6.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, box6.CheckState);
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.RadioGroup":
                            {
                                RadioGroup button = (RadioGroup)arrControl[i];
                                str2 = button.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, button.EditValue);
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.TextEdit":
                            box7 = (TextEdit)arrControl[i];
                            str2 = box7.Name.Substring(3).ToLower();
                            if (!str2.Contains("fileh"))
                            {
                                break;
                            }
                            ole.DBCmd.Parameters.AddWithValue("@" + str2, box7.Text.Trim());
                            goto Label_0614;

                        case "DevExpress.XtraEditors.GridLookUpEdit":
                            {
                                GridLookUpEdit box8 = (GridLookUpEdit)arrControl[i];
                                str2 = box8.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, box8.EditValue);
                                goto Label_0614;
                            }
                        case "System.Windows.Forms.MaskedTextBox":
                            {
                                MaskedTextBox box9 = (MaskedTextBox)arrControl[i];
                                str2 = box9.Name.Substring(3);
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, box9.Text.Trim());
                                goto Label_0614;
                            }
                        case "DevExpress.XtraEditors.PictureEdit":
                            {
                                box10 = (PictureBox)arrControl[i];
                                str2 = box10.Name.Substring(3);
                                if (!str2.Contains("file"))
                                {
                                    goto Label_05E6;
                                }
                                if (box10.Image == null)
                                {
                                    goto Label_05BE;
                                }
                                MemoryStream stream = new MemoryStream();
                                box10.Image.Save(stream, ImageFormat.Jpeg);
                                byte[] buffer = stream.ToArray();
                                ole.DBCmd.Parameters.AddWithValue("@" + str2, buffer);
                                goto Label_0614;
                            }
                        default:
                            goto Label_0614;
                    }
                    ole.DBCmd.Parameters.AddWithValue("@" + str2, box7.Text.Trim());
                    goto Label_0614;
                Label_05BE:
                    str6 = "";
                    ole.DBCmd.Parameters.AddWithValue("@" + str2, str6);
                    goto Label_0614;
                Label_05E6:
                    ole.DBCmd.Parameters.AddWithValue("@" + str2, box10.Tag.ToString().Trim());
                Label_0614:
                    str = str + (str.Contains("SET") ? (", " + str2 + " = @" + str2) : (" SET " + str2 + " = @" + str2));
                }
                string str8 = str;
                str = str8 + " WHERE " + pcKeyColumn + " = @" + pcKeyColumn;
                ole.DBCmd.CommandText = str;
                ole.DBCmd.Parameters.AddWithValue("@" + pcKeyColumn, pcKeyValue.Trim());
                try
                {
                    flag = ole.DBCmd.ExecuteNonQuery() > 0;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
                ole.CloseConnect();
                return flag;
            }
            return InsertOneKey(pcTable, arrControl, ConnStr);
        }

        public bool CheckExistItem(string pcTable, string pcKeyColumn, string pcKeyValue, string ConnStr)
        {
            string sql = "SELECT " + pcKeyColumn + " FROM " + pcTable + " WHERE " + pcKeyColumn.Trim() + " = '" + pcKeyValue.Trim() + "'";
            return !string.IsNullOrEmpty(GetStringValue(sql, ConnStr).Trim());
        }

        public string GetStringValue(string stringQuery, string ConnStr)
        {
            string str = "";
            MySqlConnection Conn = Uit.it_MySql.OpenConnect(ConnStr);

            object obj2 = Uit.it_MySql.ExecuteScalar(stringQuery, Conn);
            if (obj2 != null)
            {
                str = obj2.ToString().Trim();
            }
            Conn.Close();
            return str;

        }
    }
}
