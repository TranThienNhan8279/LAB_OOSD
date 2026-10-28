using System;
using System.Windows.Forms;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.Forms
{
    // Tuyển dụng (thử việc) và ký hợp đồng chính thức
    public class FrmNhanVien : Form
    {
        DanhMucService dm = new DanhMucService();
        NhanSuService sv = new NhanSuService();
        TextBox txtTen, txtDiaChi, txtSdt, txtLuongTV, txtDieuKhoan, txtLuongHD;
        DateTimePicker dtSinh, dtBD, dtKT, dtHDBD, dtHDKT;
        ComboBox cboPB, cboNV, cboLoai; DataGridView gNV, gHD;

        public FrmNhanVien()
        {
            Text = "Nhân viên và hợp đồng"; Width = 1000; Height = 700; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Họ tên", 10, 12); txtTen = UiHelper.Txt(this, 60, 10, 160);
            UiHelper.Lbl(this, "Ngày sinh", 235, 12); dtSinh = UiHelper.Date(this, 300, 10, -9000);
            UiHelper.Lbl(this, "Điện thoại", 425, 12); txtSdt = UiHelper.Txt(this, 495, 10, 100);
            UiHelper.Lbl(this, "Phòng ban", 610, 12); cboPB = UiHelper.Combo(this, dm.DsPhongBan(), "TenPB", "MaPB", 680, 10, 140);
            UiHelper.Lbl(this, "Địa chỉ", 10, 48); txtDiaChi = UiHelper.Txt(this, 60, 46, 300);
            UiHelper.Lbl(this, "Thử việc từ", 375, 48); dtBD = UiHelper.Date(this, 450, 46, 0);
            UiHelper.Lbl(this, "đến", 570, 48); dtKT = UiHelper.Date(this, 600, 46, 30);
            UiHelper.Lbl(this, "Lương thử việc", 725, 48); txtLuongTV = UiHelper.Txt(this, 815, 46, 90);
            UiHelper.Lbl(this, "Điều khoản", 10, 84); txtDieuKhoan = UiHelper.Txt(this, 80, 82, 500);
            UiHelper.Btn(this, "Tuyển dụng", 600, 80, 120, BtnTuyenDung);
            gNV = UiHelper.Grid(this, 10, 120, 960, 200);

            UiHelper.Lbl(this, "Nhân viên", 10, 342); cboNV = UiHelper.Combo(this, dm.DsNhanVienDangLam(), "HoTen", "MaNV", 75, 340, 150);
            UiHelper.Lbl(this, "Loại HĐ", 240, 342);
            cboLoai = new ComboBox { Left = 295, Top = 340, Width = 90, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLoai.Items.AddRange(new object[] { "Ngắn hạn", "Dài hạn" }); cboLoai.SelectedIndex = 0; Controls.Add(cboLoai);
            UiHelper.Lbl(this, "Từ ngày", 400, 342); dtHDBD = UiHelper.Date(this, 455, 340, 1);
            UiHelper.Lbl(this, "Đến ngày", 580, 342); dtHDKT = UiHelper.Date(this, 640, 340, 365);
            UiHelper.Lbl(this, "Lương", 765, 342); txtLuongHD = UiHelper.Txt(this, 810, 340, 90);
            UiHelper.Btn(this, "Ký hợp đồng chính thức", 10, 376, 180, BtnKy);
            gHD = UiHelper.Grid(this, 10, 415, 960, 230);
            Load += (s, e) => NapLai();
        }

        void NapLai() { gNV.DataSource = dm.DsNhanVien(); gHD.DataSource = dm.DsHopDong(); cboNV.DataSource = dm.DsNhanVienDangLam(); }

        void BtnTuyenDung(object s, EventArgs e)
        {
            try
            {
                string ma = sv.TuyenDung(txtTen.Text.Trim(), dtSinh.Value.Date, txtDiaChi.Text.Trim(), txtSdt.Text.Trim(), UiHelper.Chon(cboPB),
                    dtBD.Value.Date, dtKT.Value.Date, UiHelper.Tien(txtLuongTV, "Lương thử việc"), txtDieuKhoan.Text.Trim());
                UiHelper.Ok("Đã tuyển dụng, mã nhân viên " + ma);
                NapLai();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnKy(object s, EventArgs e)
        {
            try
            {
                string loai = cboLoai.SelectedItem.ToString();
                string ma = sv.KyHopDong(UiHelper.Chon(cboNV), loai, dtHDBD.Value.Date, loai == "Dài hạn" ? (DateTime?)null : dtHDKT.Value.Date,
                    UiHelper.Tien(txtLuongHD, "Lương hợp đồng"));
                UiHelper.Ok("Đã ký hợp đồng " + ma);
                NapLai();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
