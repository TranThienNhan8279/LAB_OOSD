using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyHotel.Data
{
    /// <summary>
    /// Lớp tiện ích dùng chung để kết nối và thao tác với CSDL QuanLyHotelDB.
    /// Chuỗi kết nối lấy từ App.config (key "QuanLyHotelDB").
    /// </summary>
    public static class DbHelper
    {
        private static readonly string ConnStr =
            ConfigurationManager.ConnectionStrings["QuanLyHotelDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnStr);
        }

        /// <summary>Kiểm tra kết nối tới SQL Server có thành công không.</summary>
        public static bool TestConnection(out string message)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                }
                message = "Kết nối CSDL thành công.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Lỗi kết nối: " + ex.Message;
                return false;
            }
        }

        /// <summary>Thực thi câu lệnh SELECT, trả về DataTable.</summary>
        public static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>Thực thi INSERT/UPDATE/DELETE, trả về số dòng ảnh hưởng.</summary>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Thực thi câu lệnh trả về 1 giá trị (COUNT, SUM, SCOPE_IDENTITY...).</summary>
        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
