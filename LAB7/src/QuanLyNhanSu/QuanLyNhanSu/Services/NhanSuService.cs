using System;
using System.Data;
using QuanLyNhanSu.Data;

namespace QuanLyNhanSu.Services
{
    // Tuyển dụng, hợp đồng, chức vụ, lương, bảo hiểm
    public class NhanSuService
    {
        public const int ThuViecToiDa = 60;          // giả định: thử việc tối đa 60 ngày
        public const decimal TyLeBaoHiem = 0.105m;   // giả định: 8% BHXH + 1,5% BHYT + 1% BHTN

        // Hồ sơ nhân viên và quyết định tuyển dụng lưu cùng lúc
        public string TuyenDung(string hoTen, DateTime ngaySinh, string diaChi, string sdt, string maPB,
            DateTime batDau, DateTime ketThuc, decimal luongThuViec, string dieuKhoan)
        {
            if (string.IsNullOrWhiteSpace(hoTen)) throw new NghiepVuException("Họ tên không được để trống.");
            if (ngaySinh.Date > DateTime.Today.AddYears(-18)) throw new NghiepVuException("Nhân viên phải đủ 18 tuổi.");
            if (ketThuc < batDau) throw new NghiepVuException("Ngày kết thúc thử việc phải sau ngày bắt đầu.");
            if ((ketThuc - batDau).Days > ThuViecToiDa) throw new NghiepVuException("Thời gian thử việc không quá " + ThuViecToiDa + " ngày.");
            if (luongThuViec < 0) throw new NghiepVuException("Lương thử việc không được âm.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string nv = DbHelper.NextId(tx, "NhanVien", "MaNV", "NV");
                    DbHelper.Exec(tx, "INSERT INTO NhanVien (MaNV,HoTen,NgaySinh,DiaChi,DienThoai,TrangThai,MaPB) VALUES (@a,@b,@c,@d,@e,N'Thử việc',@f)",
                        DbHelper.P("@a", nv), DbHelper.P("@b", hoTen), DbHelper.P("@c", ngaySinh.Date), DbHelper.P("@d", diaChi), DbHelper.P("@e", sdt), DbHelper.P("@f", maPB));
                    DbHelper.Exec(tx, "INSERT INTO QuyetDinhTuyenDung (MaQDTD,NgayQD,NgayBatDauThuViec,NgayKetThucThuViec,LuongThuViec,DieuKhoan,MaNV) VALUES (@a,@b,@c,@d,@e,@f,@g)",
                        DbHelper.P("@a", DbHelper.NextId(tx, "QuyetDinhTuyenDung", "MaQDTD", "TD")), DbHelper.P("@b", DateTime.Today),
                        DbHelper.P("@c", batDau.Date), DbHelper.P("@d", ketThuc.Date), DbHelper.P("@e", luongThuViec), DbHelper.P("@f", dieuKhoan), DbHelper.P("@g", nv));
                    tx.Commit();
                    return nv;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        // Ký hợp đồng chính thức: phải hết thời gian thử việc
        public string KyHopDong(string maNV, string loai, DateTime batDau, DateTime? ketThuc, decimal luong)
        {
            var td = DbHelper.Query("SELECT NgayKetThucThuViec FROM QuyetDinhTuyenDung WHERE MaNV=@n", DbHelper.P("@n", maNV));
            if (td.Rows.Count == 0) throw new NghiepVuException("Nhân viên chưa có quyết định tuyển dụng.");
            if (DbHelper.Query("SELECT 1 FROM NhanVien WHERE MaNV=@n AND TrangThai = N'Đã nghỉ'", DbHelper.P("@n", maNV)).Rows.Count > 0)
                throw new NghiepVuException("Nhân viên đã nghỉ việc.");
            DateTime het = Convert.ToDateTime(td.Rows[0][0]);
            if (DateTime.Today <= het) throw new NghiepVuException("Nhân viên chưa hết thời gian thử việc (đến " + het.ToString("dd/MM/yyyy") + ").");
            if (batDau.Date <= het) throw new NghiepVuException("Hợp đồng phải bắt đầu sau ngày kết thúc thử việc.");
            if (luong <= 0) throw new NghiepVuException("Lương hợp đồng phải lớn hơn 0.");
            if (loai == "Ngắn hạn")
            {
                if (ketThuc == null || ketThuc.Value.Date <= batDau.Date) throw new NghiepVuException("Hợp đồng ngắn hạn phải có ngày kết thúc sau ngày bắt đầu.");
            }
            else if (loai == "Dài hạn") ketThuc = null;
            else throw new NghiepVuException("Loại hợp đồng không hợp lệ.");
            // còn hợp đồng đang hiệu lực thì không ký tiếp
            if (DbHelper.Query("SELECT 1 FROM HopDong WHERE MaNV=@n AND (NgayKetThuc IS NULL OR NgayKetThuc >= @d)",
                DbHelper.P("@n", maNV), DbHelper.P("@d", batDau.Date)).Rows.Count > 0)
                throw new NghiepVuException("Nhân viên đang có hợp đồng còn hiệu lực.");
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string ma = DbHelper.NextId(tx, "HopDong", "MaHD", "HD");
                    DbHelper.Exec(tx, "INSERT INTO HopDong (MaHD,LoaiHD,NgayKy,NgayBatDau,NgayKetThuc,LuongHopDong,MaNV) VALUES (@a,@b,@c,@d,@e,@f,@g)",
                        DbHelper.P("@a", ma), DbHelper.P("@b", loai), DbHelper.P("@c", DateTime.Today), DbHelper.P("@d", batDau.Date),
                        DbHelper.P("@e", ketThuc.HasValue ? (object)ketThuc.Value.Date : DBNull.Value), DbHelper.P("@f", luong), DbHelper.P("@g", maNV));
                    DbHelper.Exec(tx, "UPDATE NhanVien SET TrangThai = N'Chính thức' WHERE MaNV=@n", DbHelper.P("@n", maNV));
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }

        public string QuyetDinhChucVu(string maNV, string maCV, DateTime hieuLuc)
        {
            KiemTraChinhThuc(maNV);
            return Them("QuyetDinhChucVu", "MaQDCV", "QC", "INSERT INTO QuyetDinhChucVu (MaQDCV,NgayQD,NgayHieuLuc,MaCV,MaNV) VALUES (@a,@b,@c,@d,@e)",
                new object[] { DateTime.Today, hieuLuc.Date, maCV, maNV });
        }

        public string QuyetDinhLuong(string maNV, decimal luongMoi, DateTime hieuLuc)
        {
            KiemTraChinhThuc(maNV);
            if (luongMoi <= 0) throw new NghiepVuException("Mức lương phải lớn hơn 0.");
            return Them("QuyetDinhLuong", "MaQDL", "QL", "INSERT INTO QuyetDinhLuong (MaQDL,NgayQD,NgayHieuLuc,LuongMoi,MaNV) VALUES (@a,@b,@c,@d,@e)",
                new object[] { DateTime.Today, hieuLuc.Date, luongMoi, maNV });
        }

        // Chỉ hợp đồng dài hạn mới có bảo hiểm
        public string DangKyBaoHiem(string maHD, string soSo)
        {
            if (string.IsNullOrWhiteSpace(soSo)) throw new NghiepVuException("Nhập số sổ bảo hiểm.");
            var dt = DbHelper.Query("SELECT LoaiHD FROM HopDong WHERE MaHD=@h", DbHelper.P("@h", maHD));
            if (dt.Rows.Count == 0) throw new NghiepVuException("Không tìm thấy hợp đồng.");
            if (dt.Rows[0][0].ToString() != "Dài hạn") throw new NghiepVuException("Chỉ hợp đồng dài hạn mới được tham gia bảo hiểm.");
            if (DbHelper.Query("SELECT 1 FROM BaoHiem WHERE MaHD=@h", DbHelper.P("@h", maHD)).Rows.Count > 0)
                throw new NghiepVuException("Hợp đồng này đã đăng ký bảo hiểm.");
            return Them("BaoHiem", "MaBH", "BH", "INSERT INTO BaoHiem (MaBH,SoSo,NgayThamGia,TyLeTrich,MaHD) VALUES (@a,@b,@c,@d,@e)",
                new object[] { soSo, DateTime.Today, TyLeBaoHiem, maHD });
        }

        static void KiemTraChinhThuc(string maNV)
        {
            if (DbHelper.Query("SELECT 1 FROM NhanVien WHERE MaNV=@n AND TrangThai = N'Chính thức'", DbHelper.P("@n", maNV)).Rows.Count == 0)
                throw new NghiepVuException("Chỉ áp dụng cho nhân viên chính thức.");
        }

        static string Them(string bang, string cot, string tienTo, string sql, object[] giaTri)
        {
            using (var c = DbHelper.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    string ma = DbHelper.NextId(tx, bang, cot, tienTo);
                    var ps = new System.Data.SqlClient.SqlParameter[giaTri.Length + 1];
                    ps[0] = DbHelper.P("@a", ma);
                    for (int i = 0; i < giaTri.Length; i++) ps[i + 1] = DbHelper.P("@" + (char)('b' + i), giaTri[i]);
                    DbHelper.Exec(tx, sql, ps);
                    tx.Commit();
                    return ma;
                }
                catch { tx.Rollback(); throw; }
            }
        }
    }
}
