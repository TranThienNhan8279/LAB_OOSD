using System;
using System.Data;
using QuanLyDuLich.Data;

namespace QuanLyDuLich.Services
{
    // Tính lương tháng và ghi nhận phiếu khảo sát sau tour
    public class LuongKhaoSatService
    {
        // Lương tháng = lương căn bản + tiền công của các tour nhân viên dẫn trong tháng (tính theo ngày đi)
        public decimal TinhLuong(string maNV, int thang, int nam)
        {
            if (thang < 1 || thang > 12) throw new NghiepVuException("Tháng phải từ 1 đến 12.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    var nv = DbHelper.Query(tx, "SELECT LuongCoBan FROM NhanVien WHERE MaNV=@m", DbHelper.P("@m", maNV));
                    if (nv.Rows.Count == 0) throw new NghiepVuException("Nhân viên không tồn tại.");
                    decimal coBan = Convert.ToDecimal(nv.Rows[0]["LuongCoBan"]);
                    decimal theoTour = Convert.ToDecimal(DbHelper.Scalar(tx,
                        "SELECT ISNULL(SUM(x.TienCong),0) FROM (" +
                        " SELECT t.TienCongHDV AS TienCong FROM ChuyenDi c JOIN Tour t ON t.MaTour=c.MaTour " +
                        "  WHERE c.MaNV=@m AND MONTH(c.NgayDi)=@t AND YEAR(c.NgayDi)=@n " +
                        " UNION ALL " +
                        " SELECT t.TienCongHDV FROM PhanCongDoan pc JOIN PhieuDangKyDoan p ON p.MaPhieuDoan=pc.MaPhieuDoan JOIN Tour t ON t.MaTour=p.MaTour " +
                        "  WHERE pc.MaNV=@m AND MONTH(p.NgayDi)=@t AND YEAR(p.NgayDi)=@n) x",
                        DbHelper.P("@m", maNV), DbHelper.P("@t", thang), DbHelper.P("@n", nam)));
                    decimal tong = coBan + theoTour;
                    var cu = DbHelper.Query(tx, "SELECT MaLuong FROM LuongThang WHERE MaNV=@m AND Thang=@t AND Nam=@n", DbHelper.P("@m", maNV), DbHelper.P("@t", thang), DbHelper.P("@n", nam));
                    if (cu.Rows.Count > 0)
                        DbHelper.Exec(tx, "UPDATE LuongThang SET LuongCoBan=@a, LuongTheoTour=@b, TongLuong=@c WHERE MaLuong=@d",
                            DbHelper.P("@a", coBan), DbHelper.P("@b", theoTour), DbHelper.P("@c", tong), DbHelper.P("@d", cu.Rows[0]["MaLuong"]));
                    else
                        DbHelper.Exec(tx, "INSERT INTO LuongThang (MaLuong,Thang,Nam,LuongCoBan,LuongTheoTour,TongLuong,MaNV) VALUES (@a,@b,@c,@d,@e,@f,@g)",
                            DbHelper.P("@a", DbHelper.NextId(tx, "LuongThang", "MaLuong", "LG")), DbHelper.P("@b", thang), DbHelper.P("@c", nam),
                            DbHelper.P("@d", coBan), DbHelper.P("@e", theoTour), DbHelper.P("@f", tong), DbHelper.P("@g", maNV));
                    tx.Commit();
                    return tong;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public string GuiKhaoSat(string maKH, string maChuyen, string maPhieuDoan, int diem, string gopY)
        {
            if (diem < 1 || diem > 5) throw new NghiepVuException("Điểm hài lòng từ 1 đến 5.");
            bool coChuyen = !string.IsNullOrEmpty(maChuyen), coDoan = !string.IsNullOrEmpty(maPhieuDoan);
            if (coChuyen == coDoan) throw new NghiepVuException("Chọn đúng một: chuyến đi hoặc phiếu đoàn.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    DateTime ngayVe;
                    if (coChuyen)
                    {
                        var r = DbHelper.Query(tx, "SELECT NgayVe FROM ChuyenDi WHERE MaChuyen=@m", DbHelper.P("@m", maChuyen));
                        if (r.Rows.Count == 0) throw new NghiepVuException("Chuyến đi không tồn tại.");
                        ngayVe = (DateTime)r.Rows[0]["NgayVe"];
                    }
                    else
                    {
                        var r = DbHelper.Query(tx, "SELECT p.NgayDi, t.SoNgay FROM PhieuDangKyDoan p JOIN Tour t ON t.MaTour=p.MaTour WHERE p.MaPhieuDoan=@m", DbHelper.P("@m", maPhieuDoan));
                        if (r.Rows.Count == 0) throw new NghiepVuException("Phiếu đoàn không tồn tại.");
                        ngayVe = ((DateTime)r.Rows[0]["NgayDi"]).AddDays(Convert.ToInt32(r.Rows[0]["SoNgay"]) - 1);
                    }
                    if (ngayVe.Date >= DateTime.Today) throw new NghiepVuException("Chỉ gửi khảo sát sau khi tour kết thúc.");
                    string ma = DbHelper.NextId(tx, "PhieuKhaoSat", "MaPKS", "KS");
                    DbHelper.Exec(tx, "INSERT INTO PhieuKhaoSat (MaPKS,NgayGui,DiemHaiLong,GopY,MaKH,MaChuyen,MaPhieuDoan) VALUES (@a,@b,@c,@d,@e,@f,@g)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", diem), DbHelper.P("@d", gopY),
                        DbHelper.P("@e", maKH), DbHelper.P("@f", coChuyen ? maChuyen : null), DbHelper.P("@g", coDoan ? maPhieuDoan : null));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public DataTable DsLuong()
        {
            return DbHelper.Query("SELECT l.MaNV, n.HoTen, l.Thang, l.Nam, l.LuongCoBan, l.LuongTheoTour, l.TongLuong FROM LuongThang l JOIN NhanVien n ON n.MaNV=l.MaNV ORDER BY l.Nam DESC, l.Thang DESC, l.MaNV");
        }

        public DataTable DsKhaoSat()
        {
            return DbHelper.Query("SELECT MaPKS, MaKH, MaChuyen, MaPhieuDoan, DiemHaiLong, GopY FROM PhieuKhaoSat ORDER BY MaPKS DESC");
        }
    }
}
