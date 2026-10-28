using System;
using System.Windows.Forms;
using QuanLyHotel.Services;

namespace QuanLyHotel.Forms
{
    public partial class FrmPhongTienNghi : Form
    {
        private readonly PhongTienNghiService _svc = new PhongTienNghiService();
        private DataGridView gridPhong, gridTienNghi, gridPhieuLapDat;
        private TextBox txtSoPhong, txtMaKhuVuc, txtSoNguoi, txtDonGia, txtTrangThaiPhong;
        private TextBox txtMaTN, txtMaLoaiTN, txtSoThuTu, txtTinhTrangTN;
        private TextBox txtSoPhieuLD, txtTienNghiLD, txtPhongLD, txtNVLD, txtGhiChuLD;
        private DateTimePicker dtpNgayLap;

        public FrmPhongTienNghi()
        {
            InitializeComponent();
            TaiLai();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản lý Phòng & Tiện nghi";
            this.Width = 950; this.Height = 650;
            var tab = new TabControl { Dock = DockStyle.Fill };
            tab.TabPages.Add(TabPhong());
            tab.TabPages.Add(TabTienNghi());
            tab.TabPages.Add(TabPhieuLapDat());
            this.Controls.Add(tab);
        }

        private void TaiLai()
        {
            gridPhong.DataSource = _svc.GetPhong();
            gridTienNghi.DataSource = _svc.GetTienNghi();
            gridPhieuLapDat.DataSource = _svc.GetPhieuLapDat();
        }

        private TabPage TabPhong()
        {
            var pg = new TabPage("Phòng");
            gridPhong = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            txtSoPhong = TaoOTextBox("Số phòng", pnl);
            txtMaKhuVuc = TaoOTextBox("Mã khu vực", pnl);
            txtSoNguoi = TaoOTextBox("SL tối đa", pnl);
            txtDonGia = TaoOTextBox("Đơn giá/ngày", pnl);
            txtTrangThaiPhong = TaoOTextBox("Trạng thái", pnl);

            var btnThem = new Button { Text = "Thêm", Width = 80 };
            btnThem.Click += (s, e) => { try { _svc.ThemPhong(txtSoPhong.Text, txtMaKhuVuc.Text, int.Parse(txtSoNguoi.Text), decimal.Parse(txtDonGia.Text)); gridPhong.DataSource = _svc.GetPhong(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            var btnSua = new Button { Text = "Sửa", Width = 80 };
            btnSua.Click += (s, e) => { try { _svc.SuaPhong(txtSoPhong.Text, txtMaKhuVuc.Text, int.Parse(txtSoNguoi.Text), decimal.Parse(txtDonGia.Text), txtTrangThaiPhong.Text); gridPhong.DataSource = _svc.GetPhong(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            var btnXoa = new Button { Text = "Xóa", Width = 80 };
            btnXoa.Click += (s, e) => { try { _svc.XoaPhong(txtSoPhong.Text); gridPhong.DataSource = _svc.GetPhong(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            pnl.Controls.Add(btnThem); pnl.Controls.Add(btnSua); pnl.Controls.Add(btnXoa);

            pg.Controls.Add(pnl); pg.Controls.Add(gridPhong);
            return pg;
        }

        private TabPage TabTienNghi()
        {
            var pg = new TabPage("Tiện nghi");
            gridTienNghi = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            txtMaTN = TaoOTextBox("Mã tiện nghi", pnl);
            txtMaLoaiTN = TaoOTextBox("Mã loại", pnl);
            txtSoThuTu = TaoOTextBox("Số thứ tự", pnl);
            txtTinhTrangTN = TaoOTextBox("Tình trạng", pnl);

            var btnThem = new Button { Text = "Thêm", Width = 80 };
            btnThem.Click += (s, e) => { try { _svc.ThemTienNghi(txtMaTN.Text, txtMaLoaiTN.Text, int.Parse(txtSoThuTu.Text), txtTinhTrangTN.Text); gridTienNghi.DataSource = _svc.GetTienNghi(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            var btnXoa = new Button { Text = "Xóa", Width = 80 };
            btnXoa.Click += (s, e) => { try { _svc.XoaTienNghi(txtMaTN.Text); gridTienNghi.DataSource = _svc.GetTienNghi(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            pnl.Controls.Add(btnThem); pnl.Controls.Add(btnXoa);

            pg.Controls.Add(pnl); pg.Controls.Add(gridTienNghi);
            return pg;
        }

        private TabPage TabPhieuLapDat()
        {
            var pg = new TabPage("Phiếu lắp đặt");
            gridPhieuLapDat = new DataGridView { Dock = DockStyle.Top, Height = 320, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            txtSoPhieuLD = TaoOTextBox("Số phiếu", pnl);
            txtTienNghiLD = TaoOTextBox("Mã tiện nghi", pnl);
            txtPhongLD = TaoOTextBox("Số phòng", pnl);
            pnl.Controls.Add(new Label { Text = "Ngày lắp", AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            dtpNgayLap = new DateTimePicker { Width = 120, Format = DateTimePickerFormat.Short };
            pnl.Controls.Add(dtpNgayLap);
            txtNVLD = TaoOTextBox("Mã NV", pnl);
            txtGhiChuLD = TaoOTextBox("Ghi chú", pnl);

            var btnThem = new Button { Text = "Thêm", Width = 80 };
            btnThem.Click += (s, e) =>
            {
                try
                {
                    _svc.ThemPhieuLapDat(txtSoPhieuLD.Text, txtTienNghiLD.Text, txtPhongLD.Text,
                        dtpNgayLap.Value, "Đã lắp đặt", txtNVLD.Text, txtGhiChuLD.Text);
                    gridPhieuLapDat.DataSource = _svc.GetPhieuLapDat();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi (có thể trùng thiết bị/ngày): " + ex.Message); }
            };
            var btnXoa = new Button { Text = "Xóa", Width = 80 };
            btnXoa.Click += (s, e) => { try { _svc.XoaPhieuLapDat(txtSoPhieuLD.Text); gridPhieuLapDat.DataSource = _svc.GetPhieuLapDat(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            pnl.Controls.Add(btnThem); pnl.Controls.Add(btnXoa);

            pg.Controls.Add(pnl); pg.Controls.Add(gridPhieuLapDat);
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
