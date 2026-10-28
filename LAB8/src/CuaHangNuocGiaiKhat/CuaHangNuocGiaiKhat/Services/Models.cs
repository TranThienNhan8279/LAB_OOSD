using System;
using System.Collections.Generic;

namespace CuaHangNuocGiaiKhat.Services
{
    public class DongHang
    {
        public string MaNGK;
        public int SoLuong;
        public decimal DonGia;   // chỉ dùng cho phiếu giao (giá nhập)
    }

    public class KetQuaBanHang
    {
        public string MaHD;
        public decimal TongTien;
        public string MaPhieuHen;   // null nếu giao đủ
        public bool ConThieu { get { return MaPhieuHen != null; } }
    }
}
