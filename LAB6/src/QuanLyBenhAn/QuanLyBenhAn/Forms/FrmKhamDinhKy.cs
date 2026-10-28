using System;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    // Hẹn lịch và khám sức khỏe định kỳ theo quý
    public class FrmKhamDinhKy : Form
    {
        DanhMucService dm = new DanhMucService();
        XuatVienService sv = new XuatVienService();
        TextBox txtHoSo, txtQuy, txtNhiet, txtHa, txtNx;
        DateTimePicker dtHen;
        ComboBox cboBS, cboKhoa; DataGridView g;

        public FrmKhamDinhKy()
        {
            Text = "Khám định kỳ"; Width = 900; Height = 560; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Mã hồ sơ", 10, 12); txtHoSo = UiHelper.Txt(this, 70, 10, 70);
            UiHelper.Lbl(this, "Quý", 155, 12); txtQuy = UiHelper.Txt(this, 185, 10, 30);
            UiHelper.Lbl(this, "Ngày hẹn", 230, 12); dtHen = UiHelper.Date(this, 295, 10, 30);
            dtHen.Format = DateTimePickerFormat.Custom; dtHen.CustomFormat = "dd/MM/yyyy HH:mm"; dtHen.Width = 130;
            UiHelper.Btn(this, "Đặt lịch", 440, 8, 80, BtnDat);
            UiHelper.Btn(this, "Xem lịch", 530, 8, 80, (s, e) => g.DataSource = sv.DsLich(txtHoSo.Text.Trim()));

            g = UiHelper.Grid(this, 10, 50, 860, 200);

            UiHelper.Lbl(this, "Khám theo lịch đang chọn", 10, 268);
            UiHelper.Lbl(this, "Khoa", 10, 302); cboKhoa = UiHelper.Combo(this, dm.DsKhoa(), "TenKhoa", "MaKhoa", 45, 300, 120);
            UiHelper.Lbl(this, "Bác sĩ", 175, 302); cboBS = UiHelper.Combo(this, dm.DsBacSi(), "HoTen", "MaBS", 220, 300, 150);
            UiHelper.Lbl(this, "Thân nhiệt", 380, 302); txtNhiet = UiHelper.Txt(this, 445, 300, 50);
            UiHelper.Lbl(this, "Huyết áp", 505, 302); txtHa = UiHelper.Txt(this, 560, 300, 70);
            UiHelper.Lbl(this, "Nhận xét", 10, 342); txtNx = UiHelper.Txt(this, 70, 340, 500);
            UiHelper.Btn(this, "Ghi nhận khám", 590, 338, 130, BtnKham);
        }

        void BtnDat(object s, EventArgs e)
        {
            try
            {
                string ma = sv.DatLich(txtHoSo.Text.Trim(), UiHelper.So(txtQuy, "Quý"), dtHen.Value);
                UiHelper.Ok("Đã đặt lịch " + ma);
                g.DataSource = sv.DsLich(txtHoSo.Text.Trim());
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnKham(object s, EventArgs e)
        {
            try
            {
                if (g.CurrentRow == null) throw new Exception("Chọn một lịch hẹn.");
                decimal nhiet;
                if (!decimal.TryParse(txtNhiet.Text, out nhiet)) throw new Exception("Thân nhiệt phải là số.");
                string ma = sv.KhamDinhKy(g.CurrentRow.Cells["MaLich"].Value.ToString(), UiHelper.Chon(cboBS), UiHelper.Chon(cboKhoa), txtNx.Text.Trim(), nhiet, txtHa.Text.Trim());
                UiHelper.Ok("Đã ghi nhận phiếu khám " + ma);
                g.DataSource = sv.DsLich(txtHoSo.Text.Trim());
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
