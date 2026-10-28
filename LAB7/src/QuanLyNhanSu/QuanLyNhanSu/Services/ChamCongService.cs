using System;
using System.Collections.Generic;
using System.Data;
using QuanLyNhanSu.Data;

namespace QuanLyNhanSu.Services
{
    public class DongPhuTroi
    {
        public string MaNV;
        public decimal SoGio;
    }

    // Phân công tuần, chấm công, phiếu phụ trội
    public class ChamCongService
    {
        public const decimal HeSoPhuTroi = 1.5m;
        public const decimal PhuTroiToiDa = 4;   // giả định: tối đa 4 giờ làm thêm mỗi ngày

        public DataTable DsKeHoach()
        {
            return DbHelper.Query("SELECT k.MaKHT, p.TenPB, k.TuanBatDau FROM KeHoachTuan k JOIN PhongBan p ON p.MaPB = k.MaPB ORDER BY k.TuanBatDau DESC");
        }

        public DataTable DsPhanCong(string maKHT)
        {
            return DbHelper.Query("SELECT c.NgayLam, c.CaLam, n.HoTen, c.CongViec FROM ChiTietPhanCong c JOIN NhanVien n ON n.MaNV = c.MaNV WHERE c.MaKHT=@k ORDER BY c.NgayLam, c.CaLam", DbHelper.P("@k", maKHT));
        }

        public DataTable DsChamCong(string maNV)
        {
            return DbHelper.Query("SELECT Ngay, SoGioLam FROM PhieuChamCong WHERE MaNV=@n ORDER BY Ngay DESC", DbHelper.P("@n", maNV));
        }

        public string LapKeHoach(string maPB, DateTime tuanBatDau, string ghiChu)
        {
            if (tuanBatDau.DayOfWeek != DayOfWeek.Monday) throw new NghiepVuException("Tuần bắt đầu phải là ngày thứ Hai.");
            if (DbHelper.Query("SELECT 1 FROM KeHoachTuan WHERE MaPB=@p AND TuanBatDau=@t", DbHelper.P("@p", maPB), DbHelper.P("@t", tuanBatDau.Date)).Rows.Count > 0)
                throw new NghiepVuException("Phòng ban đã có kế hoạch cho tuần này.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string ma = DbHelper.NextId(tx, "KeHoachTuan", "MaKHT", "KH");
                    DbHelper.Exec(tx, "INSERT INTO KeHoachTuan (MaKHT,TuanBatDau,GhiChu,MaPB) VALUES (@a,@b,@c,@d)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", tuanBatDau.Date), DbHelper.P("@c", ghiChu), DbHelper.P("@d", maPB));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        // Nhân viên phải thuộc phòng ban lập kế hoạch, ngày nằm trong tuần
        public void PhanCong(string maKHT, string maNV, DateTime ngay, string ca, string congViec)
        {
            var kh = DbHelper.Query("SELECT TuanBatDau, MaPB FROM KeHoachTuan WHERE MaKHT=@k", DbHelper.P("@k", maKHT));
            if (kh.Rows.Count == 0) throw new NghiepVuException("Không tìm thấy kế hoạch tuần.");
            DateTime dau = Convert.ToDateTime(kh.Rows[0]["TuanBatDau"]);
            if (ngay.Date < dau || ngay.Date > dau.AddDays(6)) throw new NghiepVuException("Ngày làm phải nằm trong tuần của kế hoạch.");
            if (DbHelper.Query("SELECT 1 FROM NhanVien WHERE MaNV=@n AND MaPB=@p AND TrangThai <> N'Đã nghỉ'",
                DbHelper.P("@n", maNV), DbHelper.P("@p", kh.Rows[0]["MaPB"].ToString())).Rows.Count == 0)
                throw new NghiepVuException("Nhân viên không thuộc phòng ban của kế hoạch.");
            if (string.IsNullOrWhiteSpace(congViec)) throw new NghiepVuException("Nhập nội dung công việc.");
            if (DbHelper.Query("SELECT 1 FROM ChiTietPhanCong WHERE MaNV=@n AND NgayLam=@d AND CaLam=@c",
                DbHelper.P("@n", maNV), DbHelper.P("@d", ngay.Date), DbHelper.P("@c", ca)).Rows.Count > 0)
                throw new NghiepVuException("Nhân viên đã được phân công ca này.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    DbHelper.Exec(tx, "INSERT INTO ChiTietPhanCong (MaCTPC,NgayLam,CaLam,CongViec,MaKHT,MaNV) VALUES (@a,@b,@c,@d,@e,@f)",
                        DbHelper.P("@a", DbHelper.NextId(tx, "ChiTietPhanCong", "MaCTPC", "CP")), DbHelper.P("@b", ngay.Date), DbHelper.P("@c", ca),
                        DbHelper.P("@d", congViec), DbHelper.P("@e", maKHT), DbHelper.P("@f", maNV));
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public void ChamCong(string maNV, DateTime ngay, decimal soGio)
        {
            if (ngay.Date > DateTime.Today) throw new NghiepVuException("Không chấm công cho ngày chưa tới.");
            if (soGio < 0 || soGio > 24) throw new NghiepVuException("Số giờ làm từ 0 đến 24.");
            if (DbHelper.Query("SELECT 1 FROM NhanVien WHERE MaNV=@n AND TrangThai <> N'Đã nghỉ'", DbHelper.P("@n", maNV)).Rows.Count == 0)
                throw new NghiepVuException("Nhân viên không còn làm việc.");
            if (DbHelper.Query("SELECT 1 FROM PhieuChamCong WHERE MaNV=@n AND Ngay=@d", DbHelper.P("@n", maNV), DbHelper.P("@d", ngay.Date)).Rows.Count > 0)
                throw new NghiepVuException("Ngày này đã chấm công.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    DbHelper.Exec(tx, "INSERT INTO PhieuChamCong (MaPCC,Ngay,SoGioLam,MaNV) VALUES (@a,@b,@c,@d)",
                        DbHelper.P("@a", DbHelper.NextId(tx, "PhieuChamCong", "MaPCC", "CC")), DbHelper.P("@b", ngay.Date), DbHelper.P("@c", soGio), DbHelper.P("@d", maNV));
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
        }

        // Phiếu phụ trội của một phòng ban trong một ngày, gồm danh sách nhân viên và số giờ làm thêm
        public string LapPhieuPhuTroi(string maPB, DateTime ngay, List<DongPhuTroi> ds)
        {
            if (ds == null || ds.Count == 0) throw new NghiepVuException("Phiếu phụ trội phải có ít nhất một nhân viên.");
            if (ngay.Date > DateTime.Today) throw new NghiepVuException("Không lập phiếu cho ngày chưa tới.");
            var da = new HashSet<string>();
            foreach (var d in ds)
            {
                if (d.SoGio <= 0 || d.SoGio > PhuTroiToiDa) throw new NghiepVuException("Số giờ làm thêm từ 0 đến " + PhuTroiToiDa + " giờ mỗi ngày.");
                if (!da.Add(d.MaNV)) throw new NghiepVuException("Một nhân viên chỉ xuất hiện một lần trong phiếu.");
                if (DbHelper.Query("SELECT 1 FROM NhanVien WHERE MaNV=@n AND MaPB=@p AND TrangThai <> N'Đã nghỉ'", DbHelper.P("@n", d.MaNV), DbHelper.P("@p", maPB)).Rows.Count == 0)
                    throw new NghiepVuException("Nhân viên " + d.MaNV + " không thuộc phòng ban này.");
            }
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string ma = DbHelper.NextId(tx, "PhieuPhuTroi", "MaPPT", "PP");
                    DbHelper.Exec(tx, "INSERT INTO PhieuPhuTroi (MaPPT,Ngay,HeSo,MaPB) VALUES (@a,@b,@c,@d)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", ngay.Date), DbHelper.P("@c", HeSoPhuTroi), DbHelper.P("@d", maPB));
                    foreach (var d in ds)
                        DbHelper.Exec(tx, "INSERT INTO ChiTietPhuTroi (MaCTPT,SoGio,MaPPT,MaNV) VALUES (@a,@b,@c,@d)",
                            DbHelper.P("@a", DbHelper.NextId(tx, "ChiTietPhuTroi", "MaCTPT", "CD")), DbHelper.P("@b", d.SoGio), DbHelper.P("@c", ma), DbHelper.P("@d", d.MaNV));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }
    }
}
