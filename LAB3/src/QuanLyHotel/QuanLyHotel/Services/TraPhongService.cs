using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyHotel.Data;

namespace QuanLyHotel.Services
{
    /// <summary>Nghiệp vụ cho FrmTraPhong: PhieuDenBu, HoaDon, ThanhToan, trả phòng.</summary>
    public class TraPhongService
    {
        // ---------- Quy định đền bù (tra cứu khi lập phiếu đền bù) ----------
        public DataTable GetQuyDinhDenBuTheoLoai(string maLoaiTN) => DbHelper.ExecuteQuery(
            "SELECT * FROM QuyDinhDenBu WHERE MaLoaiTN=@loai", new SqlParameter("@loai", maLoaiTN));

        public string TaoSoPhieuDenBuMoi()
        {
            var kq = DbHelper.ExecuteScalar("SELECT ISNULL(MAX(CAST(SUBSTRING(SoPhieuDenBu,4,20) AS INT)),0)+1 FROM PhieuDenBu WHERE SoPhieuDenBu LIKE 'PDB%'");
            return "PDB" + kq.ToString().PadLeft(5, '0');
        }

        public int TaoPhieuDenBu(string soPhieu, string soPhieuDat, string soPhong, string maNV, decimal tongTien) => DbHelper.ExecuteNonQuery(
            @"INSERT INTO PhieuDenBu(SoPhieuDenBu,SoPhieuDat,SoPhong,NgayLap,MaNV,TongTien)
              VALUES(@sp,@pd,@ph,GETDATE(),@nv,@tien)",
            new SqlParameter("@sp", soPhieu), new SqlParameter("@pd", soPhieuDat),
            new SqlParameter("@ph", soPhong), new SqlParameter("@nv", maNV), new SqlParameter("@tien", tongTien));

        public int ThemChiTietDenBu(string soPhieuDenBu, string maTienNghi, string mucDo, decimal soTien) => DbHelper.ExecuteNonQuery(
            @"INSERT INTO ChiTietPhieuDenBu(SoPhieuDenBu,MaTienNghi,MucDoThietHai,SoTien)
              VALUES(@sp,@tn,@md,@tien)",
            new SqlParameter("@sp", soPhieuDenBu), new SqlParameter("@tn", maTienNghi),
            new SqlParameter("@md", mucDo), new SqlParameter("@tien", soTien));

        // ---------- Hóa đơn ----------
        public string TaoSoHoaDonMoi()
        {
            var kq = DbHelper.ExecuteScalar("SELECT ISNULL(MAX(CAST(SUBSTRING(SoHoaDon,3,20) AS INT)),0)+1 FROM HoaDon WHERE SoHoaDon LIKE 'HD%'");
            return "HD" + kq.ToString().PadLeft(5, '0');
        }

        /// <summary>Tính tiền phòng = số ngày ở * đơn giá (đơn giản: 1 phòng chính của phiếu đặt).</summary>
        public decimal TinhTienPhong(string soPhieuDat, out int soNgay)
        {
            var dt = DbHelper.ExecuteQuery(
                @"SELECT pd.NgayNhan, pd.NgayTraDuKien,
                         SUM(p.DonGiaNgay) AS TongDonGiaCacPhong
                  FROM PhieuDatPhong pd
                  JOIN ChiTietDatPhong ct ON pd.SoPhieuDat = ct.SoPhieuDat
                  JOIN Phong p ON ct.SoPhong = p.SoPhong
                  WHERE pd.SoPhieuDat=@sp
                  GROUP BY pd.NgayNhan, pd.NgayTraDuKien",
                new SqlParameter("@sp", soPhieuDat));

            if (dt.Rows.Count == 0) { soNgay = 0; return 0; }

            DateTime nhan = Convert.ToDateTime(dt.Rows[0]["NgayNhan"]);
            DateTime tra = Convert.ToDateTime(dt.Rows[0]["NgayTraDuKien"]);
            soNgay = Math.Max(1, (tra - nhan).Days);
            decimal tongDonGia = Convert.ToDecimal(dt.Rows[0]["TongDonGiaCacPhong"]);
            return soNgay * tongDonGia;
        }

        /// <summary>Lập hóa đơn (tiền phòng + tiền dịch vụ), đổi trạng thái phiếu đặt/phòng, trong 1 transaction.</summary>
        public string LapHoaDonVaTraPhong(string soPhieuDat, string maNV, int soNgay,
            decimal tienPhong, decimal tienDichVu)
        {
            string soHoaDon = TaoSoHoaDonMoi();
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(
                            @"INSERT INTO HoaDon(SoHoaDon,SoPhieuDat,NgayLap,MaNV,SoNgayTinhTien,TienPhong,TienDichVu)
                              VALUES(@hd,@pd,GETDATE(),@nv,@sn,@tp,@tdv)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@hd", soHoaDon);
                            cmd.Parameters.AddWithValue("@pd", soPhieuDat);
                            cmd.Parameters.AddWithValue("@nv", maNV);
                            cmd.Parameters.AddWithValue("@sn", soNgay);
                            cmd.Parameters.AddWithValue("@tp", tienPhong);
                            cmd.Parameters.AddWithValue("@tdv", tienDichVu);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand(
                            @"UPDATE PhieuDatPhong SET TrangThai=N'Đã trả', NgayTraThucTe=GETDATE() WHERE SoPhieuDat=@sp", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@sp", soPhieuDat);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand(
                            @"UPDATE Phong SET TrangThai=N'Trống'
                              WHERE SoPhong IN (SELECT SoPhong FROM ChiTietDatPhong WHERE SoPhieuDat=@sp)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@sp", soPhieuDat);
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return soHoaDon;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        // ---------- Thanh toán ----------
        public string TaoMaThanhToanMoi()
        {
            var kq = DbHelper.ExecuteScalar("SELECT ISNULL(MAX(CAST(SUBSTRING(MaThanhToan,3,20) AS INT)),0)+1 FROM ThanhToan WHERE MaThanhToan LIKE 'TT%'");
            return "TT" + kq.ToString().PadLeft(5, '0');
        }

        public int ThanhToanHoaDon(string soHoaDon, string hinhThuc, decimal soTien)
        {
            string maTT = TaoMaThanhToanMoi();
            DbHelper.ExecuteNonQuery(
                @"INSERT INTO ThanhToan(MaThanhToan,SoHoaDon,NgayThanhToan,HinhThuc,SoTien)
                  VALUES(@ma,@hd,GETDATE(),@ht,@tien)",
                new SqlParameter("@ma", maTT), new SqlParameter("@hd", soHoaDon),
                new SqlParameter("@ht", hinhThuc), new SqlParameter("@tien", soTien));

            return DbHelper.ExecuteNonQuery(
                "UPDATE HoaDon SET TrangThai=N'Đã thanh toán' WHERE SoHoaDon=@hd",
                new SqlParameter("@hd", soHoaDon));
        }

        public DataTable GetHoaDonChuaThanhToan() => DbHelper.ExecuteQuery(
            "SELECT * FROM HoaDon WHERE TrangThai=N'Chưa thanh toán' ORDER BY NgayLap DESC");
    }
}
