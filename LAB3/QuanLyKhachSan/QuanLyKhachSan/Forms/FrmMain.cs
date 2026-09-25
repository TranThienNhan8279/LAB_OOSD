using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyKhachSan.Data;

namespace QuanLyKhachSan.Forms
{
    public partial class FrmMain: Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FrmDanhMuc form = new FrmDanhMuc())
            {
                form.ShowDialog(this);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (FrmDatPhong form = new FrmDatPhong())
            {
                form.ShowDialog(this);
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }

        private void btnPhong_Click(object sender, EventArgs e)
        {
            using (FrmPhongTienNghi form = new FrmPhongTienNghi())
            {
                form.ShowDialog(this);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult ketQua = MessageBox.Show(
       "Bạn có thực sự muốn thoát?",
       "Xác nhận",
       MessageBoxButtons.YesNo,
       MessageBoxIcon.Question
   );

            if (ketQua == DialogResult.Yes)
            {
                Close();
            }
        }

        private void btnDichVu_Click(object sender, EventArgs e)
        {
            using (FrmDichVu form = new FrmDichVu())
            {
                form.ShowDialog(this);
            }
        }

        private void btnTraPhong_Click(object sender, EventArgs e)
        {
            using (FrmTraPhong form = new FrmTraPhong())
            {
                form.ShowDialog(this);
            }
        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            using (FrmThongKe form = new FrmThongKe())
            {
                form.ShowDialog(this);
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = Db.Query("SELECT * FROM NhanVien");

                MessageBox.Show(
                    "Kết nối thành công!\nSố nhân viên: " + dt.Rows.Count
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Kết nối thất bại!\n\n" + ex.Message
                );
            }
        }
    }
}
