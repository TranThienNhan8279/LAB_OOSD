using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuanLyDuLich.Data;
using QuanLyDuLich.Services;

namespace QuanLyDuLich.Forms
{
    // Hai tab: đăng ký khách lẻ theo chuyến và đăng ký khách đoàn
    public class FrmDangKy : Form
    {
        DanhMucService dm = new DanhMucService();
        DangKyService sv = new DangKyService();
        ComboBox cboKl, cboChuyen, cboKd, cboTour, cboDoanQt;
        TextBox txtSoLe, txtDonLe, txtSoDoan, txtDonDoan;
        DateTimePicker dtDi; CheckBox chkBh; DataGridView gLe, gDoan, gNguoi;

        public FrmDangKy()
        {
            Text = "Đăng ký tour"; Width = 1000; Height = 700; StartPosition = FormStartPosition.CenterParent;
            var tab = new TabControl { Dock = DockStyle.Fill }; Controls.Add(tab);
            var t1 = new TabPage("Khách lẻ (theo chuyến)"); var t2 = new TabPage("Khách đoàn");
            tab.TabPages.Add(t1); tab.TabPages.Add(t2);

            UiHelper.Lbl(t1, "Khách", 10, 12); cboKl = UiHelper.Combo(t1, dm.DsKhachLe(), "HoTen", "MaKH", 60, 10, 180);
            UiHelper.Lbl(t1, "Chuyến", 260, 12); cboChuyen = UiHelper.Combo(t1, dm.DsChuyen(), "MaChuyen", "MaChuyen", 315, 10, 90);
            UiHelper.Lbl(t1, "Số người", 420, 12); txtSoLe = UiHelper.Txt(t1, 485, 10, 50);
            UiHelper.Lbl(t1, "Nơi đón", 550, 12); txtDonLe = UiHelper.Txt(t1, 605, 10, 200);
            UiHelper.Btn(t1, "Đăng ký", 820, 8, 100, BtnLe);
            gLe = UiHelper.Grid(t1, 10, 50, 960, 560);

            UiHelper.Lbl(t2, "Khách đoàn", 10, 12); cboKd = UiHelper.Combo(t2, dm.DsKhachDoan(), "TenCoQuan", "MaKH", 85, 10, 200);
            UiHelper.Lbl(t2, "Tour", 300, 12); cboTour = UiHelper.Combo(t2, dm.DsTour(), "TenTour", "MaTour", 335, 10, 200);
            UiHelper.Lbl(t2, "Ngày đi", 550, 12); dtDi = UiHelper.Date(t2, 605, 10, 14);
            UiHelper.Lbl(t2, "Số người", 730, 12); txtSoDoan = UiHelper.Txt(t2, 795, 10, 50);
            UiHelper.Lbl(t2, "Nơi đón", 10, 47); txtDonDoan = UiHelper.Txt(t2, 70, 45, 300);
            chkBh = new CheckBox { Text = "Có mua bảo hiểm (nhập danh sách người đi bên dưới)", Left = 390, Top = 46, Width = 380 }; t2.Controls.Add(chkBh);
            UiHelper.Btn(t2, "Đăng ký đoàn", 800, 43, 130, BtnDoan);
            gNguoi = new DataGridView { Left = 10, Top = 85, Width = 960, Height = 180, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = System.Drawing.Color.White };
            gNguoi.Columns.Add("HoTen", "Họ tên"); gNguoi.Columns.Add("NgaySinh", "Ngày sinh (dd/MM/yyyy)"); gNguoi.Columns.Add("CMND", "Số CMND");
            t2.Controls.Add(gNguoi);
            gDoan = UiHelper.Grid(t2, 10, 280, 960, 250);
            UiHelper.Lbl(t2, "Quyết toán đoàn", 10, 550); cboDoanQt = UiHelper.Combo(t2, sv.DsPhieuDoan(), "MaPhieuDoan", "MaPhieuDoan", 110, 548, 100);
            UiHelper.Btn(t2, "Quyết toán sau tour", 230, 546, 150, BtnQuyetToan);
            Load += (s, e) => Nap();
        }

        void Nap()
        {
            gLe.DataSource = sv.DsPhieuLe();
            gDoan.DataSource = sv.DsPhieuDoan();
            cboDoanQt.DataSource = sv.DsPhieuDoan();
        }

        void BtnLe(object s, EventArgs e)
        {
            try
            {
                string ma = sv.DangKyChuyen(UiHelper.Chon(cboKl), UiHelper.Chon(cboChuyen), UiHelper.So(txtSoLe, "Số người"), txtDonLe.Text.Trim());
                UiHelper.Ok("Đã lập phiếu đăng ký chuyến " + ma);
                Nap();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnDoan(object s, EventArgs e)
        {
            try
            {
                var ds = new List<NguoiDi>();
                foreach (DataGridViewRow r in gNguoi.Rows)
                {
                    if (r.IsNewRow || r.Cells["HoTen"].Value == null) continue;
                    DateTime ns;
                    DateTime? ngaySinh = null;
                    if (DateTime.TryParse(Convert.ToString(r.Cells["NgaySinh"].Value), out ns)) ngaySinh = ns;
                    ds.Add(new NguoiDi { HoTen = Convert.ToString(r.Cells["HoTen"].Value), NgaySinh = ngaySinh, SoCMND = Convert.ToString(r.Cells["CMND"].Value) });
                }
                string ma = sv.DangKyDoan(UiHelper.Chon(cboKd), UiHelper.Chon(cboTour), dtDi.Value.Date, UiHelper.So(txtSoDoan, "Số người"),
                    txtDonDoan.Text.Trim(), chkBh.Checked, ds);
                UiHelper.Ok("Đã lập phiếu đăng ký đoàn " + ma + " (đã ghi nhận tiền đặt cọc).");
                gNguoi.Rows.Clear();
                Nap();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnQuyetToan(object s, EventArgs e)
        {
            try
            {
                decimal tien = sv.QuyetToanDoan(UiHelper.Chon(cboDoanQt));
                UiHelper.Ok("Đã quyết toán, số tiền thu thêm: " + tien.ToString("N0") + " đ.");
                Nap();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
