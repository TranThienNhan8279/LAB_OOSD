using System;
using System.Windows.Forms;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.Forms
{
    // Khen thưởng và kỷ luật
    public class FrmThuongPhat : Form
    {
        DanhMucService dm = new DanhMucService();
        ThuongPhatService sv = new ThuongPhatService();
        ComboBox cboLoai, cboNV, cboPB, cboNV2, cboHinhThuc;
        TextBox txtND, txtTien, txtLyDo, txtPhat;
        DataGridView gKT, gKL;

        public FrmThuongPhat()
        {
            Text = "Khen thưởng và kỷ luật"; Width = 1000; Height = 700; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Khen thưởng", 10, 12);
            cboLoai = new ComboBox { Left = 90, Top = 10, Width = 80, DropDownStyle = ComboBoxStyle.DropDownList };
            cboLoai.Items.AddRange(new object[] { "Cá nhân", "Tập thể" }); cboLoai.SelectedIndex = 0; Controls.Add(cboLoai);
            UiHelper.Lbl(this, "Nhân viên", 185, 12); cboNV = UiHelper.Combo(this, dm.DsNhanVienDangLam(), "HoTen", "MaNV", 250, 10, 140);
            UiHelper.Lbl(this, "Phòng ban", 400, 12); cboPB = UiHelper.Combo(this, dm.DsPhongBan(), "TenPB", "MaPB", 470, 10, 130);
            UiHelper.Lbl(this, "Nội dung", 10, 52); txtND = UiHelper.Txt(this, 70, 50, 300);
            UiHelper.Lbl(this, "Số tiền", 385, 52); txtTien = UiHelper.Txt(this, 435, 50, 100);
            UiHelper.Btn(this, "Lập quyết định, phiếu chi", 560, 48, 200, BtnThuong);
            gKT = UiHelper.Grid(this, 10, 90, 960, 190);

            UiHelper.Lbl(this, "Kỷ luật", 10, 302); cboNV2 = UiHelper.Combo(this, dm.DsNhanVienDangLam(), "HoTen", "MaNV", 65, 300, 140);
            UiHelper.Lbl(this, "Hình thức", 215, 302);
            cboHinhThuc = new ComboBox { Left = 280, Top = 300, Width = 100, DropDownStyle = ComboBoxStyle.DropDownList };
            cboHinhThuc.Items.AddRange(new object[] { "Khiển trách", "Cảnh cáo", "Phạt tiền" }); cboHinhThuc.SelectedIndex = 0; Controls.Add(cboHinhThuc);
            UiHelper.Lbl(this, "Tiền phạt", 395, 302); txtPhat = UiHelper.Txt(this, 455, 300, 90);
            UiHelper.Lbl(this, "Lý do", 10, 342); txtLyDo = UiHelper.Txt(this, 50, 340, 320);
            UiHelper.Btn(this, "Lập quyết định kỷ luật", 390, 338, 180, BtnKyLuat);
            gKL = UiHelper.Grid(this, 10, 380, 960, 270);
            Load += (s, e) => NapLai();
        }

        void NapLai() { gKT.DataSource = sv.DsKhenThuong(); gKL.DataSource = sv.DsKyLuat(); }

        void BtnThuong(object s, EventArgs e)
        {
            try
            {
                string loai = cboLoai.SelectedItem.ToString();
                string ma = sv.LapKhenThuong(loai, loai == "Cá nhân" ? UiHelper.Chon(cboNV) : UiHelper.Chon(cboPB), txtND.Text.Trim(), UiHelper.Tien(txtTien, "Số tiền"));
                UiHelper.Ok("Đã lập " + ma); NapLai();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnKyLuat(object s, EventArgs e)
        {
            try
            {
                decimal phat = 0;
                if (cboHinhThuc.SelectedItem.ToString() == "Phạt tiền") phat = UiHelper.Tien(txtPhat, "Tiền phạt");
                UiHelper.Ok("Đã lập " + sv.LapKyLuat(UiHelper.Chon(cboNV2), cboHinhThuc.SelectedItem.ToString(), txtLyDo.Text.Trim(), phat)); NapLai();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
