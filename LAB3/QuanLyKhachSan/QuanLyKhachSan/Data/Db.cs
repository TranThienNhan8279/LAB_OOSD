using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using QuanLyKhachSan.Data;
using System.Windows.Forms;

namespace QuanLyKhachSan.Data
{
    public static class Db
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager
                    .ConnectionStrings["QuanLyKhachSanDB"]
                    .ConnectionString;
            }
        }

        public static SqlConnection OpenConnection()
        {
            SqlConnection cn = new SqlConnection(ConnectionString);
            cn.Open();
            return cn;
        }

        public static DataTable Query(string sql, params SqlParameter[] ps)
        {
            using (SqlConnection cn = OpenConnection())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                if (ps != null)
                {
                    cmd.Parameters.AddRange(ps);
                }

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static int Execute(string sql, params SqlParameter[] ps)
        {
            using (SqlConnection cn = OpenConnection())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                if (ps != null)
                {
                    cmd.Parameters.AddRange(ps);
                }

                return cmd.ExecuteNonQuery();
            }
        }

        public static object Scalar(string sql, params SqlParameter[] ps)
        {
            using (SqlConnection cn = OpenConnection())
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                if (ps != null)
                {
                    cmd.Parameters.AddRange(ps);
                }

                return cmd.ExecuteScalar();
            }
        }
    }
}
