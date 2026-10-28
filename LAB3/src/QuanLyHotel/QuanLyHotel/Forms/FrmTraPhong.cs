using System;
using System.Windows.Forms;
using QuanLyHotel.Services;

namespace QuanLyHotel.Forms
{
    public partial class FrmTraPhong : Form
    {
        private readonly TraPhongService _svc = new TraPhongService();
        private readonly DichVuService _dvSvc = new DichVuService();
        private TextBox txtSoPhieuDat, txtNV, txtSoNgay, txtTienPhong, txtTienDV, txtTongCong;
        private DataGridView gridHoaDon;
        private TextBox txtSoHoaDon, txtSoTienTT;
        private ComboBox cboHinhThuc;

        // Đền bù
        private TextBox txtSoPhieuDB, txtSoPhongDB, txtNVDB, txtTienNghiDB, txtMucDoDB, txtSoTienDB;

        public FrmTraPhong()
        {
            InitializeComponent();
            gridHoaDon.DataSource = _svc.GetHoaDonChuaThanhToan();
        }

        private void InitializeComponent()
        {
            this.Text = "Trả phòng & Hóa đơn";
            this.Width = 1000; this.Height = 750;
            var tab = new TabControl { Dock = DockStyle.Fill };
            tab.TabPages.Add(TabLapHoaDon());
            tab.TabPages.Add(TabThanhToan());
            tab.TabPages.Add(TabDenBu());
            this.Controls.Add(tab);
        }

        private TabPage TabLapHoaDon()
        {
            var pg = new TabPage("Trả phòng & Lập hóa đơn");
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 260, Padding = new Padding(10), FlowDirection = FlowDirection.TopDown };

            var row1 = new FlowLayoutPanel { AutoSize = true };
            row1.Controls.Add(new Label { Text = "Số phiếu đặt phòng", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtSoPhieuDat = new TextBox { Width = 100 }; row1.Controls.Add(txtSoPhieuDat);
            row1.Controls.Add(new Label { Text = "Mã NV", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtNV = new TextBox { Width = 80 }; row1.Controls.Add(txtNV);
            var btnTinh = new Button { Text = "Tính tiền phòng + dịch vụ", Width = 180 };
            row1.Controls.Add(btnTinh);

            var row2 = new FlowLayoutPanel { AutoSize = true };
            row2.Controls.Add(new Label { Text = "Số ngày", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtSoNgay = new TextBox { Width = 60, ReadOnly = true }; row2.Controls.Add(txtSoNgay);
            row2.Controls.Add(new Label { Text = "Tiền phòng", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtTienPhong = new TextBox { Width = 100, ReadOnly = true }; row2.Controls.Add(txtTienPhong);
            row2.Controls.Add(new Label { Text = "Tiền dịch vụ", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtTienDV = new TextBox { Width = 100, ReadOnly = true }; row2.Controls.Add(txtTienDV);
            row2.Controls.Add(new Label { Text = "Tổng cộng", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtTongCong = new TextBox { Width = 100, ReadOnly = true }; row2.Controls.Add(txtTongCong);

            btnTinh.Click += (s, e) =>
            {
                try
                {
                    int soNgay;
                    decimal tienPhong = _svc.TinhTienPhong(txtSoPhieuDat.Text, out soNgay);
                    decimal tienDV = _dvSvc.TongTienDichVuTheoPhieuDat(txtSoPhieuDat.Text);
                    txtSoNgay.Text = soNgay.ToString();
                    txtTienPhong.Text = tienPhong.ToString("N0");
                    txtTienDV.Text = tienDV.ToString("N0");
                    txtTongCong.Text = (tienPhong + tienDV).ToString("N0");
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };

            var btnLapHD = new Button { Text = "Lập hóa đơn & Trả phòng", Width = 200, Height = 32 };
            btnLapHD.Click += (s, e) =>
            {
                try
                {
                    int soNgay = int.Parse(txtSoNgay.Text);
                    decimal tienPhong = decimal.Parse(txtTienPhong.Text.Replace(",", ""));
                    decimal tienDV = decimal.Parse(txtTienDV.Text.Replace(",", ""));
                    string soHD = _svc.LapHoaDonVaTraPhong(txtSoPhieuDat.Text, txtNV.Text, soNgay, tienPhong, tienDV);
                    MessageBox.Show("Đã lập hóa đơn: " + soHD);
                    gridHoaDon.DataSource = _svc.GetHoaDonChuaThanhToan();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            };

            pnl.Controls.Add(row1); pnl.Controls.Add(row2); pnl.Controls.Add(btnLapHD);
            pg.Controls.Add(pnl);
            return pg;
        }

        private TabPage TabThanhToan()
        {
            var pg = new TabPage("Thanh toán hóa đơn");
            gridHoaDon = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            gridHoaDon.CellClick += (s, e) =>
            {
                if (gridHoaDon.CurrentRow == null) return;
                txtSoHoaDon.Text = gridHoaDon.CurrentRow.Cells["SoHoaDon"].Value.ToString();
                txtSoTienTT.Text = gridHoaDon.CurrentRow.Cells["TongTien"].Value.ToString();
            };

            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            pnl.Controls.Add(new Label { Text = "Số hóa đơn", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtSoHoaDon = new TextBox { Width = 100 }; pnl.Controls.Add(txtSoHoaDon);
            pnl.Controls.Add(new Label { Text = "Số tiền", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtSoTienTT = new TextBox { Width = 100 }; pnl.Controls.Add(txtSoTienTT);
            pnl.Controls.Add(new Label { Text = "Hình thức", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            cboHinhThuc = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            cboHinhThuc.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Thẻ", "Ví điện tử" });
            cboHinhThuc.SelectedIndex = 0;
            pnl.Controls.Add(cboHinhThuc);

            var btnTT = new Button { Text = "Xác nhận thanh toán", Width = 150 };
            btnTT.Click += (s, e) =>
            {
                try
                {
                    _svc.ThanhToanHoaDon(txtSoHoaDon.Text, cboHinhThuc.Text, decimal.Parse(txtSoTienTT.Text));
                    MessageBox.Show("Thanh toán thành công.");
                    gridHoaDon.DataSource = _svc.GetHoaDonChuaThanhToan();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };
            pnl.Controls.Add(btnTT);

            pg.Controls.Add(pnl); pg.Controls.Add(gridHoaDon);
            return pg;
        }

        private TabPage TabDenBu()
        {
            var pg = new TabPage("Lập phiếu đền bù");
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            txtSoPhieuDB = TaoOTextBox("Số phiếu đặt", pnl);
            txtSoPhongDB = TaoOTextBox("Số phòng", pnl);
            txtNVDB = TaoOTextBox("Mã NV", pnl);
            txtTienNghiDB = TaoOTextBox("Mã tiện nghi", pnl);
            txtMucDoDB = TaoOTextBox("Mức độ thiệt hại", pnl);
            txtSoTienDB = TaoOTextBox("Số tiền đền bù", pnl);

            var btnLap = new Button { Text = "Lập phiếu đền bù", Width = 150 };
            btnLap.Click += (s, e) =>
            {
                try
                {
                    string soPhieu = _svc.TaoSoPhieuDenBuMoi();
                    decimal soTien = decimal.Parse(txtSoTienDB.Text);
                    _svc.TaoPhieuDenBu(soPhieu, txtSoPhieuDB.Text, txtSoPhongDB.Text, txtNVDB.Text, soTien);
                    _svc.ThemChiTietDenBu(soPhieu, txtTienNghiDB.Text, txtMucDoDB.Text, soTien);
                    MessageBox.Show("Đã lập phiếu đền bù: " + soPhieu);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };
            pnl.Controls.Add(btnLap);

            pg.Controls.Add(pnl);
            return pg;
        }

        private TextBox TaoOTextBox(string nhan, FlowLayoutPanel pnl)
        {
            pnl.Controls.Add(new Label { Text = nhan, AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            var tb = new TextBox { Width = 110 };
            pnl.Controls.Add(tb);
            return tb;
        }
    }
}
