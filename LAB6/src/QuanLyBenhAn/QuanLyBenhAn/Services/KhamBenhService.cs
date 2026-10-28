using System;
using System.Collections.Generic;
using System.Data;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    public class DongThuoc
    {
        public string MaThuoc;
        public int SoLuong;
        public string LieuDung;
    }

    // Phiếu khám, xét nghiệm, toa thuốc
    public class KhamBenhService
    {
        public DataTable DsPhieuDangDieuTri()
        {
            return DbHelper.Query(
                "SELECT p.MaPhieuKham, p.MaHoSo, bn.HoTen, p.NgayGioKham, p.TrieuChung FROM PhieuKham p " +
                "JOIN HoSoBenhAn h ON h.MaHoSo = p.MaHoSo JOIN BenhNhan bn ON bn.MaBN = h.MaBN " +
                "WHERE p.TrangThai = N'Đang điều trị' ORDER BY p.NgayGioKham");
        }

        public DataTable DsXetNghiem(string maPhieu)
        {
            return DbHelper.Query(
                "SELECT x.MaXN, l.TenXN, x.NgayXN, x.KetQua FROM XetNghiem x JOIN LoaiXetNghiem l ON l.MaLoaiXN = x.MaLoaiXN " +
                "WHERE x.MaPhieuKham=@p ORDER BY x.MaXN", DbHelper.P("@p", maPhieu));
        }

        // Lập phiếu khám lần điều trị. Hồ sơ phải còn lệ phí của năm khám.
        public string LapPhieuKham(string maHoSo, string maBS, string maKhoa, string trieuChung, decimal thanNhiet, string huyetAp)
        {
            if (string.IsNullOrWhiteSpace(trieuChung)) throw new NghiepVuException("Nhập triệu chứng.");
            if (thanNhiet < 30 || thanNhiet > 43) throw new NghiepVuException("Thân nhiệt phải từ 30 đến 43 độ C.");
            KiemTraHoSo(maHoSo);
            KiemTraBacSiKhoa(maBS, maKhoa);
            return TaoPhieu(maHoSo, maBS, maKhoa, trieuChung, thanNhiet, huyetAp, "Điều trị");
        }

        public void ChiDinhXetNghiem(string maPhieu, string maLoaiXN)
        {
            KiemTraDangDieuTri(maPhieu);
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    DbHelper.Exec(tx, "INSERT INTO XetNghiem (MaXN,NgayXN,KetQua,MaPhieuKham,MaLoaiXN) VALUES (@a,@b,NULL,@c,@d)",
                        DbHelper.P("@a", DbHelper.NextId(tx, "XetNghiem", "MaXN", "XN")), DbHelper.P("@b", DateTime.Today),
                        DbHelper.P("@c", maPhieu), DbHelper.P("@d", maLoaiXN));
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public void GhiKetQuaXetNghiem(string maXN, string ketQua)
        {
            if (string.IsNullOrWhiteSpace(ketQua)) throw new NghiepVuException("Nhập kết quả xét nghiệm.");
            if (DbHelper.Exec("UPDATE XetNghiem SET KetQua=@k WHERE MaXN=@m", DbHelper.P("@k", ketQua), DbHelper.P("@m", maXN)) == 0)
                throw new NghiepVuException("Không tìm thấy xét nghiệm.");
        }

        // Toa thuốc và các dòng thuốc lưu trong cùng một transaction
        public string LapToaThuoc(string maPhieu, string loiDan, List<DongThuoc> ds)
        {
            KiemTraDangDieuTri(maPhieu);
            if (DbHelper.Query("SELECT 1 FROM ToaThuoc WHERE MaPhieuKham=@p", DbHelper.P("@p", maPhieu)).Rows.Count > 0)
                throw new NghiepVuException("Phiếu khám này đã có toa thuốc.");
            if (ds == null || ds.Count == 0) throw new NghiepVuException("Toa thuốc phải có ít nhất một thuốc.");
            var da = new HashSet<string>();
            foreach (var d in ds)
            {
                if (d.SoLuong <= 0) throw new NghiepVuException("Số lượng thuốc phải lớn hơn 0.");
                if (string.IsNullOrWhiteSpace(d.LieuDung)) throw new NghiepVuException("Nhập liều dùng cho từng thuốc.");
                if (!da.Add(d.MaThuoc)) throw new NghiepVuException("Một thuốc chỉ xuất hiện một lần trong toa.");
            }
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string toa = DbHelper.NextId(tx, "ToaThuoc", "MaToa", "TO");
                    DbHelper.Exec(tx, "INSERT INTO ToaThuoc (MaToa,NgayLap,LoiDan,MaPhieuKham) VALUES (@a,@b,@c,@d)",
                        DbHelper.P("@a", toa), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", loiDan), DbHelper.P("@d", maPhieu));
                    foreach (var d in ds)
                        DbHelper.Exec(tx, "INSERT INTO ChiTietToaThuoc (MaCTT,SoLuong,LieuDung,MaToa,MaThuoc) VALUES (@a,@b,@c,@d,@e)",
                            DbHelper.P("@a", DbHelper.NextId(tx, "ChiTietToaThuoc", "MaCTT", "CT")), DbHelper.P("@b", d.SoLuong),
                            DbHelper.P("@c", d.LieuDung), DbHelper.P("@d", toa), DbHelper.P("@e", d.MaThuoc));
                    tx.Commit();
                    return toa;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public DataTable ChiTietToa(string maPhieu)
        {
            return DbHelper.Query(
                "SELECT t.MaToa, th.TenThuoc, c.SoLuong, th.DonViTinh, c.LieuDung FROM ToaThuoc t " +
                "JOIN ChiTietToaThuoc c ON c.MaToa = t.MaToa JOIN Thuoc th ON th.MaThuoc = c.MaThuoc " +
                "WHERE t.MaPhieuKham=@p", DbHelper.P("@p", maPhieu));
        }

        // ---- dùng chung ----
        internal static string TaoPhieu(string maHoSo, string maBS, string maKhoa, string trieuChung, decimal thanNhiet, string huyetAp, string loai)
        {
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string ma = DbHelper.NextId(tx, "PhieuKham", "MaPhieuKham", "PK");
                    DbHelper.Exec(tx,
                        "INSERT INTO PhieuKham (MaPhieuKham,NgayGioKham,TrieuChung,ThanNhiet,HuyetAp,LoaiKham,TrangThai,MaHoSo,MaBS,MaKhoa) " +
                        "VALUES (@a,@b,@c,@d,@e,@f,N'Đang điều trị',@g,@h,@i)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", DateTime.Now), DbHelper.P("@c", trieuChung), DbHelper.P("@d", thanNhiet),
                        DbHelper.P("@e", huyetAp), DbHelper.P("@f", loai), DbHelper.P("@g", maHoSo), DbHelper.P("@h", maBS), DbHelper.P("@i", maKhoa));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        internal static void KiemTraHoSo(string maHoSo)
        {
            var dt = DbHelper.Query("SELECT MaBN FROM HoSoBenhAn WHERE MaHoSo=@h", DbHelper.P("@h", maHoSo));
            if (dt.Rows.Count == 0) throw new NghiepVuException("Không tìm thấy hồ sơ bệnh án.");
            if (!HoSoService.ConHan(dt.Rows[0]["MaBN"].ToString(), DateTime.Today.Year))
                throw new NghiepVuException("Bệnh nhân chưa đóng lệ phí năm " + DateTime.Today.Year + ".");
        }

        internal static void KiemTraBacSiKhoa(string maBS, string maKhoa)
        {
            if (DbHelper.Query("SELECT 1 FROM BacSi WHERE MaBS=@b AND MaKhoa=@k", DbHelper.P("@b", maBS), DbHelper.P("@k", maKhoa)).Rows.Count == 0)
                throw new NghiepVuException("Bác sĩ không thuộc khoa đã chọn.");
        }

        internal static void KiemTraDangDieuTri(string maPhieu)
        {
            var dt = DbHelper.Query("SELECT TrangThai FROM PhieuKham WHERE MaPhieuKham=@p", DbHelper.P("@p", maPhieu));
            if (dt.Rows.Count == 0) throw new NghiepVuException("Không tìm thấy phiếu khám.");
            if (dt.Rows[0]["TrangThai"].ToString() != "Đang điều trị")
                throw new NghiepVuException("Phiếu khám đã hoàn tất, không thể thay đổi.");
        }
    }
}
