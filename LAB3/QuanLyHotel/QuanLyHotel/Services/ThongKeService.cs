using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyHotel.Data;

namespace QuanLyHotel.Services
{
    /// <summary>Nghiệp vụ cho FrmThongKe: doanh thu, công suất phòng, dịch vụ bán chạy.</summary>
    public class ThongKeService
    {
        public DataTable DoanhThuTheoThang(int nam) => DbHelper.ExecuteQuery(
            @"SELECT MONTH(NgayLap) AS Thang,
                     SUM(TienPhong) AS TienPhong, SUM(TienDichVu) AS TienDichVu, SUM(TongTien) AS TongDoanhThu
              FROM HoaDon
              WHERE YEAR(NgayLap)=@nam
              GROUP BY MONTH(NgayLap) ORDER BY Thang",
            new SqlParameter("@nam", nam));

        public DataTable CongSuatPhongTheoNgay(DateTime tuNgay, DateTime denNgay) => DbHelper.ExecuteQuery(
            @"SELECT k.TenKhuVuc, COUNT(DISTINCT p.SoPhong) AS SoPhong,
                     SUM(CASE WHEN p.TrangThai=N'Đang ở' THEN 1 ELSE 0 END) AS DangO
              FROM Phong p JOIN KhuVuc k ON p.MaKhuVuc=k.MaKhuVuc
              GROUP BY k.TenKhuVuc",
            new SqlParameter("@tu", tuNgay), new SqlParameter("@den", denNgay));

        public DataTable TopDichVuBanChay(int top = 5) => DbHelper.ExecuteQuery(
            $@"SELECT TOP {top} dv.TenDV, SUM(ct.SoLuong) AS TongSoLuong, SUM(ct.ThanhTien) AS TongTien
              FROM ChiTietPhieuSuDungDV ct JOIN DichVu dv ON ct.MaDV = dv.MaDV
              GROUP BY dv.TenDV ORDER BY TongTien DESC");

        public DataTable ThongKeDenBuTheoLoai() => DbHelper.ExecuteQuery(
            @"SELECT l.TenLoaiTN, COUNT(*) AS SoLanDenBu, SUM(ct.SoTien) AS TongTienDenBu
              FROM ChiTietPhieuDenBu ct
              JOIN TienNghi tn ON ct.MaTienNghi = tn.MaTienNghi
              JOIN LoaiTienNghi l ON tn.MaLoaiTN = l.MaLoaiTN
              GROUP BY l.TenLoaiTN");
    }
}
