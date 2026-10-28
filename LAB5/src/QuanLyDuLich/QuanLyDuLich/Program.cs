using System;
using System.Windows.Forms;
using QuanLyDuLich.Forms;

namespace QuanLyDuLich
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
