/* Test ràng buộc CSDL (test/TestCases.md). Chạy sau 01_schema.sql và 02_seed.sql. */
USE QuanLyBenhAn;
GO
SET NOCOUNT ON;
DECLARE @h DATE = CAST(GETDATE() AS DATE);

-- TC-DB01: lệ phí phải lớn hơn 0
BEGIN TRY
    INSERT INTO LePhi (MaLePhi,Nam,SoTien,NgayDong,MaBN) VALUES ('LP9001',2001,0,@h,'BN0001');
    SELECT 'FAIL' AS [TC-DB01 le phi > 0];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB01 le phi > 0]; END CATCH;

-- TC-DB02: mỗi năm bệnh nhân chỉ đóng một lệ phí
BEGIN TRY
    INSERT INTO LePhi (MaLePhi,Nam,SoTien,NgayDong,MaBN) VALUES ('LP9002',YEAR(@h),200000,@h,'BN0001');
    SELECT 'FAIL' AS [TC-DB02 1 le phi moi nam];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB02 1 le phi moi nam]; END CATCH;

-- TC-DB03: loại khám chỉ là Điều trị hoặc Định kỳ
BEGIN TRY
    INSERT INTO PhieuKham (MaPhieuKham,NgayGioKham,TrieuChung,ThanNhiet,HuyetAp,LoaiKham,TrangThai,MaHoSo,MaBS,MaKhoa) VALUES ('PK9001',GETDATE(),N'x',37,NULL,N'Khác',N'Đang điều trị','HS0001','BS01','K01');
    SELECT 'FAIL' AS [TC-DB03 loai kham hop le];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB03 loai kham hop le]; END CATCH;

-- TC-DB04: mỗi phiếu khám chỉ có một toa thuốc
BEGIN TRY
    INSERT INTO ToaThuoc (MaToa,NgayLap,LoiDan,MaPhieuKham) VALUES ('TO9001',@h,N'x','PK0001');
    SELECT 'FAIL' AS [TC-DB04 1 toa moi phieu];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB04 1 toa moi phieu]; END CATCH;

-- TC-DB05: số lượng thuốc phải lớn hơn 0
BEGIN TRY
    INSERT INTO ChiTietToaThuoc (MaCTT,SoLuong,LieuDung,MaToa,MaThuoc) VALUES ('CT9001',0,N'x','TO0001','TH03');
    SELECT 'FAIL' AS [TC-DB05 so luong > 0];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB05 so luong > 0]; END CATCH;

-- TC-DB06: quý chỉ từ 1 đến 4 và mỗi quý một lịch
BEGIN TRY
    INSERT INTO LichKhamDinhKy (MaLich,Quy,Nam,NgayHen,TrangThai,MaHoSo) VALUES ('LK9001',5,YEAR(@h),GETDATE(),N'Đã hẹn','HS0001');
    SELECT 'FAIL' AS [TC-DB06 quy 1-4];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB06 quy 1-4]; END CATCH;

-- TC-DB07: xóa toa thì xóa luôn chi tiết thuốc (chạy trong transaction, rồi hoàn lại)
BEGIN TRAN;
    DELETE FROM ToaThuoc WHERE MaToa = 'TO0001';
    SELECT CASE WHEN NOT EXISTS (SELECT 1 FROM ChiTietToaThuoc WHERE MaToa='TO0001') THEN 'PASS' ELSE 'FAIL' END AS [TC-DB07 xoa toa xoa chi tiet];
ROLLBACK;
GO
