using System;
using System.Windows.Forms;
using CuaHangNuocGiaiKhat.Services;

namespace CuaHangNuocGiaiKhat.Forms
{
    /// <summary>Bán hàng theo tồn kho. Thiếu hàng thì lập phiếu hẹn giao.</summary>
    public class FrmBanHang : Form
    {
        private readonly DanhMucService dm = new DanhMucService();
        private readonly BanHangService sv = new BanHangService();
        private ComboBox cboKh, cboNv; private DataGridView gLine, gHd;
        private DateTimePicker dtHen; private CheckBox chkHen;

        public FrmBanHang()
        {
            Text = "Bán hàng"; Width = 1000; Height = 680; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Khách hàng", 10, 12); cboKh = UiHelper.Combo(this, dm.DsKhachHang(), "HoTen", "MaKH", 90, 10, 220);
            UiHelper.Lbl(this, "Nhân viên", 330, 12); cboNv = UiHelper.Combo(this, dm.DsNhanVien(), "HoTen", "MaNV", 400, 10, 180);
            gLine = UiHelper.LineGrid(this, dm.DsNuocGiaiKhat(""), 10, 50, 960, 240, false);
            chkHen = new CheckBox { Text = "Ngày hẹn giao (nếu thiếu hàng):", Left = 10, Top = 300, Width = 220, Checked = true }; Controls.Add(chkHen);
            dtHen = new DateTimePicker { Left = 235, Top = 298, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(3) }; Controls.Add(dtHen);
            UiHelper.Btn(this, "Lập hóa đơn", 380, 296, 120, BtnLap);
            gHd = UiHelper.Grid(this, 10, 340, 960, 290, true);
            Load += (s, e) => gHd.DataSource = sv.DsHoaDon();
        }

        private void BtnLap(object s, EventArgs e)
        {
            try
            {
                var kq = sv.LapHoaDon((string)cboKh.SelectedValue, (string)cboNv.SelectedValue,
                    UiHelper.DocDong(gLine, false), chkHen.Checked ? (DateTime?)dtHen.Value.Date : null);
                string msg = "Đã lập hóa đơn " + kq.MaHD + ", tổng tiền " + kq.TongTien.ToString("N0") + " đ.";
                if (kq.ConThieu) msg += "\nKhông đủ hàng: đã lập phiếu hẹn giao " + kq.MaPhieuHen + ".";
                UiHelper.Ok(msg); gLine.Rows.Clear(); gHd.DataSource = sv.DsHoaDon();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
