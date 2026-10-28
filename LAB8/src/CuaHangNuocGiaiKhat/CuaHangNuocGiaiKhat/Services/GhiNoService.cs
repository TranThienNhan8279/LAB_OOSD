using System;
using System.Data;
using CuaHangNuocGiaiKhat.Data;

namespace CuaHangNuocGiaiKhat.Services
{
    /// <summary>Lập phiếu ghi nợ cho khách quen (khớp Sequence Diagram câu 6).</summary>
    public class GhiNoService
    {
        public string LapPhieuGhiNo(string maHD, string maNV, decimal soTienNo, DateTime ngayDuKienTra, string lyDo)
        {
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    var hd = DbHelper.Query(tx, "SELECT h.MaKH, h.TongTien, k.LoaiKhach FROM HoaDon h WITH (UPDLOCK) JOIN KhachHang k ON k.MaKH=h.MaKH WHERE h.MaHD=@m", DbHelper.P("@m", maHD));
                    if (hd.Rows.Count == 0) throw new NghiepVuException("Hóa đơn không tồn tại.");
                    if ((string)hd.Rows[0]["LoaiKhach"] != "QUEN") throw new NghiepVuException("Chỉ khách quen mới được ghi nợ.");
                    if (Convert.ToInt32(DbHelper.Scalar(tx, "SELECT COUNT(*) FROM PhieuGhiNo WHERE MaHD=@m", DbHelper.P("@m", maHD))) > 0)
                        throw new NghiepVuException("Hóa đơn này đã có phiếu ghi nợ.");
                    decimal tong = Convert.ToDecimal(hd.Rows[0]["TongTien"]);
                    if (soTienNo <= 0) throw new NghiepVuException("Số tiền nợ phải > 0.");
                    if (soTienNo > tong) throw new NghiepVuException("Số tiền nợ không được vượt tổng tiền hóa đơn (" + tong.ToString("N0") + ").");
                    if (ngayDuKienTra.Date < DateTime.Today) throw new NghiepVuException("Ngày dự kiến trả phải từ hôm nay trở đi.");

                    string ma = DbHelper.NextId(tx, "PhieuGhiNo", "MaPhieuNo", "PN", 4);
                    DbHelper.Exec(tx, "INSERT INTO PhieuGhiNo (MaPhieuNo,MaKH,MaNV,MaHD,NgayLap,SoTienNo,NgayDuKienTra,LyDoNo) VALUES (@a,@b,@c,@d,@e,@f,@g,@h)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", hd.Rows[0]["MaKH"]), DbHelper.P("@c", maNV), DbHelper.P("@d", maHD),
                        DbHelper.P("@e", DateTime.Today), DbHelper.P("@f", soTienNo), DbHelper.P("@g", ngayDuKienTra.Date), DbHelper.P("@h", lyDo));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public decimal TongNo(string maKH)
        {
            var o = DbHelper.Query("SELECT ISNULL(SUM(SoTienNo),0) AS T FROM PhieuGhiNo WHERE MaKH=@m", DbHelper.P("@m", maKH));
            return Convert.ToDecimal(o.Rows[0]["T"]);
        }

        public DataTable DsPhieuGhiNo()
        {
            return DbHelper.Query("SELECT p.MaPhieuNo, p.MaHD, k.HoTen, p.NgayLap, p.SoTienNo, p.NgayDuKienTra, p.LyDoNo " +
                "FROM PhieuGhiNo p JOIN KhachHang k ON k.MaKH=p.MaKH ORDER BY p.MaPhieuNo DESC");
        }
    }
}
