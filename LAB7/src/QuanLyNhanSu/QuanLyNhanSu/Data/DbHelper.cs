using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyNhanSu.Data
{
    // Lỗi nghiệp vụ: Form hiển thị nguyên văn nội dung cho người dùng
    public class NghiepVuException : Exception
    {
        public NghiepVuException(string message) : base(message) { }
    }

    // Tầng Data: chỉ làm việc với SQL Server
    public static class DbHelper
    {
        static string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["Db"].ConnectionString; }
        }

        public static SqlConnection Open()
        {
            var c = new SqlConnection(ConnStr);
            c.Open();
            return c;
        }

        public static SqlParameter P(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        public static DataTable Query(string sql, params SqlParameter[] ps)
        {
            using (var c = Open())
            using (var cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                return dt;
            }
        }

        public static int Exec(string sql, params SqlParameter[] ps)
        {
            using (var c = Open())
            using (var cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                return cmd.ExecuteNonQuery();
            }
        }

        public static DataTable Query(SqlTransaction tx, string sql, params SqlParameter[] ps)
        {
            using (var cmd = new SqlCommand(sql, tx.Connection, tx))
            {
                cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);
                return dt;
            }
        }

        public static int Exec(SqlTransaction tx, string sql, params SqlParameter[] ps)
        {
            using (var cmd = new SqlCommand(sql, tx.Connection, tx))
            {
                cmd.Parameters.AddRange(ps);
                return cmd.ExecuteNonQuery();
            }
        }

        public static object Scalar(SqlTransaction tx, string sql, params SqlParameter[] ps)
        {
            using (var cmd = new SqlCommand(sql, tx.Connection, tx))
            {
                cmd.Parameters.AddRange(ps);
                object r = cmd.ExecuteScalar();
                return r == DBNull.Value ? null : r;
            }
        }

        // Sinh mã tiếp theo, ví dụ NextId(tx, "HoaDon", "MaHD", "HD") -> HD0005
        public static string NextId(SqlTransaction tx, string table, string col, string prefix)
        {
            string sql = "SELECT ISNULL(MAX(CAST(SUBSTRING(" + col + "," + (prefix.Length + 1) + ",20) AS INT)),0)+1 " +
                         "FROM " + table + " WITH (UPDLOCK, HOLDLOCK) WHERE " + col + " LIKE '" + prefix + "[0-9]%'";
            int n = Convert.ToInt32(Scalar(tx, sql));
            return prefix + n.ToString().PadLeft(4, '0');
        }
    }
}
