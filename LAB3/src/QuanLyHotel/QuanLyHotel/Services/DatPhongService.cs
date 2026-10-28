using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyHotel.Data;

namespace QuanLyHotel.Services
{
    /// <summary>Nghiệp vụ cho FrmDatPhong: KhachHang, PhieuDatPhong, ChiTietDatPhong, NguoiLuuTru.</summary>
    public class DatPhongService
    {
        // ---------- KhachHang ----------
        public DataTable GetKhachHang() => DbHelper.ExecuteQuery("SELECT * FROM KhachHang ORDER BY MaKhach");

        public int ThemKhachHang(string ma, string hoTen, string cmnd, string quocTich, string sdt) => DbHelper.ExecuteNonQuery(
            "INSERT INTO KhachHang(MaKhach,HoTen,SoCMND,QuocTich,SoDienThoai) VALUES(@ma,@ht,@cmnd,@qt,@sdt)",
            new SqlParameter("@ma", ma), new SqlParameter("@ht", hoTen), new SqlParameter("@cmnd", cmnd),
            new SqlParameter("@qt", quocTich), new SqlParameter("@sdt", (object)sdt ?? DBNull.Value));

        // ---------- Danh sách phòng còn trống theo khoảng ngày ----------
        /// <summary>Phòng KHÔNG bị trùng lịch với các phiếu đặt còn hiệu lực trong khoảng [ngayNhan, ngayTra).</summary>
        public DataTable GetPhongTrongTheoKhoangNgay(DateTime ngayNhan, DateTime ngayTra) => DbHelper.ExecuteQuery(
            @"SELECT p.SoPhong, p.MaKhuVuc, p.SoNguoiToiDa, p.DonGiaNgay, p.TrangThai
              FROM Phong p
              WHERE p.TrangThai <> N'Bảo trì'
                AND NOT EXISTS (
                    SELECT 1 FROM ChiTietDatPhong ct
                    JOIN PhieuDatPhong pd ON ct.SoPhieuDat = pd.SoPhieuDat
                    WHERE ct.SoPhong = p.SoPhong
                      AND pd.TrangThai IN (N'Đã đặt', N'Đang ở')
                      AND pd.NgayNhan < @tra AND pd.NgayTraDuKien > @nhan
                )
              ORDER BY p.SoPhong",
            new SqlParameter("@nhan", ngayNhan.Date), new SqlParameter("@tra", ngayTra.Date));

        // ---------- PhieuDatPhong ----------
        public DataTable GetPhieuDatPhong() => DbHelper.ExecuteQuery(
            @"SELECT pd.SoPhieuDat, kh.HoTen AS KhachHang, pd.NgayLap, pd.NgayNhan, pd.NgayTraDuKien,
                     pd.TienCoc, pd.KenhDat, pd.TrangThai
              FROM PhieuDatPhong pd JOIN KhachHang kh ON pd.MaKhach = kh.MaKhach
              ORDER BY pd.NgayLap DESC");

        public string TaoSoPhieuDatMoi()
        {
            var kq = DbHelper.ExecuteScalar("SELECT ISNULL(MAX(CAST(SUBSTRING(SoPhieuDat,3,20) AS INT)),0)+1 FROM PhieuDatPhong WHERE SoPhieuDat LIKE 'PD%'");
            return "PD" + kq.ToString().PadLeft(5, '0');
        }

        /// <summary>Tạo phiếu đặt phòng + chi tiết phòng trong 1 transaction.</summary>
        public int TaoPhieuDatPhong(string soPhieuDat, string maKhach, string maNVLeTan,
            DateTime ngayNhan, DateTime ngayTraDuKien, decimal tienCoc, string kenhDat,
            string[] danhSachSoPhong, int[] soNguoiMoiPhong)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(
                            @"INSERT INTO PhieuDatPhong(SoPhieuDat,MaKhach,MaNVLeTan,NgayLap,NgayNhan,NgayTraDuKien,TienCoc,KenhDat)
                              VALUES(@sp,@kh,@nv,GETDATE(),@nhan,@tra,@coc,@kenh)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@sp", soPhieuDat);
                            cmd.Parameters.AddWithValue("@kh", maKhach);
                            cmd.Parameters.AddWithValue("@nv", maNVLeTan);
                            cmd.Parameters.AddWithValue("@nhan", ngayNhan.Date);
                            cmd.Parameters.AddWithValue("@tra", ngayTraDuKien.Date);
                            cmd.Parameters.AddWithValue("@coc", tienCoc);
                            cmd.Parameters.AddWithValue("@kenh", kenhDat);
                            cmd.ExecuteNonQuery();
                        }

                        for (int i = 0; i < danhSachSoPhong.Length; i++)
                        {
                            using (var cmd = new SqlCommand(
                                "INSERT INTO ChiTietDatPhong(SoPhieuDat,SoPhong,SoNguoi) VALUES(@sp,@ph,@sn)", conn, tran))
                            {
                                cmd.Parameters.AddWithValue("@sp", soPhieuDat);
                                cmd.Parameters.AddWithValue("@ph", danhSachSoPhong[i]);
                                cmd.Parameters.AddWithValue("@sn", soNguoiMoiPhong[i]);
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmd2 = new SqlCommand(
                                "UPDATE Phong SET TrangThai=N'Đã đặt' WHERE SoPhong=@ph", conn, tran))
                            {
                                cmd2.Parameters.AddWithValue("@ph", danhSachSoPhong[i]);
                                cmd2.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        return 1;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public int HuyPhieuDat(string soPhieuDat) => DbHelper.ExecuteNonQuery(
            "UPDATE PhieuDatPhong SET TrangThai=N'Hủy' WHERE SoPhieuDat=@sp", new SqlParameter("@sp", soPhieuDat));

        // ---------- NguoiLuuTru ----------
        public DataTable GetNguoiLuuTru(string soPhieuDat, string soPhong) => DbHelper.ExecuteQuery(
            "SELECT * FROM NguoiLuuTru WHERE SoPhieuDat=@sp AND SoPhong=@ph",
            new SqlParameter("@sp", soPhieuDat), new SqlParameter("@ph", soPhong));

        public int ThemNguoiLuuTru(string soPhieuDat, string soPhong, string hoTen, string cmnd, string quocTich) => DbHelper.ExecuteNonQuery(
            @"INSERT INTO NguoiLuuTru(SoPhieuDat,SoPhong,HoTen,SoCMND,QuocTich)
              VALUES(@sp,@ph,@ht,@cmnd,@qt)",
            new SqlParameter("@sp", soPhieuDat), new SqlParameter("@ph", soPhong),
            new SqlParameter("@ht", hoTen), new SqlParameter("@cmnd", cmnd), new SqlParameter("@qt", quocTich));

        /// <summary>Nhận phòng: đánh dấu Đang ở + cập nhật trạng thái các phòng liên quan.</summary>
        public int NhanPhong(string soPhieuDat)
        {
            DbHelper.ExecuteNonQuery(
                "UPDATE PhieuDatPhong SET TrangThai=N'Đang ở', NgayNhanThucTe=GETDATE() WHERE SoPhieuDat=@sp",
                new SqlParameter("@sp", soPhieuDat));
            return DbHelper.ExecuteNonQuery(
                @"UPDATE Phong SET TrangThai=N'Đang ở'
                  WHERE SoPhong IN (SELECT SoPhong FROM ChiTietDatPhong WHERE SoPhieuDat=@sp)",
                new SqlParameter("@sp", soPhieuDat));
        }
    }
}
