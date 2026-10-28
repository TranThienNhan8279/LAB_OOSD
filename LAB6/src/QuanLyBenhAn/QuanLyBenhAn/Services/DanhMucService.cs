using System;
using System.Data;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    // Danh mục: khoa, bác sĩ, nhân viên, thuốc, loại xét nghiệm
    public class DanhMucService
    {
        public DataTable DsKhoa() { return DbHelper.Query("SELECT * FROM Khoa ORDER BY MaKhoa"); }
        public DataTable DsBacSi() { return DbHelper.Query("SELECT b.MaBS, b.HoTen, b.ChuyenMon, k.TenKhoa, b.MaKhoa FROM BacSi b JOIN Khoa k ON k.MaKhoa = b.MaKhoa ORDER BY b.MaBS"); }
        public DataTable DsNhanVien() { return DbHelper.Query("SELECT * FROM NhanVien ORDER BY MaNV"); }
        public DataTable DsThuoc() { return DbHelper.Query("SELECT * FROM Thuoc ORDER BY MaThuoc"); }
        public DataTable DsLoaiXN() { return DbHelper.Query("SELECT * FROM LoaiXetNghiem ORDER BY MaLoaiXN"); }

        public void ThemThuoc(string ma, string ten, string dvt)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(dvt))
                throw new NghiepVuException("Nhập đủ mã thuốc, tên thuốc và đơn vị tính.");
            if (DbHelper.Query("SELECT 1 FROM Thuoc WHERE MaThuoc=@m", DbHelper.P("@m", ma)).Rows.Count > 0)
                throw new NghiepVuException("Mã thuốc đã tồn tại.");
            DbHelper.Exec("INSERT INTO Thuoc (MaThuoc,TenThuoc,DonViTinh) VALUES (@a,@b,@c)",
                DbHelper.P("@a", ma), DbHelper.P("@b", ten), DbHelper.P("@c", dvt));
        }
    }
}
