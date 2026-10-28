using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.Forms
{
    // Kế hoạch phân công tuần, chấm công, phiếu phụ trội
    public class FrmChamCong : Form
    {
        DanhMucService dm = new DanhMucService();
        ChamCongService sv = new ChamCongService();
        ComboBox cboPB, cboKH, cboNV, cboCa, cboNV2, cboPB2, cboNV3;
        DateTimePicker dtTuan, dtNgay, dtCC, dtPT;
        TextBox txtCV, txtGio, txtGhiChu, txtGioPT;
        DataGridView gPC, gPT;
        List<DongPhuTroi> pt = new List<DongPhuTroi>();

        public FrmChamCong()
        {
            Text = "Phân công, chấm công, phụ trội"; Width = 1000; Height = 720; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Phòng ban", 10, 12); cboPB = UiHelper.Combo(this, dm.DsPhongBan(), "TenPB", "MaPB", 80, 10, 130);
            UiHelper.Lbl(this, "Tuần bắt đầu (thứ Hai)", 225, 12); dtTuan = UiHelper.Date(this, 365, 10, 0);
            UiHelper.Lbl(this, "Ghi chú", 490, 12); txtGhiChu = UiHelper.Txt(this, 545, 10, 200);
            UiHelper.Btn(this, "Lập kế hoạch tuần", 760, 8, 150, BtnKeHoach);

            UiHelper.Lbl(this, "Kế hoạch", 10, 52); cboKH = UiHelper.Combo(this, sv.DsKeHoach(), "MaKHT", "MaKHT", 75, 50, 80);
            cboKH.SelectedIndexChanged += (s, e) => NapPhanCong();
            UiHelper.Lbl(this, "Nhân viên", 170, 52); cboNV = UiHelper.Combo(this, dm.DsNhanVienDangLam(), "HoTen", "MaNV", 235, 50, 140);
            UiHelper.Lbl(this, "Ngày", 385, 52); dtNgay = UiHelper.Date(this, 420, 50, 0);
            UiHelper.Lbl(this, "Ca", 545, 52);
            cboCa = new ComboBox { Left = 570, Top = 50, Width = 70, DropDownStyle = ComboBoxStyle.DropDownList };
            cboCa.Items.AddRange(new object[] { "Sáng", "Chiều", "Tối" }); cboCa.SelectedIndex = 0; Controls.Add(cboCa);
            UiHelper.Lbl(this, "Công việc", 650, 52); txtCV = UiHelper.Txt(this, 715, 50, 150);
            UiHelper.Btn(this, "Phân công", 875, 48, 90, BtnPhanCong);
            gPC = UiHelper.Grid(this, 10, 85, 960, 150);

            UiHelper.Lbl(this, "Chấm công", 10, 262); cboNV2 = UiHelper.Combo(this, dm.DsNhanVienDangLam(), "HoTen", "MaNV", 80, 260, 150);
            UiHelper.Lbl(this, "Ngày", 245, 262); dtCC = UiHelper.Date(this, 280, 260, 0);
            UiHelper.Lbl(this, "Số giờ", 405, 262); txtGio = UiHelper.Txt(this, 445, 260, 50);
            UiHelper.Btn(this, "Chấm công", 510, 258, 100, BtnChamCong);

            UiHelper.Lbl(this, "Phụ trội", 10, 312); cboPB2 = UiHelper.Combo(this, dm.DsPhongBan(), "TenPB", "MaPB", 65, 310, 130);
            UiHelper.Lbl(this, "Ngày", 205, 312); dtPT = UiHelper.Date(this, 240, 310, 0);
            UiHelper.Lbl(this, "Nhân viên", 365, 312); cboNV3 = UiHelper.Combo(this, dm.DsNhanVienDangLam(), "HoTen", "MaNV", 430, 310, 140);
            UiHelper.Lbl(this, "Số giờ", 580, 312); txtGioPT = UiHelper.Txt(this, 620, 310, 40);
            UiHelper.Btn(this, "Thêm dòng", 675, 308, 90, BtnThemDong);
            UiHelper.Btn(this, "Lưu phiếu phụ trội", 775, 308, 150, BtnLuuPT);
            gPT = UiHelper.Grid(this, 10, 350, 960, 300);
        }

        void NapPhanCong() { if (cboKH.SelectedValue != null) gPC.DataSource = sv.DsPhanCong(UiHelper.Chon(cboKH)); }

        void BtnKeHoach(object s, EventArgs e)
        {
            try { UiHelper.Ok("Đã lập " + sv.LapKeHoach(UiHelper.Chon(cboPB), dtTuan.Value.Date, txtGhiChu.Text.Trim())); cboKH.DataSource = sv.DsKeHoach(); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnPhanCong(object s, EventArgs e)
        {
            try { sv.PhanCong(UiHelper.Chon(cboKH), UiHelper.Chon(cboNV), dtNgay.Value.Date, cboCa.SelectedItem.ToString(), txtCV.Text.Trim()); NapPhanCong(); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnChamCong(object s, EventArgs e)
        {
            try
            {
                decimal gio;
                if (!decimal.TryParse(txtGio.Text, out gio)) throw new Exception("Số giờ phải là số.");
                sv.ChamCong(UiHelper.Chon(cboNV2), dtCC.Value.Date, gio);
                UiHelper.Ok("Đã chấm công.");
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnThemDong(object s, EventArgs e)
        {
            try
            {
                decimal gio;
                if (!decimal.TryParse(txtGioPT.Text, out gio)) throw new Exception("Số giờ phải là số.");
                pt.Add(new DongPhuTroi { MaNV = UiHelper.Chon(cboNV3), SoGio = gio });
                var dt = new System.Data.DataTable();
                dt.Columns.Add("Mã NV"); dt.Columns.Add("Số giờ");
                foreach (var d in pt) dt.Rows.Add(d.MaNV, d.SoGio);
                gPT.DataSource = dt;
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnLuuPT(object s, EventArgs e)
        {
            try
            {
                string ma = sv.LapPhieuPhuTroi(UiHelper.Chon(cboPB2), dtPT.Value.Date, pt);
                UiHelper.Ok("Đã lập phiếu " + ma);
                pt.Clear(); gPT.DataSource = null;
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
