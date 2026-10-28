using System;
using System.Data;
using QuanLyNhanSu.Data;

namespace QuanLyNhanSu.Services
{
    // Khen thưởng (kèm phiếu chi) và kỷ luật (phạt tiền kèm phiếu thu)
    public class ThuongPhatService
    {
        public DataTable DsKhenThuong()
        {
            return DbHelper.Query(
                "SELECT k.MaKT, k.NgayQD, k.LoaiKT, ISNULL(n.HoTen, p.TenPB) AS DoiTuong, k.NoiDung, k.SoTien FROM QuyetDinhKhenThuong k " +
                "LEFT JOIN NhanVien n ON n.MaNV = k.MaNV LEFT JOIN PhongBan p ON p.MaPB = k.MaPB ORDER BY k.MaKT DESC");
        }

        public DataTable DsKyLuat()
        {
            return DbHelper.Query("SELECT k.MaKL, k.NgayQD, n.HoTen, k.HinhThuc, k.LyDo, k.SoTienPhat FROM QuyetDinhKyLuat k JOIN NhanVien n ON n.MaNV = k.MaNV ORDER BY k.MaKL DESC");
        }

        // Quyết định khen thưởng và phiếu chi tiền thưởng lưu cùng lúc
        public string LapKhenThuong(string loai, string maDoiTuong, string noiDung, decimal soTien)
        {
            if (string.IsNullOrWhiteSpace(noiDung)) throw new NghiepVuException("Nhập nội dung khen thưởng.");
            if (soTien <= 0) throw new NghiepVuException("Số tiền thưởng phải lớn hơn 0.");
            string maNV = null, maPB = null;
            if (loai == "Cá nhân")
            {
                if (DbHelper.Query("SELECT 1 FROM NhanVien WHERE MaNV=@n AND TrangThai <> N'Đã nghỉ'", DbHelper.P("@n", maDoiTuong)).Rows.Count == 0)
                    throw new NghiepVuException("Không tìm thấy nhân viên.");
                maNV = maDoiTuong;
            }
            else if (loai == "Tập thể")
            {
                if (DbHelper.Query("SELECT 1 FROM PhongBan WHERE MaPB=@p", DbHelper.P("@p", maDoiTuong)).Rows.Count == 0)
                    throw new NghiepVuException("Không tìm thấy phòng ban.");
                maPB = maDoiTuong;
            }
            else throw new NghiepVuException("Loại khen thưởng không hợp lệ.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string kt = DbHelper.NextId(tx, "QuyetDinhKhenThuong", "MaKT", "KT");
                    DbHelper.Exec(tx, "INSERT INTO QuyetDinhKhenThuong (MaKT,NgayQD,LoaiKT,NoiDung,SoTien,MaNV,MaPB) VALUES (@a,@b,@c,@d,@e,@f,@g)",
                        DbHelper.P("@a", kt), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", loai), DbHelper.P("@d", noiDung), DbHelper.P("@e", soTien),
                        DbHelper.P("@f", maNV == null ? (object)DBNull.Value : maNV), DbHelper.P("@g", maPB == null ? (object)DBNull.Value : maPB));
                    DbHelper.Exec(tx, "INSERT INTO PhieuChi (MaPC,NgayChi,SoTien,MaKT) VALUES (@a,@b,@c,@d)",
                        DbHelper.P("@a", DbHelper.NextId(tx, "PhieuChi", "MaPC", "PC")), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", soTien), DbHelper.P("@d", kt));
                    tx.Commit();
                    return kt;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        // Phạt tiền thì lập thêm phiếu thu
        public string LapKyLuat(string maNV, string hinhThuc, string lyDo, decimal soTienPhat)
        {
            if (string.IsNullOrWhiteSpace(lyDo)) throw new NghiepVuException("Nhập lý do kỷ luật.");
            if (DbHelper.Query("SELECT 1 FROM NhanVien WHERE MaNV=@n AND TrangThai <> N'Đã nghỉ'", DbHelper.P("@n", maNV)).Rows.Count == 0)
                throw new NghiepVuException("Không tìm thấy nhân viên.");
            bool phat = hinhThuc == "Phạt tiền";
            if (phat && soTienPhat <= 0) throw new NghiepVuException("Phạt tiền phải có số tiền lớn hơn 0.");
            if (!phat) soTienPhat = 0;
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string kl = DbHelper.NextId(tx, "QuyetDinhKyLuat", "MaKL", "KL");
                    DbHelper.Exec(tx, "INSERT INTO QuyetDinhKyLuat (MaKL,NgayQD,HinhThuc,LyDo,SoTienPhat,MaNV) VALUES (@a,@b,@c,@d,@e,@f)",
                        DbHelper.P("@a", kl), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", hinhThuc), DbHelper.P("@d", lyDo), DbHelper.P("@e", soTienPhat), DbHelper.P("@f", maNV));
                    if (phat)
                        DbHelper.Exec(tx, "INSERT INTO PhieuThu (MaPT,NgayThu,SoTien,MaKL) VALUES (@a,@b,@c,@d)",
                            DbHelper.P("@a", DbHelper.NextId(tx, "PhieuThu", "MaPT", "PT")), DbHelper.P("@b", DateTime.Today), DbHelper.P("@c", soTienPhat), DbHelper.P("@d", kl));
                    tx.Commit();
                    return kl;
                }
                catch { tx.Rollback(); throw; }
            }
        }
    }
}
