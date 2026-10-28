using System;
using System.Windows.Forms;

namespace CuaHangNuocGiaiKhat.Forms
{
    public class FrmMain : Form
    {
        public FrmMain()
        {
            Text = "Quản lý cửa hàng nước giải khát"; Width = 420; Height = 360; StartPosition = FormStartPosition.CenterScreen;
            AddBtn("Danh mục", 20, () => new FrmDanhMuc());
            AddBtn("Đặt hàng / Nhận hàng NCC", 70, () => new FrmMuaHang());
            AddBtn("Bán hàng", 120, () => new FrmBanHang());
            AddBtn("Phiếu hẹn giao", 170, () => new FrmPhieuHen());
            AddBtn("Phiếu ghi nợ", 220, () => new FrmGhiNo());
        }

        private void AddBtn(string text, int top, Func<Form> make)
        {
            var b = new Button { Text = text, Left = 60, Top = top, Width = 280, Height = 36 };
            b.Click += (s, e) => { try { using (var f = make()) f.ShowDialog(this); } catch (Exception ex) { UiHelper.Loi(ex); } };
            Controls.Add(b);
        }
    }
}
