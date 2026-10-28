USE QuanLyDuLich;
GO
DECLARE @h DATE = CAST(GETDATE() AS DATE);   -- ngày dữ liệu tính theo hôm nay

INSERT INTO PhuongTien VALUES ('PT01', N'Máy bay'), ('PT02', N'Xe đò'), ('PT03', N'Tàu hỏa');

INSERT INTO Tour VALUES
 ('T01', N'Hà Nội - Hạ Long', 4, 3, 5000000, 800000),
 ('T02', N'Đà Nẵng - Hội An',  3, 2, 3500000, 600000),
 ('T03', N'Phú Quốc',          3, 2, 4200000, 700000);

INSERT INTO DiemDung (MaDiemDung,TenDiemDung,ThuTu,DoiPhuongTien,CoAnUong,CoKhachSan,SoSaoKhachSan,MaTour,MaPT) VALUES
 ('D01', N'Hà Nội',    1, 1, 1, 1, 3, 'T01', 'PT01'),
 ('D02', N'Hạ Long',   2, 1, 1, 1, 4, 'T01', 'PT02'),
 ('D03', N'Đà Nẵng',   1, 1, 1, 1, 3, 'T02', 'PT01'),
 ('D04', N'Hội An',    2, 0, 1, 1, 4, 'T02', 'PT02'),
 ('D05', N'Phú Quốc',  1, 1, 1, 1, 5, 'T03', 'PT01');

INSERT INTO DiemThamQuan VALUES
 ('DT01', N'Vịnh Hạ Long', N'Quảng Ninh', N'Di sản thiên nhiên thế giới', N'Danh lam thắng cảnh'),
 ('DT02', N'Phố cổ Hội An', N'Quảng Nam', N'Khu phố cổ', N'Di tích lịch sử');
INSERT INTO Tour_DiemThamQuan VALUES ('T01','DT01'), ('T02','DT02');

INSERT INTO NhanVien VALUES
 ('NV01', N'Lê Hùng',   '0901000001', N'Hướng dẫn viên', 8000000),
 ('NV02', N'Phạm Lan',  '0901000002', N'Hướng dẫn viên', 8000000),
 ('NV03', N'Trần Bình', '0901000003', N'Nhân viên bán vé', 7000000);

INSERT INTO KhachHang (MaKH,LoaiKhachHang,HoTen,DiaChi,DienThoai,TenCoQuan,NguoiDaiDien) VALUES
 ('KH01','KHACHLE',  N'Nguyễn Văn A', N'Q.1',  '0912000001', NULL, NULL),
 ('KH02','KHACHLE',  N'Trần Thị B',   N'Q.5',  '0912000002', NULL, NULL),
 ('KH03','KHACHDOAN',NULL, N'Q.3',  '0283000003', N'Công ty ABC', N'Ông Hùng'),
 ('KH04','KHACHDOAN',NULL, N'Q.10', '0283000004', N'Trường XYZ',  N'Cô Mai');

INSERT INTO ChuyenDi (MaChuyen,NgayDi,NgayVe,DiaDiemDon,MaTour,MaNV) VALUES
 ('CD01', DATEADD(DAY,10,@h), DATEADD(DAY,13,@h), N'Bến xe Miền Đông', 'T01', NULL),
 ('CD02', DATEADD(DAY,10,@h), DATEADD(DAY,12,@h), N'Nhà thi đấu Phú Thọ', 'T02', NULL),
 ('CD03', DATEADD(DAY,-30,@h), DATEADD(DAY,-28,@h), N'Bến xe Miền Đông', 'T03', 'NV01');

-- Đoàn đã đi xong cách đây 20 ngày, mới đặt cọc (dùng cho test quyết toán)
INSERT INTO PhieuDangKyDoan (MaPhieuDoan,NgayDangKy,NgayDi,SoNguoi,DiaDiemDon,TienCoc,CoBaoHiem,TrangThai,MaKH,MaTour) VALUES
 ('PD0001', DATEADD(DAY,-40,@h), DATEADD(DAY,-20,@h), 15, N'Công ty ABC', 15750000, 0, N'Đã đặt cọc', 'KH03', 'T02');
INSERT INTO PhieuThanhToanDoan VALUES ('TT0001', DATEADD(DAY,-40,@h), 15750000, N'Đặt cọc', 'PD0001');
GO
