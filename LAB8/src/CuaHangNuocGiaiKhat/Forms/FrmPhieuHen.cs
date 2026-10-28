using System;
using System.Windows.Forms;
using CuaHangNuocGiaiKhat.Services;

namespace CuaHangNuocGiaiKhat.Forms
{
    /// <summary>Giao hàng theo phiếu hẹn (có thể nhiều đợt).</summary>
    public class FrmPhieuHen : Form
    {
        private readonly BanHangService sv = new BanHangService();
        private DataGridView gPhieu, gCt; private DateTimePicker dtMoi;

        public FrmPhieuHen()
        {
            Text = "Phiếu hẹn giao"; Width = 900; Height = 620; StartPosition = FormStartPosition.CenterParent;
            gPhieu = UiHelper.Grid(this, 10, 10, 860, 220, true);
            gCt = UiHelper.Grid(this, 10, 250, 860, 200, true);
            UiHelper.Lbl(this, "Nếu vẫn thiếu, hẹn lại ngày:", 10, 468);
            dtMoi = new DateTimePicker { Left = 190, Top = 466, Width = 120, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(3) }; Controls.Add(dtMoi);
            UiHelper.Btn(this, "Giao hàng theo phiếu đang chọn", 330, 462, 230, BtnGiao);
            UiHelper.Btn(this, "Tải lại", 570, 462, 80, (s, e) => LoadPhieu());
            gPhieu.SelectionChanged += (s, e) => LoadCt();
            Load += (s, e) => LoadPhieu();
        }

        private string MaChon() { return gPhieu.CurrentRow == null ? null : (string)gPhieu.CurrentRow.Cells["MaPhieuHen"].Value; }
        private void LoadPhieu() { gPhieu.DataSource = sv.DsPhieuHenChoGiao(); LoadCt(); }
        private void LoadCt() { string m = MaChon(); gCt.DataSource = m == null ? null : sv.ChiTietPhieuHen(m); }

        private void BtnGiao(object s, EventArgs e)
        {
            try
            {
                string ma = MaChon();
                if (ma == null) throw new Data.NghiepVuException("Chọn 1 phiếu hẹn.");
                string moi = sv.GiaoHangHen(ma, dtMoi.Value.Date);
                UiHelper.Ok(moi == null ? "Đã giao đủ, hóa đơn hoàn tất." : "Đã giao một phần. Còn thiếu: lập phiếu hẹn mới " + moi + ".");
                LoadPhieu();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
