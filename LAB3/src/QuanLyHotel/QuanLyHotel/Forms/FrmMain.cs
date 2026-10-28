using System;
using System.Windows.Forms;
using QuanLyHotel.Data;

namespace QuanLyHotel.Forms
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.IsMdiContainer = true;
            this.Text = "Hệ thống Quản lý Khách sạn - QuanLyHotel";
            this.WindowState = FormWindowState.Maximized;
            this.StartPosition = FormStartPosition.CenterScreen;

            var menu = new MenuStrip();

            var mnuDanhMuc = new ToolStripMenuItem("Danh mục");
            mnuDanhMuc.Click += (s, e) => MoForm(new FrmDanhMuc());

            var mnuPhong = new ToolStripMenuItem("Phòng && Tiện nghi");
            mnuPhong.Click += (s, e) => MoForm(new FrmPhongTienNghi());

            var mnuDatPhong = new ToolStripMenuItem("Đặt phòng");
            mnuDatPhong.Click += (s, e) => MoForm(new FrmDatPhong());

            var mnuDichVu = new ToolStripMenuItem("Dịch vụ");
            mnuDichVu.Click += (s, e) => MoForm(new FrmDichVu());

            var mnuTraPhong = new ToolStripMenuItem("Trả phòng && Hóa đơn");
            mnuTraPhong.Click += (s, e) => MoForm(new FrmTraPhong());

            var mnuThongKe = new ToolStripMenuItem("Thống kê");
            mnuThongKe.Click += (s, e) => MoForm(new FrmThongKe());

            var mnuThoat = new ToolStripMenuItem("Thoát");
            mnuThoat.Click += (s, e) => this.Close();

            menu.Items.AddRange(new ToolStripItem[]
            {
                mnuDanhMuc, mnuPhong, mnuDatPhong, mnuDichVu, mnuTraPhong, mnuThongKe, mnuThoat
            });

            this.MainMenuStrip = menu;
            this.Controls.Add(menu);

            var lblTrangThai = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 24,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            string msg;
            lblTrangThai.Text = DbHelper.TestConnection(out msg) ? msg : msg;
            lblTrangThai.ForeColor = msg.StartsWith("Kết nối") ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            this.Controls.Add(lblTrangThai);
        }

        private void MoForm(Form f)
        {
            f.MdiParent = this;
            f.WindowState = FormWindowState.Maximized;
            f.Show();
        }
    }
}
