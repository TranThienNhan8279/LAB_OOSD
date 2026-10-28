using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using QuanLyHotel.Services;

namespace QuanLyHotel.Forms
{
    public partial class FrmDatPhong : Form
    {
        private readonly DatPhongService _svc = new DatPhongService();
        private DataGridView gridKhach, gridPhongTrong, gridChiTietChon, gridPhieuDat;
        private TextBox txtMaKhach, txtHoTen, txtCmnd, txtQuocTich, txtSdt, txtSoNguoi, txtTienCoc;
        private ComboBox cboKenhDat, cboNhanVien;
        private DateTimePicker dtpNhan, dtpTra;
        private readonly DataTable _chiTietChon = new DataTable();

        public FrmDatPhong()
        {
            InitializeComponent();
            gridKhach.DataSource = _svc.GetKhachHang();
            gridPhieuDat.DataSource = _svc.GetPhieuDatPhong();
        }

        private void InitializeComponent()
        {
            this.Text = "Đặt phòng";
            this.Width = 1100; this.Height = 750;
            var tab = new TabControl { Dock = DockStyle.Fill };
            tab.TabPages.Add(TabKhachHang());
            tab.TabPages.Add(TabDatPhong());
            tab.TabPages.Add(TabDanhSachPhieu());
            this.Controls.Add(tab);
        }

        // ---------------- Tab Khach hang ----------------
        private TabPage TabKhachHang()
        {
            var pg = new TabPage("Khách hàng");
            gridKhach = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            txtMaKhach = TaoOTextBox("Mã khách", pnl);
            txtHoTen = TaoOTextBox("Họ tên", pnl);
            txtCmnd = TaoOTextBox("Số CMND/CCCD", pnl);
            txtQuocTich = TaoOTextBox("Quốc tịch", pnl);
            txtSdt = TaoOTextBox("SĐT", pnl);

            var btnThem = new Button { Text = "Thêm khách", Width = 100 };
            btnThem.Click += (s, e) =>
            {
                try { _svc.ThemKhachHang(txtMaKhach.Text, txtHoTen.Text, txtCmnd.Text, txtQuocTich.Text, txtSdt.Text); gridKhach.DataSource = _svc.GetKhachHang(); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };
            pnl.Controls.Add(btnThem);

            pg.Controls.Add(pnl); pg.Controls.Add(gridKhach);
            return pg;
        }

        // ---------------- Tab Dat phong ----------------
        private TabPage TabDatPhong()
        {
            var pg = new TabPage("Lập phiếu đặt phòng");
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(10) };

            top.Controls.Add(new Label { Text = "Mã khách", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            var txtMaKhachDat = new TextBox { Width = 100, Name = "txtMaKhachDat" };
            top.Controls.Add(txtMaKhachDat);

            top.Controls.Add(new Label { Text = "Mã NV lễ tân", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            var txtNVDat = new TextBox { Width = 80 };
            top.Controls.Add(txtNVDat);

            top.Controls.Add(new Label { Text = "Ngày nhận", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            dtpNhan = new DateTimePicker { Width = 110, Format = DateTimePickerFormat.Short };
            top.Controls.Add(dtpNhan);

            top.Controls.Add(new Label { Text = "Ngày trả dự kiến", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            dtpTra = new DateTimePicker { Width = 110, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(1) };
            top.Controls.Add(dtpTra);

            top.Controls.Add(new Label { Text = "Tiền cọc", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtTienCoc = new TextBox { Width = 80, Text = "0" };
            top.Controls.Add(txtTienCoc);

            top.Controls.Add(new Label { Text = "Kênh đặt", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            cboKenhDat = new ComboBox { Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cboKenhDat.Items.AddRange(new object[] { "Trực tiếp", "Điện thoại", "Website" });
            cboKenhDat.SelectedIndex = 0;
            top.Controls.Add(cboKenhDat);

            var btnTimPhong = new Button { Text = "Tìm phòng trống", Width = 110 };
            top.Controls.Add(btnTimPhong);

            gridPhongTrong = new DataGridView { Dock = DockStyle.Top, Height = 180, Top = 95, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            btnTimPhong.Click += (s, e) => gridPhongTrong.DataSource = _svc.GetPhongTrongTheoKhoangNgay(dtpNhan.Value, dtpTra.Value);

            var pnl2 = new FlowLayoutPanel { Top = 275, Height = 40, Padding = new Padding(10) };
            pnl2.Controls.Add(new Label { Text = "Số người ở phòng đã chọn", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            txtSoNguoi = new TextBox { Width = 60, Text = "1" };
            pnl2.Controls.Add(txtSoNguoi);
            var btnThemVaoPhieu = new Button { Text = "Thêm phòng vào phiếu", Width = 150 };
            pnl2.Controls.Add(btnThemVaoPhieu);

            _chiTietChon.Columns.Add("SoPhong");
            _chiTietChon.Columns.Add("SoNguoi", typeof(int));
            gridChiTietChon = new DataGridView { Top = 320, Height = 120, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, DataSource = _chiTietChon };

            btnThemVaoPhieu.Click += (s, e) =>
            {
                if (gridPhongTrong.CurrentRow == null) { MessageBox.Show("Chọn 1 phòng trong danh sách trên."); return; }
                string soPhong = gridPhongTrong.CurrentRow.Cells["SoPhong"].Value.ToString();
                _chiTietChon.Rows.Add(soPhong, int.Parse(txtSoNguoi.Text));
            };

            var btnTaoPhieu = new Button { Text = "Tạo phiếu đặt phòng", Top = 450, Width = 160, Height = 30 };
            btnTaoPhieu.Click += (s, e) =>
            {
                try
                {
                    if (_chiTietChon.Rows.Count == 0) { MessageBox.Show("Chưa chọn phòng nào."); return; }
                    var soPhong = new List<string>(); var soNguoi = new List<int>();
                    foreach (DataRow r in _chiTietChon.Rows) { soPhong.Add(r["SoPhong"].ToString()); soNguoi.Add(Convert.ToInt32(r["SoNguoi"])); }
                    string soPhieu = _svc.TaoSoPhieuDatMoi();
                    _svc.TaoPhieuDatPhong(soPhieu, txtMaKhachDat.Text, txtNVDat.Text, dtpNhan.Value, dtpTra.Value,
                        decimal.Parse(txtTienCoc.Text), cboKenhDat.Text, soPhong.ToArray(), soNguoi.ToArray());
                    MessageBox.Show("Tạo phiếu đặt phòng thành công: " + soPhieu);
                    _chiTietChon.Rows.Clear();
                    gridPhieuDat.DataSource = _svc.GetPhieuDatPhong();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            };

            pg.Controls.Add(btnTaoPhieu);
            pg.Controls.Add(gridChiTietChon);
            pg.Controls.Add(pnl2);
            pg.Controls.Add(gridPhongTrong);
            pg.Controls.Add(top);
            return pg;
        }

        // ---------------- Tab danh sach phieu / nhan phong ----------------
        private TabPage TabDanhSachPhieu()
        {
            var pg = new TabPage("Danh sách phiếu đặt");
            gridPhieuDat = new DataGridView { Dock = DockStyle.Top, Height = 400, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            var btnNhanPhong = new Button { Text = "Nhận phòng (Check-in)", Width = 160 };
            btnNhanPhong.Click += (s, e) =>
            {
                if (gridPhieuDat.CurrentRow == null) { MessageBox.Show("Chọn 1 phiếu."); return; }
                string sp = gridPhieuDat.CurrentRow.Cells["SoPhieuDat"].Value.ToString();
                try { _svc.NhanPhong(sp); gridPhieuDat.DataSource = _svc.GetPhieuDatPhong(); MessageBox.Show("Đã nhận phòng."); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };
            var btnHuy = new Button { Text = "Hủy phiếu", Width = 100 };
            btnHuy.Click += (s, e) =>
            {
                if (gridPhieuDat.CurrentRow == null) { MessageBox.Show("Chọn 1 phiếu."); return; }
                string sp = gridPhieuDat.CurrentRow.Cells["SoPhieuDat"].Value.ToString();
                try { _svc.HuyPhieuDat(sp); gridPhieuDat.DataSource = _svc.GetPhieuDatPhong(); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };
            var btnLamMoi = new Button { Text = "Làm mới", Width = 90 };
            btnLamMoi.Click += (s, e) => gridPhieuDat.DataSource = _svc.GetPhieuDatPhong();

            pnl.Controls.Add(btnNhanPhong); pnl.Controls.Add(btnHuy); pnl.Controls.Add(btnLamMoi);
            pg.Controls.Add(pnl); pg.Controls.Add(gridPhieuDat);
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
