USE QuanLyNhanSu;
GO
SET DATEFIRST 7;
DECLARE @h DATE = CAST(GETDATE() AS DATE);   -- ngày dữ liệu tính theo hôm nay
DECLARE @thu2 DATE = DATEADD(DAY, -((DATEPART(WEEKDAY, @h) + 5) % 7), @h);      -- thứ Hai của tuần này
DECLARE @dauThangTruoc DATE = DATEFROMPARTS(YEAR(DATEADD(MONTH,-1,@h)), MONTH(DATEADD(MONTH,-1,@h)), 1);

INSERT INTO PhongBan VALUES ('PB01', N'Kỹ thuật'), ('PB02', N'Kinh doanh');
INSERT INTO ChucVu VALUES ('CV01', N'Nhân viên', 0), ('CV02', N'Trưởng phòng', 3000000);

INSERT INTO NhanVien (MaNV,HoTen,NgaySinh,DiaChi,DienThoai,TrangThai,MaPB) VALUES
 ('NV0001', N'Nguyễn Hoàng Nam', '1990-03-15', N'Q1, TP.HCM',  '0901111111', N'Chính thức', 'PB01'),
 ('NV0002', N'Lê Thị Hoa',       '1995-07-20', N'Q3, TP.HCM',  '0902222222', N'Chính thức', 'PB01'),
 ('NV0003', N'Phạm Quốc Bảo',    '1998-11-02', N'Thủ Đức',     '0903333333', N'Thử việc',   'PB02'),
 ('NV0004', N'Trần Minh Tuấn',   '1997-01-25', N'Bình Thạnh',  '0904444444', N'Thử việc',   'PB02');

INSERT INTO QuyetDinhTuyenDung (MaQDTD,NgayQD,NgayBatDauThuViec,NgayKetThucThuViec,LuongThuViec,DieuKhoan,MaNV) VALUES
 ('TD0001', DATEADD(DAY,-461,@h), DATEADD(DAY,-460,@h), DATEADD(DAY,-401,@h), 8000000, N'Thử việc 2 tháng', 'NV0001'),
 ('TD0002', DATEADD(DAY,-261,@h), DATEADD(DAY,-260,@h), DATEADD(DAY,-201,@h), 6000000, N'Thử việc 2 tháng', 'NV0002'),
 ('TD0003', DATEADD(DAY,-11,@h),  DATEADD(DAY,-10,@h),  DATEADD(DAY,20,@h),   6000000, N'Thử việc 1 tháng', 'NV0003'),
 ('TD0004', DATEADD(DAY,-76,@h),  DATEADD(DAY,-75,@h),  DATEADD(DAY,-16,@h),  7000000, N'Thử việc 2 tháng', 'NV0004');

INSERT INTO HopDong (MaHD,LoaiHD,NgayKy,NgayBatDau,NgayKetThuc,LuongHopDong,MaNV) VALUES
 ('HD0001', N'Dài hạn',   DATEADD(DAY,-400,@h), DATEADD(DAY,-400,@h), NULL,                  12000000, 'NV0001'),
 ('HD0002', N'Ngắn hạn',  DATEADD(DAY,-200,@h), DATEADD(DAY,-200,@h), DATEADD(DAY,165,@h),    8000000, 'NV0002');
INSERT INTO BaoHiem VALUES ('BH0001', N'SH-0001', DATEADD(DAY,-400,@h), 0.105, 'HD0001');
INSERT INTO QuyetDinhChucVu VALUES ('QC0001', DATEADD(DAY,-300,@h), DATEADD(DAY,-300,@h), 'CV02', 'NV0001');

INSERT INTO KeHoachTuan VALUES ('KH0001', @thu2, N'Kế hoạch tuần này', 'PB01');
INSERT INTO ChiTietPhanCong VALUES ('CP0001', @thu2, N'Sáng', N'Bảo trì hệ thống', 'KH0001', 'NV0001'),
                                   ('CP0002', @thu2, N'Sáng', N'Hỗ trợ khách hàng', 'KH0001', 'NV0002');

-- dữ liệu của tháng trước để thử bảng lương
INSERT INTO PhieuChamCong VALUES ('CC0001', DATEADD(DAY,2,@dauThangTruoc), 8, 'NV0001'), ('CC0002', DATEADD(DAY,2,@dauThangTruoc), 8, 'NV0002');
INSERT INTO PhieuPhuTroi VALUES ('PP0001', DATEADD(DAY,9,@dauThangTruoc), 1.5, 'PB01');
INSERT INTO ChiTietPhuTroi VALUES ('CD0001', 3, 'PP0001', 'NV0001'), ('CD0002', 2, 'PP0001', 'NV0002');
INSERT INTO QuyetDinhKhenThuong VALUES ('KT0001', DATEADD(DAY,14,@dauThangTruoc), N'Cá nhân', N'Hoàn thành tốt dự án', 500000, 'NV0002', NULL);
INSERT INTO PhieuChi VALUES ('PC0001', DATEADD(DAY,14,@dauThangTruoc), 500000, 'KT0001');
INSERT INTO QuyetDinhKyLuat VALUES ('KL0001', DATEADD(DAY,19,@dauThangTruoc), N'Phạt tiền', N'Đi làm trễ nhiều lần', 200000, 'NV0001');
INSERT INTO PhieuThu VALUES ('PT0001', DATEADD(DAY,19,@dauThangTruoc), 200000, 'KL0001');
GO
