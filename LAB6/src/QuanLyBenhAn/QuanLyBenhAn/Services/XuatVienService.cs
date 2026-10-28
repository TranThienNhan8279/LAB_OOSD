using System;
using System.Data;
using QuanLyBenhAn.Data;

namespace QuanLyBenhAn.Services
{
    // Thanh toán viện phí, xuất viện, ghi nhận vào hồ sơ, khám định kỳ theo quý
    public class XuatVienService
    {
        public DataTable DsChoGhiNhan()
        {
            return DbHelper.Query(
                "SELECT p.MaPhieuKham, p.MaHoSo, p.NgayGioKham, t.NgayXuatVien, t.SoTien FROM PhieuKham p " +
                "JOIN ThanhToanVienPhi t ON t.MaPhieuKham = p.MaPhieuKham " +
                "WHERE p.TrangThai = N'Hoàn tất' AND p.MaNVGhiNhan IS NULL ORDER BY t.NgayXuatVien");
        }

        // Thanh toán xong thì phiếu chuyển sang Hoàn tất (đã xuất viện)
        public string ThanhToanXuatVien(string maPhieu, decimal soTien)
        {
            KhamBenhService.KiemTraDangDieuTri(maPhieu);
            if (soTien < 0) throw new NghiepVuException("Số tiền không được âm.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string ma = DbHelper.NextId(tx, "ThanhToanVienPhi", "MaTT", "TT");
                    DbHelper.Exec(tx, "INSERT INTO ThanhToanVienPhi (MaTT,NgayThanhToan,SoTien,NgayXuatVien,MaPhieuKham) VALUES (@a,@b,@c,@d,@e)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", soTien), DbHelper.P("@d", DateTime.Today), DbHelper.P("@e", maPhieu));
                    DbHelper.Exec(tx, "UPDATE PhieuKham SET TrangThai = N'Hoàn tất' WHERE MaPhieuKham=@p", DbHelper.P("@p", maPhieu));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        // Chỉ ghi nhận sau khi đã xuất viện và thanh toán
        public void GhiNhanVaoHoSo(string maPhieu, string maNV)
        {
            var dt = DbHelper.Query(
                "SELECT p.TrangThai, p.MaNVGhiNhan, (SELECT COUNT(*) FROM ThanhToanVienPhi t WHERE t.MaPhieuKham = p.MaPhieuKham) AS DaTT " +
                "FROM PhieuKham p WHERE p.MaPhieuKham=@p", DbHelper.P("@p", maPhieu));
            if (dt.Rows.Count == 0) throw new NghiepVuException("Không tìm thấy phiếu khám.");
            var r = dt.Rows[0];
            if (r["TrangThai"].ToString() != "Hoàn tất" || Convert.ToInt32(r["DaTT"]) == 0)
                throw new NghiepVuException("Chỉ ghi nhận sau khi bệnh nhân đã xuất viện và thanh toán viện phí.");
            if (r["MaNVGhiNhan"] != DBNull.Value) throw new NghiepVuException("Phiếu khám đã được ghi nhận.");
            DbHelper.Exec("UPDATE PhieuKham SET MaNVGhiNhan=@n WHERE MaPhieuKham=@p", DbHelper.P("@n", maNV), DbHelper.P("@p", maPhieu));
        }

        // ---- khám định kỳ: miễn phí, mỗi quý một lần ----
        public DataTable DsLich(string maHoSo)
        {
            return DbHelper.Query("SELECT MaLich, Quy, Nam, NgayHen, TrangThai FROM LichKhamDinhKy WHERE MaHoSo=@h ORDER BY Nam, Quy", DbHelper.P("@h", maHoSo));
        }

        public string DatLich(string maHoSo, int quy, DateTime ngayHen)
        {
            KhamBenhService.KiemTraHoSo(maHoSo);
            if (quy < 1 || quy > 4) throw new NghiepVuException("Quý phải từ 1 đến 4.");
            if (ngayHen <= DateTime.Now) throw new NghiepVuException("Ngày giờ hẹn phải sau thời điểm hiện tại.");
            int quyHen = (ngayHen.Month - 1) / 3 + 1;
            if (quyHen != quy) throw new NghiepVuException("Ngày hẹn không nằm trong quý " + quy + ".");
            if (DbHelper.Query("SELECT 1 FROM LichKhamDinhKy WHERE MaHoSo=@h AND Quy=@q AND Nam=@n",
                DbHelper.P("@h", maHoSo), DbHelper.P("@q", quy), DbHelper.P("@n", ngayHen.Year)).Rows.Count > 0)
                throw new NghiepVuException("Quý này đã có lịch khám định kỳ.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string ma = DbHelper.NextId(tx, "LichKhamDinhKy", "MaLich", "LK");
                    DbHelper.Exec(tx, "INSERT INTO LichKhamDinhKy (MaLich,Quy,Nam,NgayHen,TrangThai,MaHoSo) VALUES (@a,@b,@c,@d,N'Đã hẹn',@e)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", quy), DbHelper.P("@c", ngayHen.Year), DbHelper.P("@d", ngayHen), DbHelper.P("@e", maHoSo));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        // Khám theo lịch hẹn: tạo phiếu khám loại Định kỳ, không tính phí
        public string KhamDinhKy(string maLich, string maBS, string maKhoa, string nhanXet, decimal thanNhiet, string huyetAp)
        {
            var dt = DbHelper.Query("SELECT MaHoSo, TrangThai FROM LichKhamDinhKy WHERE MaLich=@l", DbHelper.P("@l", maLich));
            if (dt.Rows.Count == 0) throw new NghiepVuException("Không tìm thấy lịch hẹn.");
            if (dt.Rows[0]["TrangThai"].ToString() != "Đã hẹn") throw new NghiepVuException("Lịch này đã khám.");
            string hs = dt.Rows[0]["MaHoSo"].ToString();
            if (thanNhiet < 30 || thanNhiet > 43) throw new NghiepVuException("Thân nhiệt phải từ 30 đến 43 độ C.");
            KhamBenhService.KiemTraHoSo(hs);
            KhamBenhService.KiemTraBacSiKhoa(maBS, maKhoa);
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string ma = DbHelper.NextId(tx, "PhieuKham", "MaPhieuKham", "PK");
                    DbHelper.Exec(tx,
                        "INSERT INTO PhieuKham (MaPhieuKham,NgayGioKham,TrieuChung,ThanNhiet,HuyetAp,LoaiKham,TrangThai,MaHoSo,MaBS,MaKhoa) " +
                        "VALUES (@a,@b,@c,@d,@e,N'Định kỳ',N'Hoàn tất',@g,@h,@i)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", DateTime.Now), DbHelper.P("@c", string.IsNullOrWhiteSpace(nhanXet) ? "Khám định kỳ, không có bệnh" : nhanXet),
                        DbHelper.P("@d", thanNhiet), DbHelper.P("@e", huyetAp), DbHelper.P("@g", hs), DbHelper.P("@h", maBS), DbHelper.P("@i", maKhoa));
                    DbHelper.Exec(tx, "UPDATE LichKhamDinhKy SET TrangThai = N'Đã khám' WHERE MaLich=@l", DbHelper.P("@l", maLich));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }
    }
}
