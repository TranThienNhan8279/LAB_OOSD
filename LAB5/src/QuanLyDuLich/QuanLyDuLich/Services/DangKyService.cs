using System;
using System.Collections.Generic;
using System.Data;
using QuanLyDuLich.Data;

namespace QuanLyDuLich.Services
{
    public class NguoiDi
    {
        public string HoTen;
        public DateTime? NgaySinh;
        public string SoCMND;
    }

    // Đăng ký tour: khách lẻ theo chuyến (tối đa 12 người), khách đoàn (trên 12 người)
    public class DangKyService
    {
        public const int ToiDaKhachLe = 12;
        public const decimal TyLeCoc = 0.3m;   // giả định: đặt cọc 30% tổng tiền

        public string DangKyChuyen(string maKH, string maChuyen, int soNguoi, string diaDiemDon)
        {
            if (soNguoi < 1 || soNguoi > ToiDaKhachLe)
                throw new NghiepVuException("Khách lẻ đăng ký từ 1 đến " + ToiDaKhachLe + " người. Trên " + ToiDaKhachLe + " người phải đăng ký theo đoàn.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    var kh = DbHelper.Query(tx, "SELECT LoaiKhachHang FROM KhachHang WHERE MaKH=@m", DbHelper.P("@m", maKH));
                    if (kh.Rows.Count == 0) throw new NghiepVuException("Khách hàng không tồn tại.");
                    if ((string)kh.Rows[0]["LoaiKhachHang"] != "KHACHLE") throw new NghiepVuException("Đây là khách đoàn, không đăng ký theo chuyến.");
                    var cd = DbHelper.Query(tx, "SELECT c.NgayDi, t.DonGiaKhach FROM ChuyenDi c JOIN Tour t ON t.MaTour=c.MaTour WHERE c.MaChuyen=@m", DbHelper.P("@m", maChuyen));
                    if (cd.Rows.Count == 0) throw new NghiepVuException("Chuyến đi không tồn tại.");
                    if (((DateTime)cd.Rows[0]["NgayDi"]).Date < DateTime.Today) throw new NghiepVuException("Chuyến này đã khởi hành.");
                    decimal tien = soNguoi * Convert.ToDecimal(cd.Rows[0]["DonGiaKhach"]);

                    string ma = DbHelper.NextId(tx, "PhieuDangKyChuyen", "MaPhieuLe", "PL");
                    DbHelper.Exec(tx, "INSERT INTO PhieuDangKyChuyen (MaPhieuLe,NgayDangKy,SoNguoi,DiaDiemDon,SoTien,MaKH,MaChuyen) VALUES (@a,@b,@c,@d,@e,@f,@g)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", soNguoi), DbHelper.P("@d", diaDiemDon ?? ""),
                        DbHelper.P("@e", tien), DbHelper.P("@f", maKH), DbHelper.P("@g", maChuyen));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public string DangKyDoan(string maKH, string maTour, DateTime ngayDi, int soNguoi, string diaDiemDon, bool coBaoHiem, List<NguoiDi> dsNguoi)
        {
            if (soNguoi <= ToiDaKhachLe)
                throw new NghiepVuException("Khách đoàn phải trên " + ToiDaKhachLe + " người.");
            if (ngayDi.Date < DateTime.Today) throw new NghiepVuException("Ngày đi phải từ hôm nay trở đi.");
            if (coBaoHiem && (dsNguoi == null || dsNguoi.Count != soNguoi))
                throw new NghiepVuException("Có mua bảo hiểm thì phải kèm danh sách đủ " + soNguoi + " người đi cùng.");

            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    var kh = DbHelper.Query(tx, "SELECT LoaiKhachHang FROM KhachHang WHERE MaKH=@m", DbHelper.P("@m", maKH));
                    if (kh.Rows.Count == 0) throw new NghiepVuException("Khách hàng không tồn tại.");
                    if ((string)kh.Rows[0]["LoaiKhachHang"] != "KHACHDOAN") throw new NghiepVuException("Đây là khách lẻ, không đăng ký theo đoàn.");
                    var t = DbHelper.Query(tx, "SELECT DonGiaKhach FROM Tour WHERE MaTour=@m", DbHelper.P("@m", maTour));
                    if (t.Rows.Count == 0) throw new NghiepVuException("Tour không tồn tại.");
                    decimal tong = soNguoi * Convert.ToDecimal(t.Rows[0]["DonGiaKhach"]);
                    decimal coc = Math.Round(tong * TyLeCoc, 0);

                    string ma = DbHelper.NextId(tx, "PhieuDangKyDoan", "MaPhieuDoan", "PD");
                    DbHelper.Exec(tx, "INSERT INTO PhieuDangKyDoan (MaPhieuDoan,NgayDangKy,NgayDi,SoNguoi,DiaDiemDon,TienCoc,CoBaoHiem,TrangThai,MaKH,MaTour) VALUES (@a,@b,@c,@d,@e,@f,@g,@h,@i,@j)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", ngayDi.Date), DbHelper.P("@d", soNguoi),
                        DbHelper.P("@e", diaDiemDon ?? ""), DbHelper.P("@f", coc), DbHelper.P("@g", coBaoHiem), DbHelper.P("@h", "Đã đặt cọc"),
                        DbHelper.P("@i", maKH), DbHelper.P("@j", maTour));
                    if (coBaoHiem)
                    {
                        foreach (var n in dsNguoi)
                        {
                            string mn = DbHelper.NextId(tx, "NguoiDiCung", "MaNguoi", "NG");
                            DbHelper.Exec(tx, "INSERT INTO NguoiDiCung (MaNguoi,HoTen,NgaySinh,SoCMND,MaPhieuDoan) VALUES (@a,@b,@c,@d,@e)",
                                DbHelper.P("@a", mn), DbHelper.P("@b", n.HoTen), DbHelper.P("@c", n.NgaySinh), DbHelper.P("@d", n.SoCMND), DbHelper.P("@e", ma));
                        }
                    }
                    string mt = DbHelper.NextId(tx, "PhieuThanhToanDoan", "MaTT", "TT");
                    DbHelper.Exec(tx, "INSERT INTO PhieuThanhToanDoan (MaTT,NgayThanhToan,SoTien,Loai,MaPhieuDoan) VALUES (@a,@b,@c,@d,@e)",
                        DbHelper.P("@a", mt), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", coc), DbHelper.P("@d", "Đặt cọc"), DbHelper.P("@e", ma));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        // Quyết toán sau khi kết thúc tour: thu phần còn lại (tổng tiền trừ tiền đã trả)
        public decimal QuyetToanDoan(string maPhieuDoan)
        {
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    var r = DbHelper.Query(tx,
                        "SELECT p.NgayDi, p.SoNguoi, p.TrangThai, t.SoNgay, t.DonGiaKhach FROM PhieuDangKyDoan p WITH (UPDLOCK) " +
                        "JOIN Tour t ON t.MaTour = p.MaTour WHERE p.MaPhieuDoan=@m", DbHelper.P("@m", maPhieuDoan));
                    if (r.Rows.Count == 0) throw new NghiepVuException("Phiếu đăng ký đoàn không tồn tại.");
                    if ((string)r.Rows[0]["TrangThai"] == "Đã quyết toán") throw new NghiepVuException("Đoàn này đã quyết toán.");
                    DateTime ngayVe = ((DateTime)r.Rows[0]["NgayDi"]).AddDays(Convert.ToInt32(r.Rows[0]["SoNgay"]) - 1);
                    if (ngayVe.Date >= DateTime.Today) throw new NghiepVuException("Tour chưa kết thúc (ngày về dự kiến " + ngayVe.ToString("dd/MM/yyyy") + ").");
                    decimal tong = Convert.ToInt32(r.Rows[0]["SoNguoi"]) * Convert.ToDecimal(r.Rows[0]["DonGiaKhach"]);
                    decimal daTra = Convert.ToDecimal(DbHelper.Scalar(tx, "SELECT ISNULL(SUM(SoTien),0) FROM PhieuThanhToanDoan WHERE MaPhieuDoan=@m", DbHelper.P("@m", maPhieuDoan)));
                    decimal conLai = tong - daTra;
                    string mt = DbHelper.NextId(tx, "PhieuThanhToanDoan", "MaTT", "TT");
                    DbHelper.Exec(tx, "INSERT INTO PhieuThanhToanDoan (MaTT,NgayThanhToan,SoTien,Loai,MaPhieuDoan) VALUES (@a,@b,@c,@d,@e)",
                        DbHelper.P("@a", mt), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", conLai), DbHelper.P("@d", "Quyết toán"), DbHelper.P("@e", maPhieuDoan));
                    DbHelper.Exec(tx, "UPDATE PhieuDangKyDoan SET TrangThai=N'Đã quyết toán' WHERE MaPhieuDoan=@m", DbHelper.P("@m", maPhieuDoan));
                    tx.Commit();
                    return conLai;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public DataTable DsPhieuLe()
        {
            return DbHelper.Query("SELECT p.MaPhieuLe, p.MaKH, p.MaChuyen, p.SoNguoi, p.SoTien, p.NgayDangKy FROM PhieuDangKyChuyen p ORDER BY p.MaPhieuLe DESC");
        }

        public DataTable DsPhieuDoan()
        {
            return DbHelper.Query("SELECT MaPhieuDoan, MaKH, MaTour, NgayDi, SoNguoi, TienCoc, CoBaoHiem, TrangThai FROM PhieuDangKyDoan ORDER BY MaPhieuDoan DESC");
        }
    }
}
