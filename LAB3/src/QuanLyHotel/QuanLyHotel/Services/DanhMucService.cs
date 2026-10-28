using System.Data;
using System.Data.SqlClient;
using QuanLyHotel.Data;

namespace QuanLyHotel.Services
{
    /// <summary>Nghiệp vụ cho FrmDanhMuc: KhuVuc, NhanVien, LoaiTienNghi, DichVu, QuyDinhDenBu.</summary>
    public class DanhMucService
    {
        // ---------- KhuVuc ----------
        public DataTable GetKhuVuc() => DbHelper.ExecuteQuery("SELECT * FROM KhuVuc ORDER BY MaKhuVuc");

        public int ThemKhuVuc(string ma, string ten) => DbHelper.ExecuteNonQuery(
            "INSERT INTO KhuVuc(MaKhuVuc,TenKhuVuc) VALUES(@ma,@ten)",
            new SqlParameter("@ma", ma), new SqlParameter("@ten", ten));

        public int SuaKhuVuc(string ma, string ten) => DbHelper.ExecuteNonQuery(
            "UPDATE KhuVuc SET TenKhuVuc=@ten WHERE MaKhuVuc=@ma",
            new SqlParameter("@ma", ma), new SqlParameter("@ten", ten));

        public int XoaKhuVuc(string ma) => DbHelper.ExecuteNonQuery(
            "DELETE FROM KhuVuc WHERE MaKhuVuc=@ma", new SqlParameter("@ma", ma));

        // ---------- NhanVien ----------
        public DataTable GetNhanVien() => DbHelper.ExecuteQuery("SELECT * FROM NhanVien ORDER BY MaNV");

        public int ThemNhanVien(string ma, string hoTen, string vaiTro, string sdt) => DbHelper.ExecuteNonQuery(
            "INSERT INTO NhanVien(MaNV,HoTen,VaiTro,SoDienThoai) VALUES(@ma,@ht,@vt,@sdt)",
            new SqlParameter("@ma", ma), new SqlParameter("@ht", hoTen),
            new SqlParameter("@vt", vaiTro), new SqlParameter("@sdt", (object)sdt ?? System.DBNull.Value));

        public int SuaNhanVien(string ma, string hoTen, string vaiTro, string sdt) => DbHelper.ExecuteNonQuery(
            "UPDATE NhanVien SET HoTen=@ht,VaiTro=@vt,SoDienThoai=@sdt WHERE MaNV=@ma",
            new SqlParameter("@ma", ma), new SqlParameter("@ht", hoTen),
            new SqlParameter("@vt", vaiTro), new SqlParameter("@sdt", (object)sdt ?? System.DBNull.Value));

        public int XoaNhanVien(string ma) => DbHelper.ExecuteNonQuery(
            "DELETE FROM NhanVien WHERE MaNV=@ma", new SqlParameter("@ma", ma));

        // ---------- LoaiTienNghi ----------
        public DataTable GetLoaiTienNghi() => DbHelper.ExecuteQuery("SELECT * FROM LoaiTienNghi ORDER BY MaLoaiTN");

        public int ThemLoaiTienNghi(string ma, string ten) => DbHelper.ExecuteNonQuery(
            "INSERT INTO LoaiTienNghi(MaLoaiTN,TenLoaiTN) VALUES(@ma,@ten)",
            new SqlParameter("@ma", ma), new SqlParameter("@ten", ten));

        public int XoaLoaiTienNghi(string ma) => DbHelper.ExecuteNonQuery(
            "DELETE FROM LoaiTienNghi WHERE MaLoaiTN=@ma", new SqlParameter("@ma", ma));

        // ---------- DichVu ----------
        public DataTable GetDichVu() => DbHelper.ExecuteQuery("SELECT * FROM DichVu ORDER BY MaDV");

        public int ThemDichVu(string ma, string ten, string dvt, decimal donGia) => DbHelper.ExecuteNonQuery(
            "INSERT INTO DichVu(MaDV,TenDV,DonViTinh,DonGia) VALUES(@ma,@ten,@dvt,@dg)",
            new SqlParameter("@ma", ma), new SqlParameter("@ten", ten),
            new SqlParameter("@dvt", dvt), new SqlParameter("@dg", donGia));

        public int SuaDichVu(string ma, string ten, string dvt, decimal donGia) => DbHelper.ExecuteNonQuery(
            "UPDATE DichVu SET TenDV=@ten,DonViTinh=@dvt,DonGia=@dg WHERE MaDV=@ma",
            new SqlParameter("@ma", ma), new SqlParameter("@ten", ten),
            new SqlParameter("@dvt", dvt), new SqlParameter("@dg", donGia));

        public int XoaDichVu(string ma) => DbHelper.ExecuteNonQuery(
            "DELETE FROM DichVu WHERE MaDV=@ma", new SqlParameter("@ma", ma));

        // ---------- QuyDinhDenBu ----------
        public DataTable GetQuyDinhDenBu() => DbHelper.ExecuteQuery(
            @"SELECT q.MaQuyDinh, q.MaLoaiTN, l.TenLoaiTN, q.MucDoThietHai, q.MucDenBu
              FROM QuyDinhDenBu q JOIN LoaiTienNghi l ON q.MaLoaiTN = l.MaLoaiTN
              ORDER BY q.MaQuyDinh");

        public int ThemQuyDinhDenBu(string ma, string maLoai, string mucDo, decimal mucDenBu) => DbHelper.ExecuteNonQuery(
            "INSERT INTO QuyDinhDenBu(MaQuyDinh,MaLoaiTN,MucDoThietHai,MucDenBu) VALUES(@ma,@loai,@md,@tien)",
            new SqlParameter("@ma", ma), new SqlParameter("@loai", maLoai),
            new SqlParameter("@md", mucDo), new SqlParameter("@tien", mucDenBu));
    }
}
