using System;
using System.Windows.Forms;
using QuanLyDuLich.Services;

namespace QuanLyDuLich.Forms
{
    // Tính lương nhân viên và nhập phiếu khảo sát sau tour
    public class FrmLuongKhaoSat : Form
    {
        DanhMucService dm = new DanhMucService();
        LuongKhaoSatService sv = new LuongKhaoSatService();
        DangKyService dk = new DangKyService();
        ComboBox cboNv, cboKh, cboChuyen, cboDoan; TextBox txtThang, txtNam, txtDiem, txtGopY; DataGridView gLuong, gKs;

        public FrmLuongKhaoSat()
        {
            Text = "Lương nhân viên và khảo sát"; Width = 1000; Height = 700; StartPosition = FormStartPosition.CenterParent;
            var tab = new TabControl { Dock = DockStyle.Fill }; Controls.Add(tab);
            var t1 = new TabPage("Tính lương"); var t2 = new TabPage("Khảo sát");
            tab.TabPages.Add(t1); tab.TabPages.Add(t2);

            UiHelper.Lbl(t1, "Nhân viên", 10, 12); cboNv = UiHelper.Combo(t1, dm.DsNhanVien(), "HoTen", "MaNV", 75, 10, 180);
            UiHelper.Lbl(t1, "Tháng", 270, 12); txtThang = UiHelper.Txt(t1, 315, 10, 40); txtThang.Text = DateTime.Today.Month.ToString();
            UiHelper.Lbl(t1, "Năm", 370, 12); txtNam = UiHelper.Txt(t1, 400, 10, 60); txtNam.Text = DateTime.Today.Year.ToString();
            UiHelper.Btn(t1, "Tính lương", 480, 8, 100, BtnLuong);
            gLuong = UiHelper.Grid(t1, 10, 50, 960, 570);

            UiHelper.Lbl(t2, "Khách", 10, 12); cboKh = UiHelper.Combo(t2, dm.DsKhachHang(), "Ten", "MaKH", 55, 10, 200);
            UiHelper.Lbl(t2, "Chuyến", 270, 12); cboChuyen = UiHelper.Combo(t2, dm.DsChuyen(), "MaChuyen", "MaChuyen", 325, 10, 90);
            UiHelper.Lbl(t2, "hoặc đoàn", 425, 12); cboDoan = UiHelper.Combo(t2, dk.DsPhieuDoan(), "MaPhieuDoan", "MaPhieuDoan", 495, 10, 90);
            cboChuyen.SelectedIndex = -1; cboDoan.SelectedIndex = -1;
            UiHelper.Lbl(t2, "Điểm (1-5)", 600, 12); txtDiem = UiHelper.Txt(t2, 670, 10, 40);
            UiHelper.Lbl(t2, "Góp ý", 10, 52); txtGopY = UiHelper.Txt(t2, 55, 50, 600);
            UiHelper.Btn(t2, "Ghi nhận khảo sát", 670, 48, 150, BtnKhaoSat);
            gKs = UiHelper.Grid(t2, 10, 90, 960, 530);
            Load += (s, e) => { gLuong.DataSource = sv.DsLuong(); gKs.DataSource = sv.DsKhaoSat(); };
        }

        void BtnLuong(object s, EventArgs e)
        {
            try
            {
                decimal tong = sv.TinhLuong(UiHelper.Chon(cboNv), UiHelper.So(txtThang, "Tháng"), UiHelper.So(txtNam, "Năm"));
                UiHelper.Ok("Tổng lương tháng: " + tong.ToString("N0") + " đ.");
                gLuong.DataSource = sv.DsLuong();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnKhaoSat(object s, EventArgs e)
        {
            try
            {
                string ma = sv.GuiKhaoSat(UiHelper.Chon(cboKh), UiHelper.Chon(cboChuyen), UiHelper.Chon(cboDoan), UiHelper.So(txtDiem, "Điểm"), txtGopY.Text.Trim());
                UiHelper.Ok("Đã ghi nhận phiếu khảo sát " + ma);
                gKs.DataSource = sv.DsKhaoSat();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
