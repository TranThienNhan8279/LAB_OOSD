using System;
using System.Windows.Forms;
using QuanLyBenhAn.Forms;

namespace QuanLyBenhAn
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
