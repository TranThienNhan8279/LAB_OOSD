using System;
using System.Windows.Forms;
using CuaHangNuocGiaiKhat.Services;

namespace CuaHangNuocGiaiKhat.Forms
{
    /// <summary>Danh mục: nước giải khát (thêm, sửa giá, tìm), khách hàng, nhà cung ứng, nhân viên.</summary>
    public class FrmDanhMuc : Form
    {
        private readonly DanhMucService sv = new DanhMucService();
        private DataGridView gNgk, gKh, gNcc, gNv;
        private TextBox txtTim, txtMa, txtTen, txtDvt, txtGia, txtTon, txtMaKh, txtTenKh, txtDcKh, txtSdtKh;
        private ComboBox cboLoai; private CheckBox chkQuen;

        public FrmDanhMuc()
        {
            Text = "Danh mục"; Width = 1000; Height = 640; StartPosition = FormStartPosition.CenterParent;
            var tab = new TabControl { Dock = DockStyle.Fill }; Controls.Add(tab);
            var t1 = new TabPage("Nước giải khát"); var t2 = new TabPage("Khách hàng");
            var t3 = new TabPage("Nhà cung ứng"); var t4 = new TabPage("Nhân viên");
            tab.TabPages.AddRange(new[] { t1, t2, t3, t4 });

            UiHelper.Lbl(t1, "Tìm:", 10, 10); txtTim = new TextBox { Left = 50, Top = 10, Width = 200 }; t1.Controls.Add(txtTim);
            UiHelper.Btn(t1, "Tìm", 260, 8, 70, (s, e) => LoadNgk());
            gNgk = UiHelper.Grid(t1, 10, 45, 950, 330, true);
            UiHelper.Lbl(t1, "Mã", 10, 395); txtMa = new TextBox { Left = 45, Top = 395, Width = 70 }; t1.Controls.Add(txtMa);
            UiHelper.Lbl(t1, "Tên", 130, 395); txtTen = new TextBox { Left = 165, Top = 395, Width = 180 }; t1.Controls.Add(txtTen);
            UiHelper.Lbl(t1, "Loại", 360, 395); cboLoai = UiHelper.Combo(t1, sv.DsLoai(), "TenLoai", "MaLoai", 395, 395, 160);
            UiHelper.Lbl(t1, "ĐVT", 570, 395); txtDvt = new TextBox { Left = 605, Top = 395, Width = 60 }; t1.Controls.Add(txtDvt);
            UiHelper.Lbl(t1, "Giá bán", 680, 395); txtGia = new TextBox { Left = 735, Top = 395, Width = 80 }; t1.Controls.Add(txtGia);
            UiHelper.Lbl(t1, "Tồn", 830, 395); txtTon = new TextBox { Left = 865, Top = 395, Width = 60 }; t1.Controls.Add(txtTon);
            UiHelper.Btn(t1, "Thêm mới", 10, 440, 100, BtnThemNgk);
            UiHelper.Btn(t1, "Cập nhật giá dòng đang chọn", 120, 440, 210, BtnSuaGia);

            gKh = UiHelper.Grid(t2, 10, 10, 950, 330, true);
            UiHelper.Lbl(t2, "Mã", 10, 360); txtMaKh = new TextBox { Left = 45, Top = 360, Width = 70 }; t2.Controls.Add(txtMaKh);
            UiHelper.Lbl(t2, "Họ tên", 130, 360); txtTenKh = new TextBox { Left = 185, Top = 360, Width = 180 }; t2.Controls.Add(txtTenKh);
            UiHelper.Lbl(t2, "Địa chỉ", 380, 360); txtDcKh = new TextBox { Left = 435, Top = 360, Width = 200 }; t2.Controls.Add(txtDcKh);
            UiHelper.Lbl(t2, "SĐT", 650, 360); txtSdtKh = new TextBox { Left = 685, Top = 360, Width = 100 }; t2.Controls.Add(txtSdtKh);
            chkQuen = new CheckBox { Text = "Khách quen", Left = 800, Top = 360, Width = 110 }; t2.Controls.Add(chkQuen);
            UiHelper.Btn(t2, "Thêm khách hàng", 10, 400, 140, BtnThemKh);

            gNcc = UiHelper.Grid(t3, 10, 10, 950, 500, true);
            gNv = UiHelper.Grid(t4, 10, 10, 950, 500, true);
            Load += (s, e) => LoadAll();
        }

        private void LoadAll()
        {
            LoadNgk(); gKh.DataSource = sv.DsKhachHang(); gNcc.DataSource = sv.DsNhaCungUng(); gNv.DataSource = sv.DsNhanVien();
        }
        private void LoadNgk() { gNgk.DataSource = sv.DsNuocGiaiKhat(txtTim.Text); }

        private void BtnThemNgk(object s, EventArgs e)
        {
            try
            {
                decimal gia; int ton;
                if (!decimal.TryParse(txtGia.Text, out gia) || !int.TryParse(txtTon.Text, out ton))
                    throw new Data.NghiepVuException("Giá bán và tồn kho phải là số.");
                sv.ThemNuocGiaiKhat(txtMa.Text.Trim(), (string)cboLoai.SelectedValue, txtTen.Text.Trim(), txtDvt.Text.Trim(), gia, ton);
                UiHelper.Ok("Đã thêm nước giải khát."); LoadNgk();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        private void BtnSuaGia(object s, EventArgs e)
        {
            try
            {
                if (gNgk.CurrentRow == null) throw new Data.NghiepVuException("Chọn 1 dòng để cập nhật giá.");
                decimal gia;
                if (!decimal.TryParse(txtGia.Text, out gia)) throw new Data.NghiepVuException("Nhập giá bán mới vào ô Giá bán.");
                sv.CapNhatGia((string)gNgk.CurrentRow.Cells["MaNGK"].Value, gia);
                UiHelper.Ok("Đã cập nhật giá."); LoadNgk();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        private void BtnThemKh(object s, EventArgs e)
        {
            try
            {
                sv.ThemKhachHang(txtMaKh.Text.Trim(), txtTenKh.Text.Trim(), txtDcKh.Text.Trim(), txtSdtKh.Text.Trim(), chkQuen.Checked);
                UiHelper.Ok("Đã thêm khách hàng."); gKh.DataSource = sv.DsKhachHang();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
