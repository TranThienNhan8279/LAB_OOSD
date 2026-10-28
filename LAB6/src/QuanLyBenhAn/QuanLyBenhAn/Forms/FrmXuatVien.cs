using System;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    // Thanh toán viện phí, xuất viện và ghi nhận vào hồ sơ
    public class FrmXuatVien : Form
    {
        DanhMucService dm = new DanhMucService();
        KhamBenhService kb = new KhamBenhService();
        XuatVienService sv = new XuatVienService();
        ComboBox cboPhieu, cboGhi, cboNV; TextBox txtTien; DataGridView g;

        public FrmXuatVien()
        {
            Text = "Thanh toán và xuất viện"; Width = 900; Height = 520; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Phiếu khám", 10, 12); cboPhieu = UiHelper.Combo(this, kb.DsPhieuDangDieuTri(), "MaPhieuKham", "MaPhieuKham", 85, 10, 90);
            UiHelper.Lbl(this, "Viện phí", 195, 12); txtTien = UiHelper.Txt(this, 250, 10, 100);
            UiHelper.Btn(this, "Thanh toán, xuất viện", 365, 8, 170, BtnThanhToan);

            UiHelper.Lbl(this, "Ghi nhận phiếu", 10, 52); cboGhi = UiHelper.Combo(this, sv.DsChoGhiNhan(), "MaPhieuKham", "MaPhieuKham", 105, 50, 90);
            UiHelper.Lbl(this, "Nhân viên", 215, 52); cboNV = UiHelper.Combo(this, dm.DsNhanVien(), "HoTen", "MaNV", 280, 50, 160);
            UiHelper.Btn(this, "Ghi nhận vào hồ sơ", 455, 48, 150, BtnGhiNhan);

            UiHelper.Lbl(this, "Phiếu đã xuất viện, chờ ghi nhận", 10, 95);
            g = UiHelper.Grid(this, 10, 125, 860, 340);
            g.DataSource = sv.DsChoGhiNhan();
        }

        void NapLai()
        {
            cboPhieu.DataSource = kb.DsPhieuDangDieuTri();
            cboGhi.DataSource = sv.DsChoGhiNhan();
            g.DataSource = sv.DsChoGhiNhan();
        }

        void BtnThanhToan(object s, EventArgs e)
        {
            try
            {
                string ma = sv.ThanhToanXuatVien(UiHelper.Chon(cboPhieu), UiHelper.Tien(txtTien, "Viện phí"));
                UiHelper.Ok("Đã thanh toán, mã " + ma);
                NapLai();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnGhiNhan(object s, EventArgs e)
        {
            try { sv.GhiNhanVaoHoSo(UiHelper.Chon(cboGhi), UiHelper.Chon(cboNV)); UiHelper.Ok("Đã ghi nhận."); NapLai(); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
