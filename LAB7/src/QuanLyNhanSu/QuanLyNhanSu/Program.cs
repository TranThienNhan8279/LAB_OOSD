using System;
using System.Windows.Forms;
using QuanLyNhanSu.Forms;

namespace QuanLyNhanSu
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
