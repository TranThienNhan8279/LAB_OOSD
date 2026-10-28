using System;
using System.Windows.Forms;
using CuaHangNuocGiaiKhat.Services;

namespace CuaHangNuocGiaiKhat.Forms
{
    /// <summary>Đặt hàng nhà cung ứng và lập phiếu giao hàng (tối đa 3 đợt).</summary>
    public class FrmMuaHang : Form
    {
        private readonly DanhMucService dm = new DanhMucService();
        private readonly MuaHangService sv = new MuaHangService();
        private ComboBox cboNcc, cboNv, cboNv2, cboDon; private DataGridView gDat, gDon, gConLai, gGiao;
        private DateTimePicker dtGiao;

        public FrmMuaHang()
        {
            Text = "Đặt hàng & nhận hàng từ nhà cung ứng"; Width = 1000; Height = 700; StartPosition = FormStartPosition.CenterParent;
            var nvTable = dm.DsNhanVien(); var ngk = dm.DsNuocGiaiKhat("");
            var tab = new TabControl { Dock = DockStyle.Fill }; Controls.Add(tab);
            var t1 = new TabPage("Lập đơn đặt hàng"); var t2 = new TabPage("Nhận hàng theo đợt");
            tab.TabPages.AddRange(new[] { t1, t2 });

            UiHelper.Lbl(t1, "Nhà cung ứng", 10, 12); cboNcc = UiHelper.Combo(t1, dm.DsNhaCungUng(), "TenNCC", "MaNCC", 100, 10, 250);
            UiHelper.Lbl(t1, "Nhân viên", 380, 12); cboNv = UiHelper.Combo(t1, nvTable, "HoTen", "MaNV", 450, 10, 180);
            gDat = UiHelper.LineGrid(t1, ngk, 10, 50, 950, 250, false);
            UiHelper.Btn(t1, "Lập đơn đặt hàng", 10, 310, 160, BtnDat);
            gDon = UiHelper.Grid(t1, 10, 360, 950, 250, true);

            UiHelper.Lbl(t2, "Đơn đặt hàng", 10, 12); cboDon = UiHelper.Combo(t2, sv.DsDonDatHang(), "MaDDH", "MaDDH", 100, 10, 120);
            gConLai = UiHelper.Grid(t2, 10, 40, 950, 160, true);
            UiHelper.Lbl(t2, "Nhân viên", 10, 212); cboNv2 = UiHelper.Combo(t2, nvTable, "HoTen", "MaNV", 80, 210, 180);
            UiHelper.Lbl(t2, "Ngày giao", 280, 212); dtGiao = new DateTimePicker { Left = 350, Top = 210, Width = 120, Format = DateTimePickerFormat.Short }; t2.Controls.Add(dtGiao);
            gGiao = UiHelper.LineGrid(t2, ngk, 10, 250, 950, 220, true);
            UiHelper.Btn(t2, "Xem phần còn phải giao", 10, 480, 190, BtnXemConLai);
            UiHelper.Btn(t2, "Lập phiếu giao hàng", 210, 480, 170, BtnGiao);
            Load += (s, e) => LoadDon();
            tab.SelectedIndexChanged += (s, e) => LoadDon();
        }

        private void LoadDon()
        {
            var dt = sv.DsDonDatHang();
            gDon.DataSource = dt;
            cboDon.DataSource = dt; cboDon.DisplayMember = "MaDDH"; cboDon.ValueMember = "MaDDH";
        }

        private string MaDonChon()
        {
            if (cboDon.SelectedValue == null) throw new Data.NghiepVuException("Chưa có đơn đặt hàng để chọn.");
            return (string)cboDon.SelectedValue;
        }

        private void BtnDat(object s, EventArgs e)
        {
            try
            {
                string ma = sv.TaoDonDatHang((string)cboNcc.SelectedValue, (string)cboNv.SelectedValue, UiHelper.DocDong(gDat, false));
                UiHelper.Ok("Đã lập đơn đặt hàng " + ma); gDat.Rows.Clear(); LoadDon();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        private void BtnXemConLai(object s, EventArgs e)
        {
            try { gConLai.DataSource = sv.ConPhaiGiao(MaDonChon()); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        private void BtnGiao(object s, EventArgs e)
        {
            try
            {
                string ma = sv.LapPhieuGiao(MaDonChon(), dtGiao.Value.Date, UiHelper.DocDong(gGiao, true));
                UiHelper.Ok("Đã lập phiếu giao " + ma); gGiao.Rows.Clear(); gConLai.DataSource = sv.ConPhaiGiao(MaDonChon());
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
