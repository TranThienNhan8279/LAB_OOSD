using System.Data;
using System.Data.SqlClient;
using QuanLyHotel.Data;

namespace QuanLyHotel.Services
{
    /// <summary>Nghiệp vụ cho FrmPhongTienNghi: Phong, TienNghi, PhieuLapDat.</summary>
    public class PhongTienNghiService
    {
        // ---------- Phong ----------
        public DataTable GetPhong() => DbHelper.ExecuteQuery(
            @"SELECT p.SoPhong, p.MaKhuVuc, k.TenKhuVuc, p.SoNguoiToiDa, p.DonGiaNgay, p.TrangThai
              FROM Phong p JOIN KhuVuc k ON p.MaKhuVuc = k.MaKhuVuc
              ORDER BY p.SoPhong");

        public int ThemPhong(string soPhong, string maKhuVuc, int soNguoiToiDa, decimal donGia) => DbHelper.ExecuteNonQuery(
            "INSERT INTO Phong(SoPhong,MaKhuVuc,SoNguoiToiDa,DonGiaNgay) VALUES(@sp,@kv,@sn,@dg)",
            new SqlParameter("@sp", soPhong), new SqlParameter("@kv", maKhuVuc),
            new SqlParameter("@sn", soNguoiToiDa), new SqlParameter("@dg", donGia));

        public int SuaPhong(string soPhong, string maKhuVuc, int soNguoiToiDa, decimal donGia, string trangThai) => DbHelper.ExecuteNonQuery(
            "UPDATE Phong SET MaKhuVuc=@kv,SoNguoiToiDa=@sn,DonGiaNgay=@dg,TrangThai=@tt WHERE SoPhong=@sp",
            new SqlParameter("@sp", soPhong), new SqlParameter("@kv", maKhuVuc),
            new SqlParameter("@sn", soNguoiToiDa), new SqlParameter("@dg", donGia),
            new SqlParameter("@tt", trangThai));

        public int XoaPhong(string soPhong) => DbHelper.ExecuteNonQuery(
            "DELETE FROM Phong WHERE SoPhong=@sp", new SqlParameter("@sp", soPhong));

        public int CapNhatTrangThaiPhong(string soPhong, string trangThai) => DbHelper.ExecuteNonQuery(
            "UPDATE Phong SET TrangThai=@tt WHERE SoPhong=@sp",
            new SqlParameter("@sp", soPhong), new SqlParameter("@tt", trangThai));

        // ---------- TienNghi ----------
        public DataTable GetTienNghi() => DbHelper.ExecuteQuery(
            @"SELECT t.MaTienNghi, t.MaLoaiTN, l.TenLoaiTN, t.SoThuTu, t.TinhTrangHienTai
              FROM TienNghi t JOIN LoaiTienNghi l ON t.MaLoaiTN = l.MaLoaiTN
              ORDER BY t.MaTienNghi");

        public int ThemTienNghi(string ma, string maLoai, int soThuTu, string tinhTrang) => DbHelper.ExecuteNonQuery(
            "INSERT INTO TienNghi(MaTienNghi,MaLoaiTN,SoThuTu,TinhTrangHienTai) VALUES(@ma,@loai,@stt,@tt)",
            new SqlParameter("@ma", ma), new SqlParameter("@loai", maLoai),
            new SqlParameter("@stt", soThuTu), new SqlParameter("@tt", (object)tinhTrang ?? System.DBNull.Value));

        public int XoaTienNghi(string ma) => DbHelper.ExecuteNonQuery(
            "DELETE FROM TienNghi WHERE MaTienNghi=@ma", new SqlParameter("@ma", ma));

        // ---------- PhieuLapDat ----------
        public DataTable GetPhieuLapDat() => DbHelper.ExecuteQuery(
            @"SELECT pl.SoPhieuLapDat, pl.MaTienNghi, pl.SoPhong, pl.NgayLap, pl.TinhTrang, pl.MaNV, pl.GhiChu
              FROM PhieuLapDat pl ORDER BY pl.NgayLap DESC");

        /// <summary>Thêm phiếu lắp đặt. Ràng buộc UQ(MaTienNghi,NgayLap) đảm bảo 1 thiết bị/1 ngày chỉ 1 phiếu.</summary>
        public int ThemPhieuLapDat(string soPhieu, string maTienNghi, string soPhong,
            System.DateTime ngayLap, string tinhTrang, string maNV, string ghiChu) => DbHelper.ExecuteNonQuery(
            @"INSERT INTO PhieuLapDat(SoPhieuLapDat,MaTienNghi,SoPhong,NgayLap,TinhTrang,MaNV,GhiChu)
              VALUES(@sp,@tn,@ph,@ngay,@tt,@nv,@gc)",
            new SqlParameter("@sp", soPhieu), new SqlParameter("@tn", maTienNghi),
            new SqlParameter("@ph", soPhong), new SqlParameter("@ngay", ngayLap.Date),
            new SqlParameter("@tt", tinhTrang), new SqlParameter("@nv", maNV),
            new SqlParameter("@gc", (object)ghiChu ?? System.DBNull.Value));

        public int XoaPhieuLapDat(string soPhieu) => DbHelper.ExecuteNonQuery(
            "DELETE FROM PhieuLapDat WHERE SoPhieuLapDat=@sp", new SqlParameter("@sp", soPhieu));
    }
}
