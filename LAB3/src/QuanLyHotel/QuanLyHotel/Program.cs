using System;
using System.Windows.Forms;
using QuanLyHotel.Forms;

namespace QuanLyHotel
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
