using System;
using System.Windows.Forms;

namespace QuanLyDuLich.Forms
{
    public class FrmMain : Form
    {
        public FrmMain()
        {
            Text = "Quản Lý Công Ty Du Lịch";
            Width = 420;
            Height = 80 + 4 * 50;
            StartPosition = FormStartPosition.CenterScreen;
            Them("Tour và chuyến đi", 20, () => new FrmTour());
            Them("Đăng ký tour", 70, () => new FrmDangKy());
            Them("Phân công hướng dẫn viên", 120, () => new FrmPhanCong());
            Them("Lương và khảo sát", 170, () => new FrmLuongKhaoSat());
        }

        void Them(string text, int top, Func<Form> tao)
        {
            var b = new Button { Text = text, Left = 60, Top = top, Width = 280, Height = 36 };
            b.Click += (s, e) =>
            {
                try { using (var f = tao()) f.ShowDialog(this); }
                catch (Exception ex) { UiHelper.Loi(ex); }
            };
            Controls.Add(b);
        }
    }
}
