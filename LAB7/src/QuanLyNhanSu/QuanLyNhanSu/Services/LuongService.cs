using System;
using System.Data;
using QuanLyNhanSu.Data;

namespace QuanLyNhanSu.Services
{
    // Bảng lương tháng: lương, phụ cấp, làm thêm giờ, thưởng, trừ bảo hiểm và phạt
    public class LuongService
    {
        const decimal GioChuanThang = 26 * 8;   // giả định: 26 ngày công, 8 giờ mỗi ngày

        public DataTable DsBangLuong(int thang, int nam)
        {
            return DbHelper.Query(
                "SELECT b.MaBL, n.HoTen, b.LuongCoBan, b.PhuCap, b.TienPhuTroi, b.TienThuong, b.TienBaoHiem, b.TienPhat, b.ThucLanh " +
                "FROM BangLuong b JOIN NhanVien n ON n.MaNV = b.MaNV WHERE b.Thang=@t AND b.Nam=@n ORDER BY n.MaNV",
                DbHelper.P("@t", thang), DbHelper.P("@n", nam));
        }

        // Tính lại cả tháng; tính lại lần nữa thì ghi đè dòng cũ
        public int TinhBangLuong(int thang, int nam)
        {
            if (thang < 1 || thang > 12) throw new NghiepVuException("Tháng từ 1 đến 12.");
            DateTime dauThang = new DateTime(nam, thang, 1), cuoiThang = dauThang.AddMonths(1).AddDays(-1);
            if (dauThang > DateTime.Today) throw new NghiepVuException("Chưa tới tháng cần tính lương.");
            var ds = DbHelper.Query("SELECT MaNV, MaPB FROM NhanVien WHERE TrangThai <> N'Đã nghỉ' ORDER BY MaNV");
            int dem = 0;
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    foreach (DataRow r in ds.Rows)
                    {
                        string nv = r["MaNV"].ToString(), pb = r["MaPB"].ToString();
                        decimal cb; bool coHopDong;
                        if (!LayLuongCoBan(tx, nv, dauThang, cuoiThang, out cb, out coHopDong)) continue;

                        decimal phuCap = 0;
                        if (coHopDong)
                            phuCap = Dec(tx, "SELECT TOP 1 c.PhuCap FROM QuyetDinhChucVu q JOIN ChucVu c ON c.MaCV = q.MaCV WHERE q.MaNV=@n AND q.NgayHieuLuc <= @e ORDER BY q.NgayHieuLuc DESC",
                                DbHelper.P("@n", nv), DbHelper.P("@e", cuoiThang));

                        decimal gioPT = Dec(tx, "SELECT SUM(d.SoGio * p.HeSo) FROM ChiTietPhuTroi d JOIN PhieuPhuTroi p ON p.MaPPT = d.MaPPT WHERE d.MaNV=@n AND p.Ngay BETWEEN @s AND @e",
                            DbHelper.P("@n", nv), DbHelper.P("@s", dauThang), DbHelper.P("@e", cuoiThang));
                        decimal phuTroi = Math.Round(gioPT * cb / GioChuanThang, 0);

                        decimal thuong = Dec(tx, "SELECT SUM(c.SoTien) FROM PhieuChi c JOIN QuyetDinhKhenThuong k ON k.MaKT = c.MaKT WHERE k.MaNV=@n AND c.NgayChi BETWEEN @s AND @e",
                            DbHelper.P("@n", nv), DbHelper.P("@s", dauThang), DbHelper.P("@e", cuoiThang));
                        // khen thưởng tập thể chia đều cho nhân viên đang làm của phòng ban
                        thuong += Math.Round(Dec(tx,
                            "SELECT SUM(c.SoTien * 1.0 / (SELECT COUNT(*) FROM NhanVien x WHERE x.MaPB = k.MaPB AND x.TrangThai <> N'Đã nghỉ')) " +
                            "FROM PhieuChi c JOIN QuyetDinhKhenThuong k ON k.MaKT = c.MaKT WHERE k.MaPB=@p AND c.NgayChi BETWEEN @s AND @e",
                            DbHelper.P("@p", pb), DbHelper.P("@s", dauThang), DbHelper.P("@e", cuoiThang)), 0);

                        decimal baoHiem = 0;
                        if (coHopDong)
                            baoHiem = Math.Round(cb * Dec(tx,
                                "SELECT b.TyLeTrich FROM BaoHiem b JOIN HopDong h ON h.MaHD = b.MaHD WHERE h.MaNV=@n AND h.NgayBatDau <= @e AND (h.NgayKetThuc IS NULL OR h.NgayKetThuc >= @s)",
                                DbHelper.P("@n", nv), DbHelper.P("@s", dauThang), DbHelper.P("@e", cuoiThang)), 0);

                        decimal phat = Dec(tx, "SELECT SUM(t.SoTien) FROM PhieuThu t JOIN QuyetDinhKyLuat k ON k.MaKL = t.MaKL WHERE k.MaNV=@n AND t.NgayThu BETWEEN @s AND @e",
                            DbHelper.P("@n", nv), DbHelper.P("@s", dauThang), DbHelper.P("@e", cuoiThang));

                        decimal thucLanh = cb + phuCap + phuTroi + thuong - baoHiem - phat;
                        DbHelper.Exec(tx, "DELETE FROM BangLuong WHERE MaNV=@n AND Thang=@t AND Nam=@y", DbHelper.P("@n", nv), DbHelper.P("@t", thang), DbHelper.P("@y", nam));
                        DbHelper.Exec(tx,
                            "INSERT INTO BangLuong (MaBL,Thang,Nam,LuongCoBan,PhuCap,TienPhuTroi,TienThuong,TienBaoHiem,TienPhat,ThucLanh,MaNV) VALUES (@a,@b,@c,@d,@e,@f,@g,@h,@i,@j,@k)",
                            DbHelper.P("@a", DbHelper.NextId(tx, "BangLuong", "MaBL", "BL")), DbHelper.P("@b", thang), DbHelper.P("@c", nam), DbHelper.P("@d", cb),
                            DbHelper.P("@e", phuCap), DbHelper.P("@f", phuTroi), DbHelper.P("@g", thuong), DbHelper.P("@h", baoHiem), DbHelper.P("@i", phat),
                            DbHelper.P("@j", thucLanh), DbHelper.P("@k", nv));
                        dem++;
                    }
                    tx.Commit();
                    return dem;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        // Có hợp đồng thì lấy quyết định lương mới nhất (hoặc lương hợp đồng); chưa có thì lương thử việc
        static bool LayLuongCoBan(System.Data.SqlClient.SqlTransaction tx, string nv, DateTime dau, DateTime cuoi, out decimal luong, out bool coHopDong)
        {
            var hd = DbHelper.Query(tx, "SELECT TOP 1 LuongHopDong FROM HopDong WHERE MaNV=@n AND NgayBatDau <= @e AND (NgayKetThuc IS NULL OR NgayKetThuc >= @s) ORDER BY NgayBatDau DESC",
                DbHelper.P("@n", nv), DbHelper.P("@s", dau), DbHelper.P("@e", cuoi));
            if (hd.Rows.Count > 0)
            {
                coHopDong = true;
                luong = Convert.ToDecimal(hd.Rows[0][0]);
                var ql = DbHelper.Query(tx, "SELECT TOP 1 LuongMoi FROM QuyetDinhLuong WHERE MaNV=@n AND NgayHieuLuc <= @e ORDER BY NgayHieuLuc DESC", DbHelper.P("@n", nv), DbHelper.P("@e", cuoi));
                if (ql.Rows.Count > 0) luong = Convert.ToDecimal(ql.Rows[0][0]);
                return true;
            }
            coHopDong = false;
            var tv = DbHelper.Query(tx, "SELECT LuongThuViec FROM QuyetDinhTuyenDung WHERE MaNV=@n AND NgayBatDauThuViec <= @e AND NgayKetThucThuViec >= @s", DbHelper.P("@n", nv), DbHelper.P("@s", dau), DbHelper.P("@e", cuoi));
            if (tv.Rows.Count == 0) { luong = 0; return false; }
            luong = Convert.ToDecimal(tv.Rows[0][0]);
            return true;
        }

        static decimal Dec(System.Data.SqlClient.SqlTransaction tx, string sql, params System.Data.SqlClient.SqlParameter[] ps)
        {
            var dt = DbHelper.Query(tx, sql, ps);
            if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value) return 0;
            return Convert.ToDecimal(dt.Rows[0][0]);
        }
    }
}
