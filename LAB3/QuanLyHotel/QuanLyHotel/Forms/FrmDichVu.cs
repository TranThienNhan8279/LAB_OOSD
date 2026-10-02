using System;
using System.Windows.Forms;
using QuanLyHotel.Services;

namespace QuanLyHotel.Forms
{
    public partial class FrmDichVu : Form
    {
        private readonly DichVuService _svc = new DichVuService();
        private DataGridView gridPhieu, gridChiTiet;
        private TextBox txtSoPhieuDat, txtSoPhong, txtNV, txtMaDV, txtSoLuong, txtDonGia;
        private DateTimePicker dtpNgay;
        private string _soPhieuHienTai = "";

        public FrmDichVu()
        {
            InitializeComponent();
            gridPhieu.DataSource = _svc.GetPhieuSuDungDV();
        }

        private void InitializeComponent()
        {
            this.Text = "Sử dụng dịch vụ";
            this.Width = 1000; this.Height = 700;

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(10) };
            top.Controls.Add(new Label { Text = "Số phiếu đặt phòng", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtSoPhieuDat = new TextBox { Width = 90 }; top.Controls.Add(txtSoPhieuDat);
            top.Controls.Add(new Label { Text = "Số phòng", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtSoPhong = new TextBox { Width = 70 }; top.Controls.Add(txtSoPhong);
            top.Controls.Add(new Label { Text = "Ngày sử dụng", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            dtpNgay = new DateTimePicker { Width = 110, Format = DateTimePickerFormat.Short }; top.Controls.Add(dtpNgay);
            top.Controls.Add(new Label { Text = "Mã NV", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtNV = new TextBox { Width = 70 }; top.Controls.Add(txtNV);
            var btnLapPhieu = new Button { Text = "Lập phiếu sử dụng DV", Width = 150 };
            top.Controls.Add(btnLapPhieu);

            btnLapPhieu.Click += (s, e) =>
            {
                try
                {
                    _soPhieuHienTai = _svc.TaoSoPhieuSDDVMoi();
                    _svc.TaoPhieuSuDungDV(_soPhieuHienTai, txtSoPhieuDat.Text, txtSoPhong.Text, dtpNgay.Value, txtNV.Text);
                    MessageBox.Show("Đã lập phiếu: " + _soPhieuHienTai + ". Tiếp tục thêm dịch vụ bên dưới.");
                    gridPhieu.DataSource = _svc.GetPhieuSuDungDV();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi (có thể trùng phòng/ngày): " + ex.Message); }
            };

            var mid = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
            mid.Controls.Add(new Label { Text = "Mã dịch vụ", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtMaDV = new TextBox { Width = 70 }; mid.Controls.Add(txtMaDV);
            mid.Controls.Add(new Label { Text = "Số lượng", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtSoLuong = new TextBox { Width = 60 }; mid.Controls.Add(txtSoLuong);
            mid.Controls.Add(new Label { Text = "Đơn giá", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtDonGia = new TextBox { Width = 80 }; mid.Controls.Add(txtDonGia);
            var btnThemCT = new Button { Text = "Thêm dịch vụ vào phiếu", Width = 160 };
            mid.Controls.Add(btnThemCT);

            btnThemCT.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(_soPhieuHienTai)) { MessageBox.Show("Lập phiếu sử dụng DV trước."); return; }
                try
                {
                    _svc.ThemChiTietDichVu(_soPhieuHienTai, txtMaDV.Text, int.Parse(txtSoLuong.Text), decimal.Parse(txtDonGia.Text));
                    gridChiTiet.DataSource = _svc.GetChiTiet(_soPhieuHienTai);
                    gridPhieu.DataSource = _svc.GetPhieuSuDungDV();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };

            gridPhieu = new DataGridView { Dock = DockStyle.Top, Height = 250, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            gridPhieu.CellClick += (s, e) =>
            {
                if (gridPhieu.CurrentRow == null) return;
                _soPhieuHienTai = gridPhieu.CurrentRow.Cells["SoPhieuSDDV"].Value.ToString();
                gridChiTiet.DataSource = _svc.GetChiTiet(_soPhieuHienTai);
            };

            gridChiTiet = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            this.Controls.Add(gridChiTiet);
            this.Controls.Add(gridPhieu);
            this.Controls.Add(mid);
            this.Controls.Add(top);
        }
    }
}
