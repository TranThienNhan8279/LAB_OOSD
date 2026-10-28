using System;
using System.Windows.Forms;
using QuanLyHotel.Services;

namespace QuanLyHotel.Forms
{
    public partial class FrmThongKe : Form
    {
        private readonly ThongKeService _svc = new ThongKeService();
        private DataGridView gridDoanhThu, gridCongSuat, gridDichVu, gridDenBu;
        private NumericUpDown numNam;

        public FrmThongKe()
        {
            InitializeComponent();
            TaiLai();
        }

        private void InitializeComponent()
        {
            this.Text = "Thống kê - Báo cáo";
            this.Width = 950; this.Height = 650;
            var tab = new TabControl { Dock = DockStyle.Fill };
            tab.TabPages.Add(TabDoanhThu());
            tab.TabPages.Add(TabCongSuat());
            tab.TabPages.Add(TabDichVu());
            tab.TabPages.Add(TabDenBu());
            this.Controls.Add(tab);
        }

        private void TaiLai()
        {
            gridDoanhThu.DataSource = _svc.DoanhThuTheoThang((int)numNam.Value);
            gridCongSuat.DataSource = _svc.CongSuatPhongTheoNgay(DateTime.Today.AddMonths(-1), DateTime.Today);
            gridDichVu.DataSource = _svc.TopDichVuBanChay(5);
            gridDenBu.DataSource = _svc.ThongKeDenBuTheoLoai();
        }

        private TabPage TabDoanhThu()
        {
            var pg = new TabPage("Doanh thu theo tháng");
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(10) };
            pnl.Controls.Add(new Label { Text = "Năm", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            numNam = new NumericUpDown { Width = 80, Minimum = 2000, Maximum = 2100, Value = DateTime.Today.Year };
            pnl.Controls.Add(numNam);
            var btnXem = new Button { Text = "Xem báo cáo", Width = 100 };
            btnXem.Click += (s, e) => gridDoanhThu.DataSource = _svc.DoanhThuTheoThang((int)numNam.Value);
            pnl.Controls.Add(btnXem);

            gridDoanhThu = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            pg.Controls.Add(gridDoanhThu); pg.Controls.Add(pnl);
            return pg;
        }

        private TabPage TabCongSuat()
        {
            var pg = new TabPage("Công suất phòng");
            gridCongSuat = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            pg.Controls.Add(gridCongSuat);
            return pg;
        }

        private TabPage TabDichVu()
        {
            var pg = new TabPage("Dịch vụ bán chạy");
            gridDichVu = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            pg.Controls.Add(gridDichVu);
            return pg;
        }

        private TabPage TabDenBu()
        {
            var pg = new TabPage("Thống kê đền bù");
            gridDenBu = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            pg.Controls.Add(gridDenBu);
            return pg;
        }
    }
}
