using System;
using System.Data;
using QuanLyDuLich.Data;

namespace QuanLyDuLich.Services
{
    // Phân công hướng dẫn viên, bảo đảm không chồng chéo lịch
    public class PhanCongService
    {
        // Lấy các khoảng ngày bận của hướng dẫn viên (chuyến lẻ + đoàn), bỏ qua 1 chuyến/đoàn đang xét
        private bool BiTrung(System.Data.SqlClient.SqlTransaction tx, string maNV, DateTime tu, DateTime den)
        {
            var chuyen = DbHelper.Query(tx, "SELECT NgayDi, NgayVe FROM ChuyenDi WHERE MaNV=@m", DbHelper.P("@m", maNV));
            foreach (DataRow r in chuyen.Rows)
                if (tu <= ((DateTime)r["NgayVe"]).Date && den >= ((DateTime)r["NgayDi"]).Date) return true;
            var doan = DbHelper.Query(tx,
                "SELECT p.NgayDi, t.SoNgay FROM PhanCongDoan pc JOIN PhieuDangKyDoan p ON p.MaPhieuDoan=pc.MaPhieuDoan " +
                "JOIN Tour t ON t.MaTour=p.MaTour WHERE pc.MaNV=@m", DbHelper.P("@m", maNV));
            foreach (DataRow r in doan.Rows)
            {
                DateTime d0 = ((DateTime)r["NgayDi"]).Date, d1 = d0.AddDays(Convert.ToInt32(r["SoNgay"]) - 1);
                if (tu <= d1 && den >= d0) return true;
            }
            return false;
        }

        public void PhanCongChuyen(string maChuyen, string maNV)
        {
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    var cd = DbHelper.Query(tx, "SELECT NgayDi, NgayVe FROM ChuyenDi WITH (UPDLOCK) WHERE MaChuyen=@m", DbHelper.P("@m", maChuyen));
                    if (cd.Rows.Count == 0) throw new NghiepVuException("Chuyến đi không tồn tại.");
                    DateTime tu = ((DateTime)cd.Rows[0]["NgayDi"]).Date, den = ((DateTime)cd.Rows[0]["NgayVe"]).Date;
                    if (BiTrung(tx, maNV, tu, den)) throw new NghiepVuException("Hướng dẫn viên đã có lịch trùng với chuyến này.");
                    DbHelper.Exec(tx, "UPDATE ChuyenDi SET MaNV=@n WHERE MaChuyen=@m", DbHelper.P("@n", maNV), DbHelper.P("@m", maChuyen));
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public string PhanCongDoan(string maPhieuDoan, string maNV, string vaiTro)
        {
            if (string.IsNullOrWhiteSpace(vaiTro)) vaiTro = "Hướng dẫn viên";
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    var p = DbHelper.Query(tx, "SELECT p.NgayDi, t.SoNgay FROM PhieuDangKyDoan p WITH (UPDLOCK) JOIN Tour t ON t.MaTour=p.MaTour WHERE p.MaPhieuDoan=@m", DbHelper.P("@m", maPhieuDoan));
                    if (p.Rows.Count == 0) throw new NghiepVuException("Phiếu đăng ký đoàn không tồn tại.");
                    DateTime tu = ((DateTime)p.Rows[0]["NgayDi"]).Date, den = tu.AddDays(Convert.ToInt32(p.Rows[0]["SoNgay"]) - 1);
                    if (DbHelper.Query(tx, "SELECT 1 FROM PhanCongDoan WHERE MaPhieuDoan=@p AND MaNV=@n", DbHelper.P("@p", maPhieuDoan), DbHelper.P("@n", maNV)).Rows.Count > 0)
                        throw new NghiepVuException("Nhân viên này đã được phân công cho đoàn.");
                    if (BiTrung(tx, maNV, tu, den)) throw new NghiepVuException("Hướng dẫn viên đã có lịch trùng với đoàn này.");
                    string ma = DbHelper.NextId(tx, "PhanCongDoan", "MaPC", "PC");
                    DbHelper.Exec(tx, "INSERT INTO PhanCongDoan (MaPC,VaiTro,MaPhieuDoan,MaNV) VALUES (@a,@b,@c,@d)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", vaiTro), DbHelper.P("@c", maPhieuDoan), DbHelper.P("@d", maNV));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public DataTable LichPhanCong(string maNV)
        {
            return DbHelper.Query(
                "SELECT N'Chuyến lẻ' AS Loai, c.MaChuyen AS Ma, t.TenTour, c.NgayDi, c.NgayVe FROM ChuyenDi c JOIN Tour t ON t.MaTour=c.MaTour WHERE c.MaNV=@m " +
                "UNION ALL " +
                "SELECT N'Đoàn', p.MaPhieuDoan, t.TenTour, p.NgayDi, DATEADD(DAY, t.SoNgay - 1, p.NgayDi) " +
                "FROM PhanCongDoan pc JOIN PhieuDangKyDoan p ON p.MaPhieuDoan=pc.MaPhieuDoan JOIN Tour t ON t.MaTour=p.MaTour WHERE pc.MaNV=@m ORDER BY 4",
                DbHelper.P("@m", maNV));
        }
    }
}
