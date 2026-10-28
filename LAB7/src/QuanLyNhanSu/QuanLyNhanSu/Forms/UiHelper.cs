using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhanSu.Data;

namespace QuanLyNhanSu.Forms
{
    // Hàm dựng control dùng chung, giao diện không chứa quy tắc nghiệp vụ
    public static class UiHelper
    {
        public static Label Lbl(Control parent, string text, int x, int y)
        {
            var l = new Label { Text = text, Left = x, Top = y + 3, AutoSize = true };
            parent.Controls.Add(l);
            return l;
        }

        public static TextBox Txt(Control parent, int x, int y, int w)
        {
            var t = new TextBox { Left = x, Top = y, Width = w };
            parent.Controls.Add(t);
            return t;
        }

        public static DateTimePicker Date(Control parent, int x, int y, int addDays)
        {
            var d = new DateTimePicker { Left = x, Top = y, Width = 110, Format = DateTimePickerFormat.Short };
            d.Value = DateTime.Today.AddDays(addDays);
            parent.Controls.Add(d);
            return d;
        }

        public static ComboBox Combo(Control parent, DataTable dt, string display, string value, int x, int y, int w)
        {
            var c = new ComboBox { Left = x, Top = y, Width = w, DropDownStyle = ComboBoxStyle.DropDownList };
            c.DataSource = dt;
            c.DisplayMember = display;
            c.ValueMember = value;
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

        public static DataGridView Grid(Control parent, int x, int y, int w, int h)
        {
            var g = new DataGridView
            {
                Left = x, Top = y, Width = w, Height = h, ReadOnly = true,
                AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, BackgroundColor = Color.White
            };
            parent.Controls.Add(g);
            return g;
        }

        public static string Chon(ComboBox c)
        {
            return c.SelectedValue == null ? null : Convert.ToString(c.SelectedValue);
        }

        public static int So(TextBox t, string ten)
        {
            int n;
            if (!int.TryParse(t.Text.Trim(), out n)) throw new NghiepVuException(ten + " phải là số nguyên.");
            return n;
        }

        public static decimal Tien(TextBox t, string ten)
        {
            decimal n;
            if (!decimal.TryParse(t.Text.Trim(), out n)) throw new NghiepVuException(ten + " phải là số.");
            return n;
        }

        public static void Loi(Exception ex)
        {
            if (ex is NghiepVuException)
                MessageBox.Show(ex.Message, "Không thực hiện được", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Ok(string msg)
        {
            MessageBox.Show(msg, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static Form Base(string title, int w, int h)
        {
            return new Form { Text = title, Width = w, Height = h, StartPosition = FormStartPosition.CenterParent };
        }
    }
}
