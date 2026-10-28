using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuanLyBenhAn.Services;

namespace QuanLyBenhAn.Forms
{
    // Bác sĩ: lập phiếu khám, chỉ định xét nghiệm, kê toa
    public class FrmKhamBenh : Form
    {
        DanhMucService dm = new DanhMucService();
        KhamBenhService sv = new KhamBenhService();
        TextBox txtHoSo, txtTrieu, txtNhiet, txtHa, txtLoiDan, txtSl, txtLieu, txtKq;
        ComboBox cboBS, cboKhoa, cboPhieu, cboLoaiXN, cboThuoc;
        DataGridView gXN, gToa;
        List<DongThuoc> toa = new List<DongThuoc>();

        public FrmKhamBenh()
        {
            Text = "Khám bệnh"; Width = 1000; Height = 700; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Mã hồ sơ", 10, 12); txtHoSo = UiHelper.Txt(this, 70, 10, 70);
            UiHelper.Lbl(this, "Khoa", 150, 12); cboKhoa = UiHelper.Combo(this, dm.DsKhoa(), "TenKhoa", "MaKhoa", 185, 10, 120);
            UiHelper.Lbl(this, "Bác sĩ", 315, 12); cboBS = UiHelper.Combo(this, dm.DsBacSi(), "HoTen", "MaBS", 360, 10, 150);
            UiHelper.Lbl(this, "Triệu chứng", 520, 12); txtTrieu = UiHelper.Txt(this, 595, 10, 200);
            UiHelper.Lbl(this, "Thân nhiệt", 805, 12); txtNhiet = UiHelper.Txt(this, 870, 10, 50);
            UiHelper.Lbl(this, "Huyết áp", 10, 48); txtHa = UiHelper.Txt(this, 70, 46, 70);
            UiHelper.Btn(this, "Lập phiếu khám", 160, 44, 130, BtnLapPhieu);

            UiHelper.Lbl(this, "Phiếu đang điều trị", 10, 92); cboPhieu = UiHelper.Combo(this, sv.DsPhieuDangDieuTri(), "MaPhieuKham", "MaPhieuKham", 125, 90, 90);
            UiHelper.Btn(this, "Nạp lại", 225, 88, 70, (s, e) => { cboPhieu.DataSource = sv.DsPhieuDangDieuTri(); NapPhieu(); });
            cboPhieu.SelectedIndexChanged += (s, e) => NapPhieu();

            UiHelper.Lbl(this, "Xét nghiệm", 10, 132); cboLoaiXN = UiHelper.Combo(this, dm.DsLoaiXN(), "TenXN", "MaLoaiXN", 85, 130, 160);
            UiHelper.Btn(this, "Chỉ định", 255, 128, 80, BtnChiDinh);
            UiHelper.Lbl(this, "Kết quả", 350, 132); txtKq = UiHelper.Txt(this, 405, 130, 300);
            UiHelper.Btn(this, "Ghi kết quả cho dòng chọn", 715, 128, 200, BtnKetQua);
            gXN = UiHelper.Grid(this, 10, 165, 960, 130);

            UiHelper.Lbl(this, "Thuốc", 10, 312); cboThuoc = UiHelper.Combo(this, dm.DsThuoc(), "TenThuoc", "MaThuoc", 55, 310, 170);
            UiHelper.Lbl(this, "Số lượng", 240, 312); txtSl = UiHelper.Txt(this, 300, 310, 40);
            UiHelper.Lbl(this, "Liều dùng", 350, 312); txtLieu = UiHelper.Txt(this, 415, 310, 250);
            UiHelper.Btn(this, "Thêm vào toa", 675, 308, 110, BtnThemThuoc);
            UiHelper.Lbl(this, "Lời dặn", 10, 352); txtLoiDan = UiHelper.Txt(this, 70, 350, 500);
            UiHelper.Btn(this, "Lưu toa thuốc", 590, 348, 120, BtnLuuToa);
            gToa = UiHelper.Grid(this, 10, 390, 960, 250);
            gToa.AutoGenerateColumns = true;
        }

        string Phieu() { return UiHelper.Chon(cboPhieu); }

        void NapPhieu()
        {
            toa.Clear();
            if (Phieu() == null) { gXN.DataSource = null; gToa.DataSource = null; return; }
            gXN.DataSource = sv.DsXetNghiem(Phieu());
            gToa.DataSource = sv.ChiTietToa(Phieu());
        }

        void BtnLapPhieu(object s, EventArgs e)
        {
            try
            {
                decimal nhiet;
                if (!decimal.TryParse(txtNhiet.Text, out nhiet)) throw new Exception("Thân nhiệt phải là số.");
                string ma = sv.LapPhieuKham(txtHoSo.Text.Trim(), UiHelper.Chon(cboBS), UiHelper.Chon(cboKhoa), txtTrieu.Text.Trim(), nhiet, txtHa.Text.Trim());
                UiHelper.Ok("Đã lập phiếu khám " + ma);
                cboPhieu.DataSource = sv.DsPhieuDangDieuTri();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnChiDinh(object s, EventArgs e)
        {
            try { sv.ChiDinhXetNghiem(Phieu(), UiHelper.Chon(cboLoaiXN)); gXN.DataSource = sv.DsXetNghiem(Phieu()); }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnKetQua(object s, EventArgs e)
        {
            try
            {
                if (gXN.CurrentRow == null) throw new Exception("Chọn một xét nghiệm.");
                sv.GhiKetQuaXetNghiem(gXN.CurrentRow.Cells["MaXN"].Value.ToString(), txtKq.Text.Trim());
                gXN.DataSource = sv.DsXetNghiem(Phieu());
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnThemThuoc(object s, EventArgs e)
        {
            try
            {
                toa.Add(new DongThuoc { MaThuoc = UiHelper.Chon(cboThuoc), SoLuong = UiHelper.So(txtSl, "Số lượng"), LieuDung = txtLieu.Text.Trim() });
                var dt = new System.Data.DataTable();
                dt.Columns.Add("Thuốc"); dt.Columns.Add("Số lượng"); dt.Columns.Add("Liều dùng");
                foreach (var d in toa) dt.Rows.Add(d.MaThuoc, d.SoLuong, d.LieuDung);
                gToa.DataSource = dt;
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnLuuToa(object s, EventArgs e)
        {
            try
            {
                string ma = sv.LapToaThuoc(Phieu(), txtLoiDan.Text.Trim(), toa);
                UiHelper.Ok("Đã lập toa " + ma);
                NapPhieu();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
