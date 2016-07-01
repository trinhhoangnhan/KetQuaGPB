using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace BUS
{
    public class GetData
    {
        public static DataTable getLoaiDV(MySqlConnection Conn)
        {
            DataTable dtRs = null;

            string sql = "SELECT\n" +
                        "	ldv.IDLoaiKQ,\n" +
                        "	ldv.TenLoaiKQ,\n" +
                        "	ldv.PreFix,\n" +
                        "	ldv.TenTat,\n" +
                         "	YEAR(NOW()) AS YEAR\n" +
                        "FROM\n" +
                        "	gpb_loaidichvu AS ldv\n" +
                        "WHERE\n" +
                        "	ldv.TinhTrang = 0";
            dtRs = Uit.it_MySql.getDataTable(sql, Conn);

            return dtRs;
        }

        public static DataTable getKeyCode(MySqlConnection Conn)
        {
            DataTable dtCode = null;

            string SqlCode = "SELECT\n" +
                            "   cod.KetLuan,\n" +
                            "   cod.ViThe,\n" +
                            "   cod.KeyCode\n" +
                            "FROM\n" +
                            "   keycode AS cod\n" +
                            "ORDER BY\n" +
                            "   cod.KeyCode\n";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);

            return dtCode;
        }

        public static DataTable getNum(string Ma, MySqlConnection Conn)
        {
            DataTable dtCode = null;

            string SqlCode = "SELECT\n" +
                        "	smx.SoMax\n" +
                        "FROM\n" +
                        "	gpb_somax smx\n" +
                        "WHERE\n" +
                        "	smx.IDSo = " + Ma + "\n" +
                        "	AND smx.Nam  = EXTRACT(YEAR FROM NOW())";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);

            return dtCode;
        }

        public static long getMaxNum(int Ma,int LoaiSo, MySqlConnection Conn)
        {
            DataTable dtCode = null;                        
            
            string SqlCode = "SELECT\n" +
                            "	smx.SoMax\n" +
                            "	,smx.Nam\n" +
                            "FROM\n" +
                            "	gpb_somax smx\n" +
                            "WHERE\n" +
                            "	smx.IDSo = " + Ma + "\n" +
                            "	AND smx.LoaiSo = " + LoaiSo + "\n" +
                            "	AND smx.Nam  = EXTRACT(YEAR FROM NOW())";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);
            if (dtCode != null)
            {                
                if (dtCode.Rows.Count > 0)
                {
                    string strCode = "1";
                    //strCode = Ma.ToString();
                    strCode = Uit.it_Parse.ToString(dtCode.Rows[0]["Nam"]).Substring(2, 2);
                    strCode = strCode + Uit.it_Parse.ToInteger(dtCode.Rows[0][0]).ToString("00000");
                    return Uit.it_Parse.ToLong(strCode);
                }
                else
                {
                    SqlCode = "INSERT INTO gpb_somax (IDSo, Nam, LoaiSo,Thang,SoMax)\n" +
                              "VALUES (" + Ma + ", YEAR(NOW()), " + LoaiSo + ",MONTH(NOW()),1);";
                    bool rsnew = Uit.it_MySql.ExecuteNonQuery(SqlCode, Conn);
                    return getMaxNum(Ma, LoaiSo, Conn);
                }
            }            
           
            return 1;
        }

        public static long getMaxNumBN(MySqlConnection Conn)
        {
            DataTable dtCode = null;
            long Rscode = 1;

            string SqlCode = "SELECT \n" +
                            "	MAX(bn.MaBN)\n" +
                            "FROM \n" +
                            "	gpb_benhnhan bn\n" +
                            "WHERE\n" +
                            "	bn.TinhTrang = 0";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);
            if (dtCode != null)
            {
                if (dtCode.Rows.Count > 0)
                {
                    Rscode = Uit.it_Parse.ToLong(dtCode.Rows[0][0]);
                }
            }
            return Rscode;
        }

        public static long get_CurMaxNumBN(MySqlConnection Conn)
        {
            DataTable dtCode = null;
            long Rscode = 1;

            string SqlCode = "SELECT\n" +
                            "	MAX(MaBN)\n" +
                            "FROM \n" +
                            "	gpb_benhnhan\n" +
                            "WHERE\n" +
                            "	TinhTrang = 0";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);
            if (dtCode != null)
            {
                if (dtCode.Rows.Count > 0)
                {
                    Rscode = Uit.it_Parse.ToLong(dtCode.Rows[0][0]);
                }
            }
            return Rscode;
        }

        public static long get_CurMinNumBN(MySqlConnection Conn)
        {
            DataTable dtCode = null;
            long Rscode = 1;

            string SqlCode = "SELECT\n" +
                            "	MIN(MaBN)\n" +
                            "FROM \n" +
                            "	gpb_benhnhan\n" +
                            "WHERE\n" +
                            "	TinhTrang = 0";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);
            if (dtCode != null)
            {
                if (dtCode.Rows.Count > 0)
                {
                    Rscode = Uit.it_Parse.ToLong(dtCode.Rows[0][0]);
                }
            }
            return Rscode;
        }

        public static long get_MinIDGPB(MySqlConnection Conn)
        {
            DataTable dtCode = null;
            long Rscode = 1;

            string SqlCode = "SELECT\n" +
                            "	MIN(IDGPB)\n" +
                            "FROM \n" +
                            "	gpb_ketqua\n" ;

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);
            if (dtCode != null)
            {
                if (dtCode.Rows.Count > 0)
                {
                    Rscode = Uit.it_Parse.ToLong(dtCode.Rows[0][0]);
                }
            }
            return Rscode;
        }

        public static long get_MaxIDGPB(int LoaiKQ, MySqlConnection Conn)
        {
            DataTable dtCode = null;
            long Rscode = 1;

            string SqlCode = "SELECT\n" +
                            "	MAX(IDGPB)\n" +
                            "FROM \n" +
                            "	gpb_ketqua\n" +
                            "WHERE\n"+
                            "	LoaiKQ = " + LoaiKQ + "\n";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);
            if (dtCode != null)
            {
                if (dtCode.Rows.Count > 0)
                {
                    Rscode = Uit.it_Parse.ToLong(dtCode.Rows[0][0]);
                }
            }
            return Rscode;
        }

        public static DataTable getBNByMa(long MaBN, MySqlConnection Conn)
        {
            DataTable dtCode = null;
            long Rscode = 1;

            string SqlCode = "SELECT\n" +
                            "	bn.MaBN,\n" +
                            "	bn.Ho,\n" +
                            "	bn.Ten,\n" +
                            "	bn.NamSinh,\n" +
                            "	0 AS Tuoi,\n" +
                            "	bn.GioiTinh,\n" +
                            "	'' AS Gioi,\n" +
                            "	bn.DiaChi,\n" +
                            "	bn.DienThoai,\n" +
                            "	Now() AS NowDate\n" +
                            "FROM\n" +
                            "	gpb_benhnhan bn\n" +
                            "WHERE\n" +
                            "	bn.MaBN = " + MaBN + "\n" +
                            "	AND TinhTrang = 0";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);
            int NowYear = 0;
            if (dtCode != null)
            {
                for (int i = 0; i < dtCode.Rows.Count; i++)
                {
                    if (Uit.it_Parse.ToInteger(dtCode.Rows[i]["GioiTinh"]) == 1)
                        dtCode.Rows[i]["Gioi"] = "Nam";
                    else
                        dtCode.Rows[i]["Gioi"] = "Nữ";
                    NowYear = Uit.it_Parse.ToDateTime(dtCode.Rows[i]["NowDate"]).Year;
                    dtCode.Rows[i]["Tuoi"] = Uit.it_Parse.ToInteger(dtCode.Rows[i]["NamSinh"]) - NowYear;
                }
            }
            return dtCode;
        }

        public static DataTable getKQByID(long IDGPB,int LoaiKQ, MySqlConnection Conn)
        {
            DataTable dtCode = null;
            long Rscode = 1;

            string SqlCode = "SELECT\n" +
                            "	kq.BSChiDinh,\n" +
                            "	kq.BSDocQK,\n" +
                            "	kq.ChanDoan,\n" +
                            "	kq.CodeKQ,\n" +
                            "	kq.DaiThe,\n" +
                            "	kq.ViThe,\n" +
                            "	kq.KetLuan,\n" +
                            "	kq.ChatBP,\n" +                            
                            "	kq.DVGuiMau,\n" +
                            "	kq.IDGPB,\n" +
                            "	kq.LoaiKQ,\n" +
                            "	kq.MaBN,\n" +
                            "	kq.NgayLM,\n" +
                            "	kq.NgayTra,\n" +
                            "	kq.CDate,\n" +
                            "   kq.IDGPB\n" +
                            "FROM\n" +
                            "	gpb_ketqua kq\n" +
                            "WHERE\n" +
                            "	kq.IDGPB = " + IDGPB + "\n" +
                            "	AND kq.LoaiKQ = " + LoaiKQ + "";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);
           
            return dtCode;
        }


        public static DataTable getKQByMaBN(long MaBN, MySqlConnection Conn)
        {
            DataTable dtCode = new DataTable();
            long Rscode = 1;

            string SqlCode = "SELECT\n" +
                            "	kq.BSChiDinh,\n" +
                            "	kq.BSDocQK,\n" +
                            "	kq.ChanDoan,\n" +
                            "	kq.CodeKQ,\n" +
                            "	kq.DaiThe,\n" +
                            "	kq.ViThe,\n" +
                            "	kq.KetLuan,\n" +
                            "	kq.DVGuiMau,\n" +
                            "	kq.IDGPB,\n" +
                            "	kq.LoaiKQ,\n" +
                            "	kq.MaBN,\n" +
                            "	kq.NgayLM,\n" +
                            "	kq.NgayTra\n" +
                           
                            "FROM\n" +
                            "	gpb_ketqua kq\n" +
                            "WHERE\n" +
                            "	kq.MaBN = " + MaBN + "";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);

            return dtCode;
        }


        public static DataTable getAllBN(MySqlConnection Conn)
        {
            DataTable dtCode = null;
           

            string SqlCode = "SELECT\n" +
                            "	bn.MaBN,\n" +
                            "	bn.Ho,\n" +
                            "	bn.Ten,\n" +
                            "	bn.NamSinh,\n" +
                            "	0 AS Tuoi,\n" +
                            "	bn.GioiTinh,\n" +
                            "	'' AS Gioi,\n" +
                            "	bn.DiaChi,\n" +
                            "	bn.DienThoai,\n" +
                            "	Now() AS NowDate\n" +
                            "FROM\n" +
                            "	gpb_benhnhan bn\n" +
                            "WHERE\n" +                            
                            "	TinhTrang = 0";

            dtCode = Uit.it_MySql.getDataTable(SqlCode, Conn);
            int NowYear = 0;
            if (dtCode != null)
            {
                for (int i = 0; i < dtCode.Rows.Count; i++)
                {
                    if (Uit.it_Parse.ToInteger(dtCode.Rows[i]["GioiTinh"]) == 1)
                        dtCode.Rows[i]["Gioi"] = "Nam";
                    else
                        dtCode.Rows[i]["Gioi"] = "Nữ";
                    NowYear = Uit.it_Parse.ToDateTime(dtCode.Rows[i]["NowDate"]).Year;
                    dtCode.Rows[i]["Tuoi"] = Uit.it_Parse.ToInteger(dtCode.Rows[i]["NamSinh"]) - NowYear;
                }
            }
            return dtCode;
        }
    }
}
