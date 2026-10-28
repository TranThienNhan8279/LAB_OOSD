using System;
using System.Windows.Forms;
using CuaHangNuocGiaiKhat.Forms;

namespace CuaHangNuocGiaiKhat
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
