using System;
using System.Windows.Forms;
using CuaHangNuocGiaiKhat.Services;

namespace CuaHangNuocGiaiKhat.Forms
{
    /// <summary>Lập phiếu ghi nợ cho khách quen.</summary>
    public class FrmGhiNo : Form
    {
        private readonly DanhMucService dm = new DanhMucService();
        private readonly BanHangService bh = new BanHangService();
        private readonly GhiNoService sv = new GhiNoService();
        private ComboBox cboHd, cboNv; private TextBox txtTien, txtLyDo; private DateTimePicker dtTra; private DataGridView g;

        public FrmGhiNo()
        {
            Text = "Lập phiếu ghi nợ"; Width = 900; Height = 560; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Hóa đơn", 10, 12); cboHd = UiHelper.Combo(this, bh.DsHoaDon(), "MaHD", "MaHD", 70, 10, 100);
            UiHelper.Lbl(this, "Nhân viên", 190, 12); cboNv = UiHelper.Combo(this, dm.DsNhanVien(), "HoTen", "MaNV", 260, 10, 180);
            UiHelper.Lbl(this, "Số tiền nợ", 10, 52); txtTien = new TextBox { Left = 80, Top = 50, Width = 120 }; Controls.Add(txtTien);
            UiHelper.Lbl(this, "Dự kiến trả", 220, 52); dtTra = new DateTimePicker { Left = 295, Top = 50, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(30) }; Controls.Add(dtTra);
            UiHelper.Lbl(this, "Lý do", 10, 92); txtLyDo = new TextBox { Left = 80, Top = 90, Width = 335 }; Controls.Add(txtLyDo);
            UiHelper.Btn(this, "Lập phiếu ghi nợ", 430, 88, 150, BtnLap);
            g = UiHelper.Grid(this, 10, 140, 860, 360, true);
            Load += (s, e) => g.DataSource = sv.DsPhieuGhiNo();
        }

        private void BtnLap(object s, EventArgs e)
        {
            try
            {
                decimal tien;
                if (!decimal.TryParse(txtTien.Text, out tien)) throw new Data.NghiepVuException("Số tiền nợ phải là số.");
                string ma = sv.LapPhieuGhiNo((string)cboHd.SelectedValue, (string)cboNv.SelectedValue, tien, dtTra.Value.Date, txtLyDo.Text.Trim());
                UiHelper.Ok("Đã lập phiếu ghi nợ " + ma); g.DataSource = sv.DsPhieuGhiNo();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
