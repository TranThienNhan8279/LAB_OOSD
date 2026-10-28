using System;
using System.Windows.Forms;

namespace QuanLyNhanSu.Forms
{
    public class FrmMain : Form
    {
        public FrmMain()
        {
            Text = "Quản Lý Nhân Sự";
            Width = 420;
            Height = 80 + 5 * 50;
            StartPosition = FormStartPosition.CenterScreen;
            Them("Nhân viên và hợp đồng", 20, () => new FrmNhanVien());
            Them("Quyết định chức vụ, lương, bảo hiểm", 70, () => new FrmQuyetDinh());
            Them("Phân công, chấm công, phụ trội", 120, () => new FrmChamCong());
            Them("Khen thưởng và kỷ luật", 170, () => new FrmThuongPhat());
            Them("Bảng lương tháng", 220, () => new FrmBangLuong());
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
