using System;
using System.Windows.Forms;
using QuanLyDuLich.Services;

namespace QuanLyDuLich.Forms
{
    public class FrmTour : Form
    {
        DanhMucService sv = new DanhMucService();
        DataGridView gTour, gChuyen;
        TextBox txtMa, txtTen, txtNgay, txtDem, txtGia, txtCong, txtDon;
        ComboBox cboTour; DateTimePicker dtDi, dtVe;

        public FrmTour()
        {
            Text = "Tour và chuyến đi"; Width = 1000; Height = 680; StartPosition = FormStartPosition.CenterParent;
            gTour = UiHelper.Grid(this, 10, 10, 960, 200);
            UiHelper.Lbl(this, "Mã", 10, 225); txtMa = UiHelper.Txt(this, 40, 225, 60);
            UiHelper.Lbl(this, "Tên tour", 110, 225); txtTen = UiHelper.Txt(this, 170, 225, 200);
            UiHelper.Lbl(this, "Ngày", 380, 225); txtNgay = UiHelper.Txt(this, 420, 225, 40);
            UiHelper.Lbl(this, "Đêm", 470, 225); txtDem = UiHelper.Txt(this, 505, 225, 40);
            UiHelper.Lbl(this, "Giá/khách", 555, 225); txtGia = UiHelper.Txt(this, 625, 225, 90);
            UiHelper.Lbl(this, "Tiền công HDV", 725, 225); txtCong = UiHelper.Txt(this, 815, 225, 90);
            UiHelper.Btn(this, "Thêm tour", 10, 260, 100, BtnThemTour);

            gChuyen = UiHelper.Grid(this, 10, 310, 960, 240);
            UiHelper.Lbl(this, "Tour", 10, 570); cboTour = UiHelper.Combo(this, sv.DsTour(), "TenTour", "MaTour", 50, 568, 220);
            UiHelper.Lbl(this, "Ngày đi", 290, 570); dtDi = UiHelper.Date(this, 345, 568, 10);
            UiHelper.Lbl(this, "Ngày về", 470, 570); dtVe = UiHelper.Date(this, 525, 568, 13);
            UiHelper.Lbl(this, "Nơi đón", 650, 570); txtDon = UiHelper.Txt(this, 705, 568, 150);
            UiHelper.Btn(this, "Thêm chuyến", 865, 566, 100, BtnThemChuyen);
            Load += (s, e) => Nap();
        }

        void Nap() { gTour.DataSource = sv.DsTour(); gChuyen.DataSource = sv.DsChuyen(); }

        void BtnThemTour(object s, EventArgs e)
        {
            try
            {
                sv.ThemTour(txtMa.Text.Trim(), txtTen.Text.Trim(), UiHelper.So(txtNgay, "Số ngày"), UiHelper.So(txtDem, "Số đêm"),
                    UiHelper.Tien(txtGia, "Đơn giá"), UiHelper.Tien(txtCong, "Tiền công"));
                UiHelper.Ok("Đã thêm tour.");
                Nap();
                cboTour.DataSource = sv.DsTour();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }

        void BtnThemChuyen(object s, EventArgs e)
        {
            try
            {
                string ma = sv.ThemChuyen(UiHelper.Chon(cboTour), dtDi.Value.Date, dtVe.Value.Date, txtDon.Text.Trim());
                UiHelper.Ok("Đã thêm chuyến " + ma);
                Nap();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
