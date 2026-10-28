using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using CuaHangNuocGiaiKhat.Data;
using CuaHangNuocGiaiKhat.Services;

namespace CuaHangNuocGiaiKhat.Forms
{
    /// <summary>Hàm dựng control dùng chung cho các Form (UI không chứa quy tắc nghiệp vụ).</summary>
    public static class UiHelper
    {
        public static Label Lbl(Control parent, string text, int x, int y)
        {
            var l = new Label { Text = text, Left = x, Top = y + 3, AutoSize = true };
            parent.Controls.Add(l);
            return l;
        }

        public static ComboBox Combo(Control parent, DataTable dt, string display, string value, int x, int y, int w)
        {
            var c = new ComboBox { Left = x, Top = y, Width = w, DropDownStyle = ComboBoxStyle.DropDownList };
            c.DataSource = dt; c.DisplayMember = display; c.ValueMember = value;
            parent.Controls.Add(c);
            return c;
        }

        public static Button Btn(Control parent, string text, int x, int y, int w, EventHandler click)
        {
            var b = new Button { Text = text, Left = x, Top = y, Width = w, Height = 28 };
            b.Click += click;
            parent.Controls.Add(b);
            return b;
        }

        public static DataGridView Grid(Control parent, int x, int y, int w, int h, bool readOnly)
        {
            var g = new DataGridView
            {
                Left = x, Top = y, Width = w, Height = h, ReadOnly = readOnly,
                AllowUserToAddRows = !readOnly, AllowUserToDeleteRows = !readOnly,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = Color.White
            };
            parent.Controls.Add(g);
            return g;
        }

        /// <summary>Lưới nhập dòng hàng: cột chọn nước giải khát + số lượng (+ đơn giá nếu cần).</summary>
        public static DataGridView LineGrid(Control parent, DataTable ngk, int x, int y, int w, int h, bool coDonGia)
        {
            var g = Grid(parent, x, y, w, h, false);
            g.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "MaNGK", HeaderText = "Nước giải khát", DataSource = ngk,
                DisplayMember = "TenNGK", ValueMember = "MaNGK"
            });
            g.Columns.Add("SoLuong", "Số lượng");
            if (coDonGia) g.Columns.Add("DonGia", "Đơn giá nhập");
            return g;
        }

        public static List<DongHang> DocDong(DataGridView g, bool coDonGia)
        {
            var ds = new List<DongHang>();
            foreach (DataGridViewRow r in g.Rows)
            {
                if (r.IsNewRow || r.Cells["MaNGK"].Value == null) continue;
                int sl;
                if (!int.TryParse(Convert.ToString(r.Cells["SoLuong"].Value), out sl))
                    throw new NghiepVuException("Số lượng phải là số nguyên.");
                decimal gia = 0;
                if (coDonGia && !decimal.TryParse(Convert.ToString(r.Cells["DonGia"].Value), out gia))
                    throw new NghiepVuException("Đơn giá nhập phải là số.");
                ds.Add(new DongHang { MaNGK = (string)r.Cells["MaNGK"].Value, SoLuong = sl, DonGia = gia });
            }
            return ds;
        }

        public static void Loi(Exception ex)
        {
            if (ex is NghiepVuException)
                MessageBox.Show(ex.Message, "Không thực hiện được", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Ok(string msg) { MessageBox.Show(msg, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information); }

        public static Form Base(string title, int w, int h)
        {
            return new Form { Text = title, Width = w, Height = h, StartPosition = FormStartPosition.CenterParent };
        }
    }
}
