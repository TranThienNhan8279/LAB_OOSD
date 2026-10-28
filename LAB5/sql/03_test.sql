/* Test ràng buộc CSDL (test/TestCases.md). Chạy sau 01_schema.sql và 02_seed.sql. */
USE QuanLyDuLich;
GO
SET NOCOUNT ON;
DECLARE @h DATE = CAST(GETDATE() AS DATE);

-- TC-DB01: khách lẻ tối đa 12 người
BEGIN TRY
    INSERT INTO PhieuDangKyChuyen (MaPhieuLe,NgayDangKy,SoNguoi,DiaDiemDon,SoTien,MaKH,MaChuyen) VALUES ('PL9001',@h,13,N'x',1,'KH01','CD01');
    SELECT 'FAIL' AS [TC-DB01 khach le <= 12 nguoi];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB01 khach le <= 12 nguoi]; END CATCH;

-- TC-DB02: khách đoàn phải trên 12 người
BEGIN TRY
    INSERT INTO PhieuDangKyDoan (MaPhieuDoan,NgayDangKy,NgayDi,SoNguoi,TienCoc,CoBaoHiem,TrangThai,MaKH,MaTour) VALUES ('PD9001',@h,@h,12,0,0,N'x','KH03','T01');
    SELECT 'FAIL' AS [TC-DB02 khach doan > 12 nguoi];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB02 khach doan > 12 nguoi]; END CATCH;

-- TC-DB03: ngày về không được trước ngày đi
BEGIN TRY
    INSERT INTO ChuyenDi (MaChuyen,NgayDi,NgayVe,DiaDiemDon,MaTour) VALUES ('CD9001',@h,DATEADD(DAY,-1,@h),N'x','T01');
    SELECT 'FAIL' AS [TC-DB03 ngay ve >= ngay di];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB03 ngay ve >= ngay di]; END CATCH;

-- TC-DB04: khách sạn chỉ 2, 3, 4 hoặc 5 sao
BEGIN TRY
    INSERT INTO DiemDung (MaDiemDung,TenDiemDung,ThuTu,DoiPhuongTien,CoAnUong,CoKhachSan,SoSaoKhachSan,MaTour,MaPT) VALUES ('D9001',N'x',9,0,0,1,1,'T01','PT01');
    SELECT 'FAIL' AS [TC-DB04 khach san 2-5 sao];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB04 khach san 2-5 sao]; END CATCH;

-- TC-DB05: phiếu khảo sát phải thuộc đúng một chuyến hoặc một đoàn
BEGIN TRY
    INSERT INTO PhieuKhaoSat (MaPKS,NgayGui,DiemHaiLong,GopY,MaKH,MaChuyen,MaPhieuDoan) VALUES ('KS9001',@h,5,N'x','KH01',NULL,NULL);
    SELECT 'FAIL' AS [TC-DB05 khao sat thuoc 1 nguon];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB05 khao sat thuoc 1 nguon]; END CATCH;

-- TC-DB06: một nhân viên chỉ phân công một lần cho cùng đoàn
INSERT INTO PhanCongDoan (MaPC,VaiTro,MaPhieuDoan,MaNV) VALUES ('PC9001',N'Trưởng đoàn','PD0001','NV02');
BEGIN TRY
    INSERT INTO PhanCongDoan (MaPC,VaiTro,MaPhieuDoan,MaNV) VALUES ('PC9002',N'Phụ','PD0001','NV02');
    SELECT 'FAIL' AS [TC-DB06 khong phan cong trung];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB06 khong phan cong trung]; END CATCH;

-- TC-DB07: mỗi nhân viên chỉ có 1 bảng lương cho mỗi tháng
INSERT INTO LuongThang VALUES ('LG9001',1,2026,8000000,0,8000000,'NV01');
BEGIN TRY
    INSERT INTO LuongThang VALUES ('LG9002',1,2026,8000000,0,8000000,'NV01');
    SELECT 'FAIL' AS [TC-DB07 luong 1 thang 1 dong];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB07 luong 1 thang 1 dong]; END CATCH;
GO
-- dọn dữ liệu test
DELETE FROM LuongThang WHERE MaLuong LIKE 'LG9%';
DELETE FROM PhanCongDoan WHERE MaPC LIKE 'PC9%';
GO
