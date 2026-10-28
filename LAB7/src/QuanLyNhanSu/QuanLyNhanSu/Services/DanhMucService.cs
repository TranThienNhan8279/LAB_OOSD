using System;
using System.Data;
using QuanLyNhanSu.Data;

namespace QuanLyNhanSu.Services
{
    // Danh mục dùng cho các màn hình
    public class DanhMucService
    {
        public DataTable DsPhongBan() { return DbHelper.Query("SELECT * FROM PhongBan ORDER BY MaPB"); }
        public DataTable DsChucVu() { return DbHelper.Query("SELECT * FROM ChucVu ORDER BY MaCV"); }
        public DataTable DsNhanVien()
        {
            return DbHelper.Query("SELECT n.MaNV, n.HoTen, n.NgaySinh, n.DienThoai, n.TrangThai, p.TenPB, n.MaPB FROM NhanVien n JOIN PhongBan p ON p.MaPB = n.MaPB ORDER BY n.MaNV");
        }
        public DataTable DsNhanVienDangLam()
        {
            return DbHelper.Query("SELECT MaNV, HoTen FROM NhanVien WHERE TrangThai <> N'Đã nghỉ' ORDER BY MaNV");
        }
        public DataTable DsHopDong()
        {
            return DbHelper.Query("SELECT h.MaHD, n.HoTen, h.LoaiHD, h.NgayBatDau, h.NgayKetThuc, h.LuongHopDong FROM HopDong h JOIN NhanVien n ON n.MaNV = h.MaNV ORDER BY h.MaHD");
        }
        public DataTable DsHopDongDaiHan()
        {
            return DbHelper.Query("SELECT h.MaHD, h.MaHD + N' - ' + n.HoTen AS Ten FROM HopDong h JOIN NhanVien n ON n.MaNV = h.MaNV WHERE h.LoaiHD = N'Dài hạn' ORDER BY h.MaHD");
        }
    }
}
