using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CuaHangNuocGiaiKhat.Data;

namespace CuaHangNuocGiaiKhat.Services
{
    /// <summary>Bán hàng theo tồn kho; thiếu hàng thì lập phiếu hẹn giao (có thể giao nhiều đợt).</summary>
    public class BanHangService
    {
        public KetQuaBanHang LapHoaDon(string maKH, string maNV, List<DongHang> dong, DateTime? ngayHen)
        {
            if (string.IsNullOrEmpty(maKH)) throw new NghiepVuException("Chưa chọn khách hàng.");
            if (dong == null || dong.Count == 0) throw new NghiepVuException("Hóa đơn phải có ít nhất 1 mặt hàng.");
            if (dong.Any(d => d.SoLuong <= 0)) throw new NghiepVuException("Số lượng mua phải > 0.");
            if (dong.GroupBy(d => d.MaNGK).Any(g => g.Count() > 1)) throw new NghiepVuException("Mỗi nước giải khát chỉ xuất hiện 1 dòng trong hóa đơn.");

            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string maHD = DbHelper.NextId(tx, "HoaDon", "MaHD", "HD", 4);
                    var thieu = new List<DongHang>();
                    decimal tong = 0;
                    var lines = new List<object[]>();

                    foreach (var d in dong)
                    {
                        var r = DbHelper.Query(tx, "SELECT DonGiaBan, SoLuongTon FROM NuocGiaiKhat WITH (UPDLOCK) WHERE MaNGK=@m", DbHelper.P("@m", d.MaNGK));
                        if (r.Rows.Count == 0) throw new NghiepVuException("Nước giải khát " + d.MaNGK + " không tồn tại.");
                        decimal gia = Convert.ToDecimal(r.Rows[0]["DonGiaBan"]);
                        int ton = Convert.ToInt32(r.Rows[0]["SoLuongTon"]);
                        int giao = Math.Min(ton, d.SoLuong);
                        if (giao < d.SoLuong) thieu.Add(new DongHang { MaNGK = d.MaNGK, SoLuong = d.SoLuong - giao });
                        tong += gia * d.SoLuong;
                        lines.Add(new object[] { d.MaNGK, d.SoLuong, gia, giao });
                    }

                    if (thieu.Count > 0)
                    {
                        if (ngayHen == null) throw new NghiepVuException("Không đủ tồn kho cho " + thieu.Count + " mặt hàng, cần chọn ngày hẹn giao.");
                        if (ngayHen.Value.Date < DateTime.Today) throw new NghiepVuException("Ngày hẹn giao phải từ hôm nay trở đi.");
                    }

                    DbHelper.Exec(tx, "INSERT INTO HoaDon (MaHD,MaKH,MaNV,NgayLap,TongTien,TrangThaiGiao) VALUES (@a,@b,@c,@d,@e,@f)",
                        DbHelper.P("@a", maHD), DbHelper.P("@b", maKH), DbHelper.P("@c", maNV), DbHelper.P("@d", DateTime.Today),
                        DbHelper.P("@e", tong), DbHelper.P("@f", thieu.Count > 0 ? "Còn thiếu hàng" : "Đã giao đủ"));

                    foreach (var l in lines)
                    {
                        string ct = DbHelper.NextId(tx, "ChiTietHoaDon", "MaCTHD", "CTH", 4);
                        DbHelper.Exec(tx, "INSERT INTO ChiTietHoaDon (MaCTHD,MaHD,MaNGK,SoLuong,DonGia,SoLuongDaGiao) VALUES (@a,@b,@c,@d,@e,@f)",
                            DbHelper.P("@a", ct), DbHelper.P("@b", maHD), DbHelper.P("@c", l[0]), DbHelper.P("@d", l[1]), DbHelper.P("@e", l[2]), DbHelper.P("@f", l[3]));
                        DbHelper.Exec(tx, "UPDATE NuocGiaiKhat SET SoLuongTon = SoLuongTon - @s WHERE MaNGK=@m", DbHelper.P("@s", l[3]), DbHelper.P("@m", l[0]));
                    }

                    string maHen = null;
                    if (thieu.Count > 0) maHen = TaoPhieuHen(tx, maHD, ngayHen.Value.Date, thieu);
                    tx.Commit();
                    return new KetQuaBanHang { MaHD = maHD, TongTien = tong, MaPhieuHen = maHen };
                }
                catch { tx.Rollback(); throw; }
            }
        }

        private string TaoPhieuHen(System.Data.SqlClient.SqlTransaction tx, string maHD, DateTime ngayHen, List<DongHang> thieu)
        {
            string ma = DbHelper.NextId(tx, "PhieuHenGiao", "MaPhieuHen", "PH", 4);
            DbHelper.Exec(tx, "INSERT INTO PhieuHenGiao (MaPhieuHen,MaHD,NgayLap,NgayHenGiao) VALUES (@a,@b,@c,@d)",
                DbHelper.P("@a", ma), DbHelper.P("@b", maHD), DbHelper.P("@c", DateTime.Today), DbHelper.P("@d", ngayHen));
            foreach (var t in thieu)
            {
                string ct = DbHelper.NextId(tx, "ChiTietPhieuHen", "MaCTPH", "CPH", 4);
                DbHelper.Exec(tx, "INSERT INTO ChiTietPhieuHen (MaCTPH,MaPhieuHen,MaNGK,SoLuongHen) VALUES (@a,@b,@c,@d)",
                    DbHelper.P("@a", ct), DbHelper.P("@b", ma), DbHelper.P("@c", t.MaNGK), DbHelper.P("@d", t.SoLuong));
            }
            return ma;
        }

        public DataTable DsPhieuHenChoGiao()
        {
            return DbHelper.Query("SELECT p.MaPhieuHen, p.MaHD, k.HoTen, p.NgayHenGiao, p.TrangThai " +
                "FROM PhieuHenGiao p JOIN HoaDon h ON h.MaHD=p.MaHD JOIN KhachHang k ON k.MaKH=h.MaKH " +
                "WHERE p.TrangThai = N'Chờ giao' ORDER BY p.NgayHenGiao");
        }

        public DataTable ChiTietPhieuHen(string maPhieuHen)
        {
            return DbHelper.Query("SELECT c.MaNGK, n.TenNGK, c.SoLuongHen, n.SoLuongTon " +
                "FROM ChiTietPhieuHen c JOIN NuocGiaiKhat n ON n.MaNGK=c.MaNGK WHERE c.MaPhieuHen=@m", DbHelper.P("@m", maPhieuHen));
        }

        /// <summary>
        /// Giao hàng theo phiếu hẹn: giao tối đa theo tồn kho. Còn thiếu thì lập phiếu hẹn mới (đợt tiếp theo).
        /// Trả về mã phiếu hẹn mới hoặc null nếu hóa đơn đã giao đủ.
        /// </summary>
        public string GiaoHangHen(string maPhieuHen, DateTime? ngayHenMoi)
        {
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    var ph = DbHelper.Query(tx, "SELECT MaHD, TrangThai FROM PhieuHenGiao WITH (UPDLOCK) WHERE MaPhieuHen=@m", DbHelper.P("@m", maPhieuHen));
                    if (ph.Rows.Count == 0) throw new NghiepVuException("Phiếu hẹn không tồn tại.");
                    if ((string)ph.Rows[0]["TrangThai"] != "Chờ giao") throw new NghiepVuException("Phiếu hẹn này đã giao.");
                    string maHD = (string)ph.Rows[0]["MaHD"];

                    var ct = DbHelper.Query(tx, "SELECT c.MaNGK, c.SoLuongHen, n.SoLuongTon FROM ChiTietPhieuHen c " +
                        "JOIN NuocGiaiKhat n WITH (UPDLOCK) ON n.MaNGK=c.MaNGK WHERE c.MaPhieuHen=@m", DbHelper.P("@m", maPhieuHen));
                    var thieu = new List<DongHang>();
                    int tongGiao = 0;
                    foreach (DataRow r in ct.Rows)
                    {
                        string ngk = (string)r["MaNGK"];
                        int hen = Convert.ToInt32(r["SoLuongHen"]), ton = Convert.ToInt32(r["SoLuongTon"]);
                        int giao = Math.Min(hen, ton);
                        if (giao > 0)
                        {
                            DbHelper.Exec(tx, "UPDATE NuocGiaiKhat SET SoLuongTon = SoLuongTon - @s WHERE MaNGK=@m", DbHelper.P("@s", giao), DbHelper.P("@m", ngk));
                            DbHelper.Exec(tx, "UPDATE ChiTietHoaDon SET SoLuongDaGiao = SoLuongDaGiao + @s WHERE MaHD=@h AND MaNGK=@m",
                                DbHelper.P("@s", giao), DbHelper.P("@h", maHD), DbHelper.P("@m", ngk));
                            tongGiao += giao;
                        }
                        if (giao < hen) thieu.Add(new DongHang { MaNGK = ngk, SoLuong = hen - giao });
                    }
                    if (tongGiao == 0) throw new NghiepVuException("Tồn kho chưa có hàng để giao cho phiếu hẹn này.");

                    string maMoi = null;
                    if (thieu.Count > 0)
                    {
                        if (ngayHenMoi == null || ngayHenMoi.Value.Date < DateTime.Today)
                            throw new NghiepVuException("Vẫn còn thiếu hàng, cần chọn ngày hẹn giao mới (từ hôm nay trở đi).");
                        maMoi = TaoPhieuHen(tx, maHD, ngayHenMoi.Value.Date, thieu);
                    }
                    DbHelper.Exec(tx, "UPDATE PhieuHenGiao SET TrangThai=N'Đã giao' WHERE MaPhieuHen=@m", DbHelper.P("@m", maPhieuHen));
                    if (maMoi == null)
                        DbHelper.Exec(tx, "UPDATE HoaDon SET TrangThaiGiao=N'Đã giao đủ' WHERE MaHD=@h", DbHelper.P("@h", maHD));
                    tx.Commit();
                    return maMoi;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public DataTable DsHoaDon()
        {
            return DbHelper.Query("SELECT h.MaHD, k.HoTen, k.LoaiKhach, h.NgayLap, h.TongTien, h.TrangThaiGiao " +
                "FROM HoaDon h JOIN KhachHang k ON k.MaKH=h.MaKH ORDER BY h.MaHD DESC");
        }
    }
}
