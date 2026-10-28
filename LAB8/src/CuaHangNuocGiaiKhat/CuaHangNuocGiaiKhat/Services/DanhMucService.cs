using System.Data;
using CuaHangNuocGiaiKhat.Data;

namespace CuaHangNuocGiaiKhat.Services
{
    /// <summary>Danh mục: loại NGK, nhà cung ứng, nước giải khát, nhân viên, khách hàng.</summary>
    public class DanhMucService
    {
        public DataTable DsNhaCungUng() { return DbHelper.Query("SELECT * FROM NhaCungUng ORDER BY MaNCC"); }
        public DataTable DsLoai() { return DbHelper.Query("SELECT * FROM LoaiNuocGiaiKhat ORDER BY MaLoai"); }
        public DataTable DsNhanVien() { return DbHelper.Query("SELECT * FROM NhanVien ORDER BY MaNV"); }
        public DataTable DsKhachHang() { return DbHelper.Query("SELECT * FROM KhachHang ORDER BY MaKH"); }

        public DataTable DsNuocGiaiKhat(string tuKhoa)
        {
            return DbHelper.Query(
                "SELECT n.MaNGK, n.TenNGK, l.TenLoai, n.DonViTinh, n.DonGiaBan, n.SoLuongTon " +
                "FROM NuocGiaiKhat n JOIN LoaiNuocGiaiKhat l ON l.MaLoai = n.MaLoai " +
                "WHERE n.TenNGK LIKE @k OR n.MaNGK LIKE @k ORDER BY n.MaNGK",
                DbHelper.P("@k", "%" + (tuKhoa ?? "") + "%"));
        }

        public void ThemNuocGiaiKhat(string ma, string maLoai, string ten, string dvt, decimal gia, int ton)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten))
                throw new NghiepVuException("Mã và tên nước giải khát không được để trống.");
            if (gia < 0 || ton < 0) throw new NghiepVuException("Đơn giá và tồn kho phải >= 0.");
            if (DbHelper.Query("SELECT 1 FROM NuocGiaiKhat WHERE MaNGK=@m", DbHelper.P("@m", ma)).Rows.Count > 0)
                throw new NghiepVuException("Mã nước giải khát đã tồn tại.");
            DbHelper.Exec("INSERT INTO NuocGiaiKhat VALUES (@ma,@loai,@ten,@dvt,@gia,@ton)",
                DbHelper.P("@ma", ma), DbHelper.P("@loai", maLoai), DbHelper.P("@ten", ten),
                DbHelper.P("@dvt", dvt), DbHelper.P("@gia", gia), DbHelper.P("@ton", ton));
        }

        public void CapNhatGia(string ma, decimal gia)
        {
            if (gia < 0) throw new NghiepVuException("Đơn giá phải >= 0.");
            DbHelper.Exec("UPDATE NuocGiaiKhat SET DonGiaBan=@g WHERE MaNGK=@m", DbHelper.P("@g", gia), DbHelper.P("@m", ma));
        }

        public void ThemKhachHang(string ma, string ten, string diaChi, string sdt, bool quen)
        {
            if (string.IsNullOrWhiteSpace(ma) || string.IsNullOrWhiteSpace(ten))
                throw new NghiepVuException("Mã và họ tên khách hàng không được để trống.");
            DbHelper.Exec("INSERT INTO KhachHang (MaKH,HoTen,DiaChi,SoDienThoai,LoaiKhach,NgayDangKy) VALUES (@ma,@ten,@dc,@sdt,@lk,@nd)",
                DbHelper.P("@ma", ma), DbHelper.P("@ten", ten), DbHelper.P("@dc", diaChi), DbHelper.P("@sdt", sdt),
                DbHelper.P("@lk", quen ? "QUEN" : "VANGLAI"),
                DbHelper.P("@nd", quen ? (object)System.DateTime.Today : null));
        }
    }
}
