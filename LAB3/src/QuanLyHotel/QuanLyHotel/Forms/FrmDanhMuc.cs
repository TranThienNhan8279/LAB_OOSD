using System;
using System.Windows.Forms;
using QuanLyHotel.Services;

namespace QuanLyHotel.Forms
{
    public partial class FrmDanhMuc : Form
    {
        private readonly DanhMucService _svc = new DanhMucService();
        private TabControl tab;

        // Khu vuc
        private DataGridView gridKhuVuc; private TextBox txtKVMa, txtKVTen;
        // Nhan vien
        private DataGridView gridNV; private TextBox txtNVMa, txtNVHoTen, txtNVVaiTro, txtNVSdt;
        // Loai tien nghi
        private DataGridView gridLoaiTN; private TextBox txtLoaiTNMa, txtLoaiTNTen;
        // Dich vu
        private DataGridView gridDV; private TextBox txtDVMa, txtDVTen, txtDVDvt, txtDVGia;
        // Quy dinh den bu
        private DataGridView gridQD; private TextBox txtQDMa, txtQDLoai, txtQDMucDo, txtQDTien;

        public FrmDanhMuc()
        {
            InitializeComponent();
            TaiTatCa();
        }

        private void InitializeComponent()
        {
            this.Text = "Danh mục hệ thống";
            this.Width = 900; this.Height = 600;

            tab = new TabControl { Dock = DockStyle.Fill };
            tab.TabPages.Add(TaoTabKhuVuc());
            tab.TabPages.Add(TaoTabNhanVien());
            tab.TabPages.Add(TaoTabLoaiTienNghi());
            tab.TabPages.Add(TaoTabDichVu());
            tab.TabPages.Add(TaoTabQuyDinhDenBu());

            this.Controls.Add(tab);
        }

        private void TaiTatCa()
        {
            gridKhuVuc.DataSource = _svc.GetKhuVuc();
            gridNV.DataSource = _svc.GetNhanVien();
            gridLoaiTN.DataSource = _svc.GetLoaiTienNghi();
            gridDV.DataSource = _svc.GetDichVu();
            gridQD.DataSource = _svc.GetQuyDinhDenBu();
        }

        // ---------------- Khu vuc ----------------
        private TabPage TaoTabKhuVuc()
        {
            var pg = new TabPage("Khu vực");
            gridKhuVuc = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            txtKVMa = TaoOTextBox("Mã khu vực", pnl);
            txtKVTen = TaoOTextBox("Tên khu vực", pnl);

            var btnThem = new Button { Text = "Thêm", Width = 80 };
            btnThem.Click += (s, e) => { try { _svc.ThemKhuVuc(txtKVMa.Text, txtKVTen.Text); gridKhuVuc.DataSource = _svc.GetKhuVuc(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            var btnSua = new Button { Text = "Sửa", Width = 80 };
            btnSua.Click += (s, e) => { try { _svc.SuaKhuVuc(txtKVMa.Text, txtKVTen.Text); gridKhuVuc.DataSource = _svc.GetKhuVuc(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            var btnXoa = new Button { Text = "Xóa", Width = 80 };
            btnXoa.Click += (s, e) => { try { _svc.XoaKhuVuc(txtKVMa.Text); gridKhuVuc.DataSource = _svc.GetKhuVuc(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            pnl.Controls.Add(btnThem); pnl.Controls.Add(btnSua); pnl.Controls.Add(btnXoa);

            pg.Controls.Add(pnl); pg.Controls.Add(gridKhuVuc);
            return pg;
        }

        // ---------------- Nhan vien ----------------
        private TabPage TaoTabNhanVien()
        {
            var pg = new TabPage("Nhân viên");
            gridNV = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            txtNVMa = TaoOTextBox("Mã NV", pnl);
            txtNVHoTen = TaoOTextBox("Họ tên", pnl);
            txtNVVaiTro = TaoOTextBox("Vai trò", pnl);
            txtNVSdt = TaoOTextBox("SĐT", pnl);

            var btnThem = new Button { Text = "Thêm", Width = 80 };
            btnThem.Click += (s, e) => { try { _svc.ThemNhanVien(txtNVMa.Text, txtNVHoTen.Text, txtNVVaiTro.Text, txtNVSdt.Text); gridNV.DataSource = _svc.GetNhanVien(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            var btnSua = new Button { Text = "Sửa", Width = 80 };
            btnSua.Click += (s, e) => { try { _svc.SuaNhanVien(txtNVMa.Text, txtNVHoTen.Text, txtNVVaiTro.Text, txtNVSdt.Text); gridNV.DataSource = _svc.GetNhanVien(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            var btnXoa = new Button { Text = "Xóa", Width = 80 };
            btnXoa.Click += (s, e) => { try { _svc.XoaNhanVien(txtNVMa.Text); gridNV.DataSource = _svc.GetNhanVien(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            pnl.Controls.Add(btnThem); pnl.Controls.Add(btnSua); pnl.Controls.Add(btnXoa);

            pg.Controls.Add(pnl); pg.Controls.Add(gridNV);
            return pg;
        }

        // ---------------- Loai tien nghi ----------------
        private TabPage TaoTabLoaiTienNghi()
        {
            var pg = new TabPage("Loại tiện nghi");
            gridLoaiTN = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            txtLoaiTNMa = TaoOTextBox("Mã loại", pnl);
            txtLoaiTNTen = TaoOTextBox("Tên loại", pnl);

            var btnThem = new Button { Text = "Thêm", Width = 80 };
            btnThem.Click += (s, e) => { try { _svc.ThemLoaiTienNghi(txtLoaiTNMa.Text, txtLoaiTNTen.Text); gridLoaiTN.DataSource = _svc.GetLoaiTienNghi(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            var btnXoa = new Button { Text = "Xóa", Width = 80 };
            btnXoa.Click += (s, e) => { try { _svc.XoaLoaiTienNghi(txtLoaiTNMa.Text); gridLoaiTN.DataSource = _svc.GetLoaiTienNghi(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            pnl.Controls.Add(btnThem); pnl.Controls.Add(btnXoa);

            pg.Controls.Add(pnl); pg.Controls.Add(gridLoaiTN);
            return pg;
        }

        // ---------------- Dich vu ----------------
        private TabPage TaoTabDichVu()
        {
            var pg = new TabPage("Dịch vụ");
            gridDV = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            txtDVMa = TaoOTextBox("Mã DV", pnl);
            txtDVTen = TaoOTextBox("Tên DV", pnl);
            txtDVDvt = TaoOTextBox("Đơn vị tính", pnl);
            txtDVGia = TaoOTextBox("Đơn giá", pnl);

            var btnThem = new Button { Text = "Thêm", Width = 80 };
            btnThem.Click += (s, e) => { try { _svc.ThemDichVu(txtDVMa.Text, txtDVTen.Text, txtDVDvt.Text, decimal.Parse(txtDVGia.Text)); gridDV.DataSource = _svc.GetDichVu(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            var btnSua = new Button { Text = "Sửa", Width = 80 };
            btnSua.Click += (s, e) => { try { _svc.SuaDichVu(txtDVMa.Text, txtDVTen.Text, txtDVDvt.Text, decimal.Parse(txtDVGia.Text)); gridDV.DataSource = _svc.GetDichVu(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            var btnXoa = new Button { Text = "Xóa", Width = 80 };
            btnXoa.Click += (s, e) => { try { _svc.XoaDichVu(txtDVMa.Text); gridDV.DataSource = _svc.GetDichVu(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            pnl.Controls.Add(btnThem); pnl.Controls.Add(btnSua); pnl.Controls.Add(btnXoa);

            pg.Controls.Add(pnl); pg.Controls.Add(gridDV);
            return pg;
        }

        // ---------------- Quy dinh den bu ----------------
        private TabPage TaoTabQuyDinhDenBu()
        {
            var pg = new TabPage("Quy định đền bù");
            gridQD = new DataGridView { Dock = DockStyle.Top, Height = 350, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            var pnl = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            txtQDMa = TaoOTextBox("Mã quy định", pnl);
            txtQDLoai = TaoOTextBox("Mã loại tiện nghi", pnl);
            txtQDMucDo = TaoOTextBox("Mức độ thiệt hại", pnl);
            txtQDTien = TaoOTextBox("Mức đền bù", pnl);

            var btnThem = new Button { Text = "Thêm", Width = 80 };
            btnThem.Click += (s, e) => { try { _svc.ThemQuyDinhDenBu(txtQDMa.Text, txtQDLoai.Text, txtQDMucDo.Text, decimal.Parse(txtQDTien.Text)); gridQD.DataSource = _svc.GetQuyDinhDenBu(); } catch (Exception ex) { MessageBox.Show(ex.Message); } };
            pnl.Controls.Add(btnThem);

            pg.Controls.Add(pnl); pg.Controls.Add(gridQD);
            return pg;
        }

        private TextBox TaoOTextBox(string nhan, FlowLayoutPanel pnl)
        {
            pnl.Controls.Add(new Label { Text = nhan, AutoSize = true, Padding = new Padding(0, 8, 0, 0) });
            var tb = new TextBox { Width = 120 };
            pnl.Controls.Add(tb);
            return tb;
        }
    }
}
