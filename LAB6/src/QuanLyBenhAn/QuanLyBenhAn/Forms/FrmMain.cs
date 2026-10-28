using System;
using System.Windows.Forms;

namespace QuanLyBenhAn.Forms
{
    public class FrmMain : Form
    {
        public FrmMain()
        {
            Text = "Quản Lý Hồ Sơ Bệnh Án Của Bệnh Nhân";
            Width = 420;
            Height = 80 + 4 * 50;
            StartPosition = FormStartPosition.CenterScreen;
            Them("Đăng ký và tra cứu hồ sơ", 20, () => new FrmDangKy());
            Them("Khám bệnh", 70, () => new FrmKhamBenh());
            Them("Thanh toán và xuất viện", 120, () => new FrmXuatVien());
            Them("Khám định kỳ", 170, () => new FrmKhamDinhKy());
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
