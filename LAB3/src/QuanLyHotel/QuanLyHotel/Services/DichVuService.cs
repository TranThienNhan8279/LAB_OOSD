using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyHotel.Data;

namespace QuanLyHotel.Services
{
    /// <summary>Nghiệp vụ cho FrmDichVu: PhieuSuDungDV, ChiTietPhieuSuDungDV.</summary>
    public class DichVuService
    {
        public DataTable GetPhieuSuDungDV() => DbHelper.ExecuteQuery(
            @"SELECT p.SoPhieuSDDV, p.SoPhieuDat, p.SoPhong, p.NgaySuDung, p.MaNV,
                     ISNULL((SELECT SUM(ThanhTien) FROM ChiTietPhieuSuDungDV c WHERE c.SoPhieuSDDV = p.SoPhieuSDDV),0) AS TongTien
              FROM PhieuSuDungDV p ORDER BY p.NgaySuDung DESC");

        public DataTable GetChiTiet(string soPhieuSDDV) => DbHelper.ExecuteQuery(
            @"SELECT ct.MaDV, dv.TenDV, ct.SoLuong, ct.DonGia, ct.ThanhTien
              FROM ChiTietPhieuSuDungDV ct JOIN DichVu dv ON ct.MaDV = dv.MaDV
              WHERE ct.SoPhieuSDDV=@sp", new SqlParameter("@sp", soPhieuSDDV));

        public string TaoSoPhieuSDDVMoi()
        {
            var kq = DbHelper.ExecuteScalar("SELECT ISNULL(MAX(CAST(SUBSTRING(SoPhieuSDDV,4,20) AS INT)),0)+1 FROM PhieuSuDungDV WHERE SoPhieuSDDV LIKE 'PDV%'");
            return "PDV" + kq.ToString().PadLeft(5, '0');
        }

        /// <summary>Tạo phiếu sử dụng dịch vụ. Ràng buộc UQ(SoPhieuDat,SoPhong,NgaySuDung) => mỗi phòng/ngày chỉ 1 phiếu, dùng chức năng thêm dòng chi tiết.</summary>
        public int TaoPhieuSuDungDV(string soPhieu, string soPhieuDat, string soPhong, DateTime ngaySuDung, string maNV) => DbHelper.ExecuteNonQuery(
            @"INSERT INTO PhieuSuDungDV(SoPhieuSDDV,SoPhieuDat,SoPhong,NgaySuDung,MaNV)
              VALUES(@sp,@pd,@ph,@ngay,@nv)",
            new SqlParameter("@sp", soPhieu), new SqlParameter("@pd", soPhieuDat),
            new SqlParameter("@ph", soPhong), new SqlParameter("@ngay", ngaySuDung.Date),
            new SqlParameter("@nv", maNV));

        public int ThemChiTietDichVu(string soPhieuSDDV, string maDV, int soLuong, decimal donGia) => DbHelper.ExecuteNonQuery(
            @"INSERT INTO ChiTietPhieuSuDungDV(SoPhieuSDDV,MaDV,SoLuong,DonGia)
              VALUES(@sp,@dv,@sl,@dg)",
            new SqlParameter("@sp", soPhieuSDDV), new SqlParameter("@dv", maDV),
            new SqlParameter("@sl", soLuong), new SqlParameter("@dg", donGia));

        public int XoaChiTietDichVu(string soPhieuSDDV, string maDV) => DbHelper.ExecuteNonQuery(
            "DELETE FROM ChiTietPhieuSuDungDV WHERE SoPhieuSDDV=@sp AND MaDV=@dv",
            new SqlParameter("@sp", soPhieuSDDV), new SqlParameter("@dv", maDV));

        /// <summary>Tổng tiền dịch vụ của 1 phiếu đặt phòng (dùng khi lập hóa đơn).</summary>
        public decimal TongTienDichVuTheoPhieuDat(string soPhieuDat)
        {
            var kq = DbHelper.ExecuteScalar(
                @"SELECT ISNULL(SUM(ct.ThanhTien),0)
                  FROM ChiTietPhieuSuDungDV ct JOIN PhieuSuDungDV p ON ct.SoPhieuSDDV = p.SoPhieuSDDV
                  WHERE p.SoPhieuDat=@sp", new SqlParameter("@sp", soPhieuDat));
            return Convert.ToDecimal(kq);
        }
    }
}
