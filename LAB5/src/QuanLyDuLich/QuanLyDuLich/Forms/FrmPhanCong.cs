using System;
using System.Windows.Forms;
using QuanLyDuLich.Services;

namespace QuanLyDuLich.Forms
{
    public class FrmPhanCong : Form
    {
        DanhMucService dm = new DanhMucService();
        PhanCongService sv = new PhanCongService();
        DangKyService dk = new DangKyService();
        ComboBox cboChuyen, cboNv1, cboDoan, cboNv2, cboXem; TextBox txtVaiTro; DataGridView g;

        public FrmPhanCong()
        {
            Text = "Phân công hướng dẫn viên"; Width = 900; Height = 560; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Chuyến lẻ", 10, 12); cboChuyen = UiHelper.Combo(this, dm.DsChuyen(), "MaChuyen", "MaChuyen", 75, 10, 90);
            UiHelper.Lbl(this, "Hướng dẫn viên", 180, 12); cboNv1 = UiHelper.Combo(this, dm.DsNhanVien(), "HoTen", "MaNV", 280, 10, 160);
            UiHelper.Btn(this, "Phân công chuyến", 460, 8, 140, BtnChuyen);

            UiHelper.Lbl(this, "Đoàn", 10, 52); cboDoan = UiHelper.Combo(this, dk.DsPhieuDoan(), "MaPhieuDoan", "MaPhieuDoan", 75, 50, 90);
            UiHelper.Lbl(this, "Hướng dẫn viên", 180, 52); cboNv2 = UiHelper.Combo(this, dm.DsNhanVien(), "HoTen", "MaNV", 280, 50, 160);
            UiHelper.Lbl(this, "Vai trò", 460, 52); txtVaiTro = UiHelper.Txt(this, 510, 50, 120);
            UiHelper.Btn(this, "Phân công đoàn", 650, 48, 130, BtnDoan);

            UiHelper.Lbl(this, "Xem lịch của", 10, 100); cboXem = UiHelper.Combo(this, dm.DsNhanVien(), "HoTen", "MaNV", 95, 98, 160);
            UiHelper.Btn(this, "Xem", 270, 96, 70, (s, e) => g.DataSource = sv.LichPhanCong(UiHelper.Chon(cboXem)));
            g = UiHelper.Grid(this, 10, 140, 860, 370);
        }

        void BtnChuyen(object s, EventArgs e)
        {
            try { sv.PhanCongChuyen(UiHelper.Chon(cboChuyen), UiHelper.Chon(cboNv1)); UiHelper.Ok("Đã phân công."); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnDoan(object s, EventArgs e)
        {
            try { string ma = sv.PhanCongDoan(UiHelper.Chon(cboDoan), UiHelper.Chon(cboNv2), txtVaiTro.Text.Trim()); UiHelper.Ok("Đã phân công " + ma); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
