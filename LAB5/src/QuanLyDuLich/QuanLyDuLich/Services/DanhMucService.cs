using System;
using System.Data;
using QuanLyDuLich.Data;

namespace QuanLyDuLich.Services
{
    // Danh mục: tour, chuyến đi, khách hàng, nhân viên
    public class DanhMucService
    {
        public DataTable DsTour() { return DbHelper.Query("SELECT * FROM Tour ORDER BY MaTour"); }
        public DataTable DsNhanVien() { return DbHelper.Query("SELECT * FROM NhanVien ORDER BY MaNV"); }
        public DataTable DsKhachHang() { return DbHelper.Query("SELECT MaKH, ISNULL(TenCoQuan, HoTen) AS Ten, LoaiKhachHang, DienThoai FROM KhachHang ORDER BY MaKH"); }
        public DataTable DsKhachLe() { return DbHelper.Query("SELECT MaKH, HoTen FROM KhachHang WHERE LoaiKhachHang = 'KHACHLE' ORDER BY MaKH"); }
        public DataTable DsKhachDoan() { return DbHelper.Query("SELECT MaKH, TenCoQuan FROM KhachHang WHERE LoaiKhachHang = 'KHACHDOAN' ORDER BY MaKH"); }

        public DataTable DsChuyen()
        {
            return DbHelper.Query(
                "SELECT c.MaChuyen, t.TenTour, c.NgayDi, c.NgayVe, c.DiaDiemDon, c.MaNV " +
                "FROM ChuyenDi c JOIN Tour t ON t.MaTour = c.MaTour ORDER BY c.NgayDi");
        }

        public void ThemTour(string ma, string ten, int soNgay, int soDem, decimal donGia, decimal tienCong)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten))
                throw new NghiepVuException("Mã tour và tên tour không được để trống.");
            if (soNgay < 1 || soDem < 0) throw new NghiepVuException("Số ngày phải từ 1, số đêm không âm.");
            if (donGia < 0) throw new NghiepVuException("Đơn giá không được âm.");
            if (DbHelper.Query("SELECT 1 FROM Tour WHERE MaTour=@m", DbHelper.P("@m", ma)).Rows.Count > 0)
                throw new NghiepVuException("Mã tour đã tồn tại.");
            DbHelper.Exec("INSERT INTO Tour (MaTour,TenTour,SoNgay,SoDem,DonGiaKhach,TienCongHDV) VALUES (@a,@b,@c,@d,@e,@f)",
                DbHelper.P("@a", ma), DbHelper.P("@b", ten), DbHelper.P("@c", soNgay), DbHelper.P("@d", soDem),
                DbHelper.P("@e", donGia), DbHelper.P("@f", tienCong));
        }

        public string ThemChuyen(string maTour, DateTime ngayDi, DateTime ngayVe, string diaDiemDon)
        {
            if (ngayVe < ngayDi) throw new NghiepVuException("Ngày về phải sau hoặc bằng ngày đi.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string ma = DbHelper.NextId(tx, "ChuyenDi", "MaChuyen", "CD");
                    DbHelper.Exec(tx, "INSERT INTO ChuyenDi (MaChuyen,NgayDi,NgayVe,DiaDiemDon,MaTour) VALUES (@a,@b,@c,@d,@e)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", ngayDi), DbHelper.P("@c", ngayVe), DbHelper.P("@d", diaDiemDon), DbHelper.P("@e", maTour));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }
    }
}
