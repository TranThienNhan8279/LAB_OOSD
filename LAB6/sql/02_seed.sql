USE QuanLyBenhAn;
GO
DECLARE @h DATE = CAST(GETDATE() AS DATE);   -- ngày dữ liệu tính theo hôm nay
DECLARE @nam INT = YEAR(@h);

INSERT INTO Khoa VALUES ('K01', N'Nội tổng quát'), ('K02', N'Nhi');
INSERT INTO BacSi VALUES ('BS01', N'Lê Minh Quân', N'Nội tiêu hóa', 'K01'), ('BS02', N'Phạm Thu Hà', N'Nhi hô hấp', 'K02');
INSERT INTO NhanVien VALUES ('NV01', N'Võ Thị Lan', N'Chăm sóc khách hàng'), ('NV02', N'Đặng Văn Hải', N'Thu ngân');
INSERT INTO Thuoc VALUES ('TH01', N'Paracetamol 500mg', N'viên'), ('TH02', N'Amoxicillin 500mg', N'viên'), ('TH03', N'Vitamin C', N'viên');
INSERT INTO LoaiXetNghiem VALUES ('XN01', N'Công thức máu'), ('XN02', N'X-quang phổi');

INSERT INTO BenhNhan VALUES
 ('BN0001', N'Nguyễn Văn An',  N'12 Lê Lợi, Q1',     '1990-05-12', '0901234567', N'an.jpg'),
 ('BN0002', N'Trần Thị Bình',  N'45 Hai Bà Trưng, Q3', '1985-09-30', '0912345678', N'binh.jpg');
INSERT INTO PhieuDangKy VALUES ('PD0001', DATEADD(DAY,-200,@h), 'BN0001'), ('PD0002', DATEADD(YEAR,-1,@h), 'BN0002');
INSERT INTO LePhi VALUES
 ('LP0001', @nam,   200000, @h, 'BN0001'),
 ('LP0002', @nam-1, 200000, DATEADD(YEAR,-1,@h), 'BN0002');     -- BN0002 chưa đóng lệ phí năm nay
INSERT INTO HoSoBenhAn VALUES ('HS0001', DATEADD(DAY,-200,@h), 'BN0001'), ('HS0002', DATEADD(YEAR,-1,@h), 'BN0002');

-- PK0001: đã xuất viện, thanh toán, có toa, chưa ghi nhận vào hồ sơ
INSERT INTO PhieuKham (MaPhieuKham,NgayGioKham,TrieuChung,ThanNhiet,HuyetAp,LoaiKham,TrangThai,MaHoSo,MaBS,MaKhoa,MaNVGhiNhan) VALUES
 ('PK0001', DATEADD(DAY,-10,GETDATE()), N'Sốt, ho', 38.5, N'120/80', N'Điều trị', N'Hoàn tất', 'HS0001', 'BS01', 'K01', NULL),
 ('PK0002', DATEADD(DAY,-1,GETDATE()),  N'Đau bụng', 37.0, N'110/70', N'Điều trị', N'Đang điều trị', 'HS0001', 'BS01', 'K01', NULL),
 ('PK0003', GETDATE(),                  N'Nhức đầu', 36.8, N'115/75', N'Điều trị', N'Đang điều trị', 'HS0001', 'BS01', 'K01', NULL);
INSERT INTO ToaThuoc VALUES ('TO0001', DATEADD(DAY,-10,@h), N'Uống sau ăn', 'PK0001');
INSERT INTO ChiTietToaThuoc VALUES ('CT0001', 10, N'2 viên/ngày', 'TO0001', 'TH01'), ('CT0002', 14, N'1 viên sáng, 1 viên tối', 'TO0001', 'TH02');
INSERT INTO XetNghiem VALUES ('XN0001', DATEADD(DAY,-10,@h), N'Bình thường', 'PK0001', 'XN01'), ('XN0002', @h, NULL, 'PK0002', 'XN01');
INSERT INTO ThanhToanVienPhi VALUES ('TT0001', DATEADD(DAY,-10,@h), 350000, DATEADD(DAY,-10,@h), 'PK0001');
GO
