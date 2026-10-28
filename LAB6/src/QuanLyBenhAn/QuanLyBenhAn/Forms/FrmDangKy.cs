using System;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    // Đăng ký lần đầu, đóng lệ phí năm, tra cứu hồ sơ
    public class FrmDangKy : Form
    {
        HoSoService sv = new HoSoService();
        TextBox txtTen, txtDiaChi, txtSdt, txtAnh, txtTim, txtBN;
        DateTimePicker dtSinh;
        DataGridView gHoSo, gSu;

        public FrmDangKy()
        {
            Text = "Đăng ký và tra cứu hồ sơ"; Width = 1000; Height = 680; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Họ tên", 10, 12); txtTen = UiHelper.Txt(this, 60, 10, 180);
            UiHelper.Lbl(this, "Ngày sinh", 255, 12); dtSinh = UiHelper.Date(this, 320, 10, -9000);
            UiHelper.Lbl(this, "Điện thoại", 445, 12); txtSdt = UiHelper.Txt(this, 515, 10, 100);
            UiHelper.Lbl(this, "Địa chỉ", 10, 48); txtDiaChi = UiHelper.Txt(this, 60, 46, 300);
            UiHelper.Lbl(this, "Ảnh 3x4 (tên file)", 375, 48); txtAnh = UiHelper.Txt(this, 490, 46, 125);
            UiHelper.Btn(this, "Đăng ký, cấp hồ sơ", 640, 8, 150, BtnDangKy);
            UiHelper.Lbl(this, "Mã BN", 640, 52); txtBN = UiHelper.Txt(this, 690, 46, 70);
            UiHelper.Btn(this, "Đóng lệ phí năm nay", 770, 44, 150, BtnLePhi);

            UiHelper.Lbl(this, "Tra cứu (mã hồ sơ, tên, điện thoại)", 10, 92); txtTim = UiHelper.Txt(this, 240, 90, 200);
            UiHelper.Btn(this, "Tìm", 450, 88, 70, (s, e) => gHoSo.DataSource = sv.TraCuu(txtTim.Text));
            gHoSo = UiHelper.Grid(this, 10, 128, 960, 200);
            gHoSo.SelectionChanged += (s, e) =>
            {
                if (gHoSo.CurrentRow != null && gHoSo.CurrentRow.Cells["MaHoSo"].Value != null)
                    gSu.DataSource = sv.LichSu(gHoSo.CurrentRow.Cells["MaHoSo"].Value.ToString());
            };
            UiHelper.Lbl(this, "Lịch sử khám của hồ sơ đang chọn", 10, 340);
            gSu = UiHelper.Grid(this, 10, 370, 960, 260);
            Load += (s, e) => gHoSo.DataSource = sv.TraCuu("");
        }

        void BtnDangKy(object s, EventArgs e)
        {
            try
            {
                string hs = sv.DangKyLanDau(txtTen.Text.Trim(), txtDiaChi.Text.Trim(), dtSinh.Value.Date, txtSdt.Text.Trim(), txtAnh.Text.Trim());
                UiHelper.Ok("Đã cấp hồ sơ " + hs);
                gHoSo.DataSource = sv.TraCuu("");
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnLePhi(object s, EventArgs e)
        {
            try { sv.DongLePhi(txtBN.Text.Trim()); UiHelper.Ok("Đã thu lệ phí năm " + DateTime.Today.Year); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
