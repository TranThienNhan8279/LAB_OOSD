using System;
using System.Windows.Forms;
using QuanLyNhanSu.Services;

namespace QuanLyNhanSu.Forms
{
    public class FrmBangLuong : Form
    {
        LuongService sv = new LuongService();
        TextBox txtThang, txtNam; DataGridView g;

        public FrmBangLuong()
        {
            Text = "Bảng lương tháng"; Width = 960; Height = 520; StartPosition = FormStartPosition.CenterParent;
            UiHelper.Lbl(this, "Tháng", 10, 12); txtThang = UiHelper.Txt(this, 55, 10, 40); txtThang.Text = DateTime.Today.Month.ToString();
            UiHelper.Lbl(this, "Năm", 110, 12); txtNam = UiHelper.Txt(this, 145, 10, 60); txtNam.Text = DateTime.Today.Year.ToString();
            UiHelper.Btn(this, "Tính bảng lương", 225, 8, 130, BtnTinh);
            UiHelper.Btn(this, "Xem", 365, 8, 70, (s, e) => Xem());
            g = UiHelper.Grid(this, 10, 50, 920, 410);
        }

        void Xem() { try { g.DataSource = sv.DsBangLuong(UiHelper.So(txtThang, "Tháng"), UiHelper.So(txtNam, "Năm")); } catch (Exception ex) { UiHelper.Loi(ex); } }

        void BtnTinh(object s, EventArgs e)
        {
            try
            {
                int n = sv.TinhBangLuong(UiHelper.So(txtThang, "Tháng"), UiHelper.So(txtNam, "Năm"));
                UiHelper.Ok("Đã tính lương cho " + n + " nhân viên.");
                Xem();
            }
            catch (Exception ex) { UiHelper.Loi(ex); }
        }
    }
}
