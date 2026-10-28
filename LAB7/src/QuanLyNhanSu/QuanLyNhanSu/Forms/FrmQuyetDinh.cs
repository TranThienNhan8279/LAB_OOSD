using System;
using System.Windows.Forms;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.Forms
{
    // Quyết định chức vụ, quyết định lương, đăng ký bảo hiểm
    public class FrmQuyetDinh : Form
    {
        DanhMucService dm = new DanhMucService();
        NhanSuService sv = new NhanSuService();
        ComboBox cboNV1, cboCV, cboNV2, cboHD; DateTimePicker dtCV, dtLuong;
        TextBox txtLuong, txtSoSo;

        public FrmQuyetDinh()
        {
            Text = "Quyết định chức vụ, lương, bảo hiểm"; Width = 820; Height = 300; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Chức vụ", 10, 15); cboNV1 = UiHelper.Combo(this, dm.DsNhanVienDangLam(), "HoTen", "MaNV", 75, 12, 150);
            cboCV = UiHelper.Combo(this, dm.DsChucVu(), "TenCV", "MaCV", 235, 12, 140);
            UiHelper.Lbl(this, "Hiệu lực", 390, 15); dtCV = UiHelper.Date(this, 450, 12, 0);
            UiHelper.Btn(this, "Lập quyết định chức vụ", 580, 10, 190, BtnChucVu);

            UiHelper.Lbl(this, "Lương", 10, 65); cboNV2 = UiHelper.Combo(this, dm.DsNhanVienDangLam(), "HoTen", "MaNV", 75, 62, 150);
            UiHelper.Lbl(this, "Mức mới", 235, 65); txtLuong = UiHelper.Txt(this, 295, 62, 90);
            UiHelper.Lbl(this, "Hiệu lực", 390, 65); dtLuong = UiHelper.Date(this, 450, 62, 0);
            UiHelper.Btn(this, "Lập quyết định lương", 580, 60, 190, BtnLuong);

            UiHelper.Lbl(this, "Bảo hiểm", 10, 115); cboHD = UiHelper.Combo(this, dm.DsHopDongDaiHan(), "Ten", "MaHD", 75, 112, 200);
            UiHelper.Lbl(this, "Số sổ", 290, 115); txtSoSo = UiHelper.Txt(this, 335, 112, 120);
            UiHelper.Btn(this, "Đăng ký bảo hiểm", 580, 110, 190, BtnBaoHiem);
        }

        void BtnChucVu(object s, EventArgs e)
        {
            try { UiHelper.Ok("Đã lập " + sv.QuyetDinhChucVu(UiHelper.Chon(cboNV1), UiHelper.Chon(cboCV), dtCV.Value.Date)); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnLuong(object s, EventArgs e)
        {
            try { UiHelper.Ok("Đã lập " + sv.QuyetDinhLuong(UiHelper.Chon(cboNV2), UiHelper.Tien(txtLuong, "Mức lương"), dtLuong.Value.Date)); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnBaoHiem(object s, EventArgs e)
        {
            try { UiHelper.Ok("Đã đăng ký " + sv.DangKyBaoHiem(UiHelper.Chon(cboHD), txtSoSo.Text.Trim())); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
