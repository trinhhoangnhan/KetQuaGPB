using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace BUS
{
    public static class UpdateData
    {
        public static bool updSoMax(int Ma, int LoaiSo, int MaxNum, MySqlConnection Conn)
        {
            bool dtRs = false;

            string sql = "UPDATE gpb_somax\n" +
                        "SET SoMax = " + MaxNum + "\n" +
                        "WHERE\n" +
                        "	IDSo = " + Ma + "\n" +
                        "AND LoaiSo = " + LoaiSo + "\n" +
                        "AND Nam = EXTRACT(YEAR FROM NOW())";
            dtRs = Uit.it_MySql.ExecuteNonQuery(sql, Conn);

            return dtRs;
        }
    }
}
