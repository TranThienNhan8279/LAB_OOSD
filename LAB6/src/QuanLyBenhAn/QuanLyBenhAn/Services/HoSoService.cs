using System;
using System.Data;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    // Đăng ký lần đầu, đóng lệ phí năm, cấp hồ sơ, tra cứu hồ sơ
    public class HoSoService
    {
        public const decimal LePhiNam = 200000;   // giả định: lệ phí quản lý 200.000đ/năm

        // Phiếu đăng ký + lệ phí năm + hồ sơ bệnh án phải cùng thành công hoặc cùng huỷ
        public string DangKyLanDau(string hoTen, string diaChi, DateTime ngaySinh, string dienThoai, string anh)
        {
            if (string.IsNullOrWhiteSpace(hoTen)) throw new NghiepVuException("Họ tên không được để trống.");
            if (ngaySinh.Date >= DateTime.Today) throw new NghiepVuException("Ngày sinh phải trước hôm nay.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string bn = DbHelper.NextId(tx, "BenhNhan", "MaBN", "BN");
                    DbHelper.Exec(tx, "INSERT INTO BenhNhan (MaBN,HoTen,DiaChi,NgaySinh,DienThoai,AnhChup) VALUES (@a,@b,@c,@d,@e,@f)",
                        DbHelper.P("@a", bn), DbHelper.P("@b", hoTen), DbHelper.P("@c", diaChi), DbHelper.P("@d", ngaySinh.Date),
                        DbHelper.P("@e", dienThoai), DbHelper.P("@f", anh));
                    DbHelper.Exec(tx, "INSERT INTO PhieuDangKy (MaPhieuDK,NgayDangKy,MaBN) VALUES (@a,@b,@c)",
                        DbHelper.P("@a", DbHelper.NextId(tx, "PhieuDangKy", "MaPhieuDK", "PD")), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", bn));
                    DbHelper.Exec(tx, "INSERT INTO LePhi (MaLePhi,Nam,SoTien,NgayDong,MaBN) VALUES (@a,@b,@c,@d,@e)",
                        DbHelper.P("@a", DbHelper.NextId(tx, "LePhi", "MaLePhi", "LP")), DbHelper.P("@b", DateTime.Today.Year),
                        DbHelper.P("@c", LePhiNam), DbHelper.P("@d", DateTime.Today), DbHelper.P("@e", bn));
                    string hs = DbHelper.NextId(tx, "HoSoBenhAn", "MaHoSo", "HS");
                    DbHelper.Exec(tx, "INSERT INTO HoSoBenhAn (MaHoSo,NgayCap,MaBN) VALUES (@a,@b,@c)",
                        DbHelper.P("@a", hs), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", bn));
                    tx.Commit();
                    return hs;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        // Gia hạn: đóng lệ phí cho năm hiện tại
        public void DongLePhi(string maBN)
        {
            if (DbHelper.Query("SELECT 1 FROM BenhNhan WHERE MaBN=@m", DbHelper.P("@m", maBN)).Rows.Count == 0)
                throw new NghiepVuException("Không tìm thấy bệnh nhân.");
            if (ConHan(maBN, DateTime.Today.Year))
                throw new NghiepVuException("Bệnh nhân đã đóng lệ phí năm " + DateTime.Today.Year + ".");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    DbHelper.Exec(tx, "INSERT INTO LePhi (MaLePhi,Nam,SoTien,NgayDong,MaBN) VALUES (@a,@b,@c,@d,@e)",
                        DbHelper.P("@a", DbHelper.NextId(tx, "LePhi", "MaLePhi", "LP")), DbHelper.P("@b", DateTime.Today.Year),
                        DbHelper.P("@c", LePhiNam), DbHelper.P("@d", DateTime.Today), DbHelper.P("@e", maBN));
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public static bool ConHan(string maBN, int nam)
        {
            return DbHelper.Query("SELECT 1 FROM LePhi WHERE MaBN=@b AND Nam=@n", DbHelper.P("@b", maBN), DbHelper.P("@n", nam)).Rows.Count > 0;
        }

        // Tìm theo mã hồ sơ, họ tên hoặc điện thoại
        public DataTable TraCuu(string tuKhoa)
        {
            string k = "%" + (tuKhoa ?? "").Trim() + "%";
            return DbHelper.Query(
                "SELECT h.MaHoSo, b.MaBN, b.HoTen, b.NgaySinh, b.DienThoai, b.DiaChi " +
                "FROM HoSoBenhAn h JOIN BenhNhan b ON b.MaBN = h.MaBN " +
                "WHERE h.MaHoSo LIKE @k OR b.HoTen LIKE @k OR b.DienThoai LIKE @k ORDER BY h.MaHoSo", DbHelper.P("@k", k));
        }

        public DataTable LichSu(string maHoSo)
        {
            return DbHelper.Query(
                "SELECT p.MaPhieuKham, p.NgayGioKham, p.LoaiKham, k.TenKhoa, b.HoTen AS BacSi, p.TrieuChung, p.ThanNhiet, p.HuyetAp, p.TrangThai, " +
                "  (SELECT COUNT(*) FROM XetNghiem x WHERE x.MaPhieuKham = p.MaPhieuKham) AS SoXetNghiem, " +
                "  CASE WHEN EXISTS (SELECT 1 FROM ToaThuoc t WHERE t.MaPhieuKham = p.MaPhieuKham) THEN N'Có' ELSE N'Không' END AS CoToa " +
                "FROM PhieuKham p JOIN BacSi b ON b.MaBS = p.MaBS JOIN Khoa k ON k.MaKhoa = p.MaKhoa " +
                "WHERE p.MaHoSo=@h ORDER BY p.NgayGioKham DESC", DbHelper.P("@h", maHoSo));
        }
    }
}
