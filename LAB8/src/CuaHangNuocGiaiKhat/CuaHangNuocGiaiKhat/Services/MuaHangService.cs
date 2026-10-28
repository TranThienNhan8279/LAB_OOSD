using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CuaHangNuocGiaiKhat.Data;

namespace CuaHangNuocGiaiKhat.Services
{
    /// <summary>Đặt hàng nhà cung ứng và nhận hàng theo phiếu giao (tối đa 3 đợt).</summary>
    public class MuaHangService
    {
        public const int ToiDaDotGiao = 3;

        public string TaoDonDatHang(string maNCC, string maNV, List<DongHang> dong)
        {
            if (dong == null || dong.Count == 0) throw new NghiepVuException("Đơn đặt hàng phải có ít nhất 1 mặt hàng.");
            if (dong.Any(d => d.SoLuong <= 0)) throw new NghiepVuException("Số lượng đặt phải > 0.");
            if (dong.GroupBy(d => d.MaNGK).Any(g => g.Count() > 1)) throw new NghiepVuException("Mỗi nước giải khát chỉ xuất hiện 1 dòng trong đơn.");

            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string ma = DbHelper.NextId(tx, "DonDatHang", "MaDDH", "DDH", 4);
                    DbHelper.Exec(tx, "INSERT INTO DonDatHang (MaDDH,MaNCC,MaNV,NgayDat) VALUES (@a,@b,@c,@d)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", maNCC), DbHelper.P("@c", maNV), DbHelper.P("@d", DateTime.Today));
                    foreach (var d in dong)
                    {
                        string ct = DbHelper.NextId(tx, "ChiTietDonDatHang", "MaCTDDH", "CTD", 4);
                        DbHelper.Exec(tx, "INSERT INTO ChiTietDonDatHang VALUES (@a,@b,@c,@d)",
                            DbHelper.P("@a", ct), DbHelper.P("@b", ma), DbHelper.P("@c", d.MaNGK), DbHelper.P("@d", d.SoLuong));
                    }
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        /// <summary>Phần còn phải giao của từng mặt hàng trong đơn.</summary>
        public DataTable ConPhaiGiao(string maDDH)
        {
            return DbHelper.Query(
                "SELECT ct.MaNGK, n.TenNGK, ct.SoLuongDat, " +
                "ISNULL((SELECT SUM(g.SoLuongGiao) FROM ChiTietPhieuGiao g JOIN PhieuGiaoHang p ON p.MaPGH=g.MaPGH " +
                "        WHERE p.MaDDH=ct.MaDDH AND g.MaNGK=ct.MaNGK),0) AS DaGiao " +
                "FROM ChiTietDonDatHang ct JOIN NuocGiaiKhat n ON n.MaNGK=ct.MaNGK WHERE ct.MaDDH=@m",
                DbHelper.P("@m", maDDH));
        }

        public DataTable DsDonDatHang()
        {
            return DbHelper.Query("SELECT d.MaDDH, n.TenNCC, d.NgayDat, d.TrangThai, " +
                "(SELECT COUNT(*) FROM PhieuGiaoHang p WHERE p.MaDDH=d.MaDDH) AS SoDotDaGiao " +
                "FROM DonDatHang d JOIN NhaCungUng n ON n.MaNCC=d.MaNCC ORDER BY d.MaDDH DESC");
        }

        public string LapPhieuGiao(string maDDH, DateTime ngayGiao, List<DongHang> dong)
        {
            if (dong == null || dong.Count == 0) throw new NghiepVuException("Phiếu giao phải có ít nhất 1 mặt hàng.");
            if (dong.Any(d => d.SoLuong <= 0 || d.DonGia < 0)) throw new NghiepVuException("Số lượng giao phải > 0 và đơn giá nhập >= 0.");

            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    var ddh = DbHelper.Query(tx, "SELECT TrangThai FROM DonDatHang WITH (UPDLOCK) WHERE MaDDH=@m", DbHelper.P("@m", maDDH));
                    if (ddh.Rows.Count == 0) throw new NghiepVuException("Đơn đặt hàng không tồn tại.");
                    if ((string)ddh.Rows[0]["TrangThai"] == "Đã giao đủ") throw new NghiepVuException("Đơn đặt hàng đã giao đủ.");

                    int soDot = Convert.ToInt32(DbHelper.Scalar(tx, "SELECT COUNT(*) FROM PhieuGiaoHang WHERE MaDDH=@m", DbHelper.P("@m", maDDH)));
                    if (soDot >= ToiDaDotGiao) throw new NghiepVuException("Đơn đặt hàng đã giao đủ tối đa " + ToiDaDotGiao + " đợt.");
                    int dot = soDot + 1;

                    var conLai = DbHelper.Query(tx,
                        "SELECT ct.MaNGK, ct.SoLuongDat - ISNULL((SELECT SUM(g.SoLuongGiao) FROM ChiTietPhieuGiao g " +
                        "JOIN PhieuGiaoHang p ON p.MaPGH=g.MaPGH WHERE p.MaDDH=ct.MaDDH AND g.MaNGK=ct.MaNGK),0) AS ConLai " +
                        "FROM ChiTietDonDatHang ct WHERE ct.MaDDH=@m", DbHelper.P("@m", maDDH));
                    var map = conLai.Rows.Cast<DataRow>().ToDictionary(r => (string)r["MaNGK"], r => Convert.ToInt32(r["ConLai"]));

                    foreach (var d in dong)
                    {
                        if (!map.ContainsKey(d.MaNGK)) throw new NghiepVuException("Mặt hàng " + d.MaNGK + " không có trong đơn đặt hàng.");
                        if (d.SoLuong > map[d.MaNGK]) throw new NghiepVuException("Mặt hàng " + d.MaNGK + ": giao " + d.SoLuong + " vượt số còn lại " + map[d.MaNGK] + ".");
                        map[d.MaNGK] -= d.SoLuong;
                    }
                    bool xongHet = map.Values.All(v => v == 0);
                    if (dot == ToiDaDotGiao && !xongHet)
                        throw new NghiepVuException("Đợt giao thứ 3 là đợt cuối, phải giao đủ phần còn lại.");

                    string maPGH = DbHelper.NextId(tx, "PhieuGiaoHang", "MaPGH", "PGH", 4);
                    decimal tong = dong.Sum(d => d.SoLuong * d.DonGia);
                    DbHelper.Exec(tx, "INSERT INTO PhieuGiaoHang (MaPGH,MaDDH,DotGiao,NgayGiao,TongTien) VALUES (@a,@b,@c,@d,@e)",
                        DbHelper.P("@a", maPGH), DbHelper.P("@b", maDDH), DbHelper.P("@c", dot), DbHelper.P("@d", ngayGiao), DbHelper.P("@e", tong));
                    foreach (var d in dong)
                    {
                        string ct = DbHelper.NextId(tx, "ChiTietPhieuGiao", "MaCTPGH", "CTG", 4);
                        DbHelper.Exec(tx, "INSERT INTO ChiTietPhieuGiao (MaCTPGH,MaPGH,MaNGK,SoLuongGiao,DonGiaNhap) VALUES (@a,@b,@c,@d,@e)",
                            DbHelper.P("@a", ct), DbHelper.P("@b", maPGH), DbHelper.P("@c", d.MaNGK), DbHelper.P("@d", d.SoLuong), DbHelper.P("@e", d.DonGia));
                        DbHelper.Exec(tx, "UPDATE NuocGiaiKhat SET SoLuongTon = SoLuongTon + @s WHERE MaNGK=@m",
                            DbHelper.P("@s", d.SoLuong), DbHelper.P("@m", d.MaNGK));
                    }
                    DbHelper.Exec(tx, "UPDATE DonDatHang SET TrangThai=@t, NgayNhan=@n WHERE MaDDH=@m",
                        DbHelper.P("@t", xongHet ? "Đã giao đủ" : "Đang giao"),
                        DbHelper.P("@n", xongHet ? (object)ngayGiao : null), DbHelper.P("@m", maDDH));
                    tx.Commit();
                    return maPGH;
                }
                catch { tx.Rollback(); throw; }
            }
        }
    }
}
