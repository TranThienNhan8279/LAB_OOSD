/* Test ràng buộc CSDL (test/TestCases.md). Chạy sau 01_schema.sql và 02_seed.sql. */
USE QuanLyNhanSu;
GO
SET NOCOUNT ON;
DECLARE @h DATE = CAST(GETDATE() AS DATE);

-- TC-DB01: hợp đồng dài hạn không có ngày kết thúc
BEGIN TRY
    INSERT INTO HopDong (MaHD,LoaiHD,NgayKy,NgayBatDau,NgayKetThuc,LuongHopDong,MaNV) VALUES ('HD9001',N'Dài hạn',@h,@h,DATEADD(YEAR,1,@h),1000000,'NV0003');
    SELECT 'FAIL' AS [TC-DB01 dai han khong co ngay ket thuc];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB01 dai han khong co ngay ket thuc]; END CATCH;

-- TC-DB02: hợp đồng ngắn hạn phải có ngày kết thúc
BEGIN TRY
    INSERT INTO HopDong (MaHD,LoaiHD,NgayKy,NgayBatDau,NgayKetThuc,LuongHopDong,MaNV) VALUES ('HD9002',N'Ngắn hạn',@h,@h,NULL,1000000,'NV0003');
    SELECT 'FAIL' AS [TC-DB02 ngan han co ngay ket thuc];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB02 ngan han co ngay ket thuc]; END CATCH;

-- TC-DB03: khen thưởng cá nhân phải có nhân viên
BEGIN TRY
    INSERT INTO QuyetDinhKhenThuong (MaKT,NgayQD,LoaiKT,NoiDung,SoTien,MaNV,MaPB) VALUES ('KT9001',@h,N'Cá nhân',N'x',1000,NULL,'PB01');
    SELECT 'FAIL' AS [TC-DB03 khen thuong dung doi tuong];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB03 khen thuong dung doi tuong]; END CATCH;

-- TC-DB04: phạt tiền phải có số tiền, hình thức khác thì không có tiền
BEGIN TRY
    INSERT INTO QuyetDinhKyLuat (MaKL,NgayQD,HinhThuc,LyDo,SoTienPhat,MaNV) VALUES ('KL9001',@h,N'Phạt tiền',N'x',0,'NV0001');
    SELECT 'FAIL' AS [TC-DB04 phat tien co so tien];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB04 phat tien co so tien]; END CATCH;

-- TC-DB05: giờ phụ trội tối đa 4 giờ
BEGIN TRY
    INSERT INTO ChiTietPhuTroi (MaCTPT,SoGio,MaPPT,MaNV) VALUES ('CD9001',5,'PP0001','NV0001');
    SELECT 'FAIL' AS [TC-DB05 phu troi toi da 4 gio];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB05 phu troi toi da 4 gio]; END CATCH;

-- TC-DB06: hệ số phụ trội không thấp hơn 1.5
BEGIN TRY
    INSERT INTO PhieuPhuTroi (MaPPT,Ngay,HeSo,MaPB) VALUES ('PP9001',@h,1.0,'PB01');
    SELECT 'FAIL' AS [TC-DB06 he so phu troi >= 1.5];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB06 he so phu troi >= 1.5]; END CATCH;

-- TC-DB07: một nhân viên chỉ chấm công một lần mỗi ngày
BEGIN TRY
    INSERT INTO PhieuChamCong (MaPCC,Ngay,SoGioLam,MaNV) SELECT 'CC9001',Ngay,8,MaNV FROM PhieuChamCong WHERE MaPCC='CC0001';
    SELECT 'FAIL' AS [TC-DB07 cham cong 1 lan moi ngay];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB07 cham cong 1 lan moi ngay]; END CATCH;

-- TC-DB08: không phân công trùng ca
BEGIN TRY
    INSERT INTO ChiTietPhanCong (MaCTPC,NgayLam,CaLam,CongViec,MaKHT,MaNV) SELECT 'CP9001',NgayLam,CaLam,N'x',MaKHT,MaNV FROM ChiTietPhanCong WHERE MaCTPC='CP0001';
    SELECT 'FAIL' AS [TC-DB08 khong phan cong trung ca];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB08 khong phan cong trung ca]; END CATCH;

-- TC-DB09: mỗi hợp đồng chỉ có một thông tin bảo hiểm
BEGIN TRY
    INSERT INTO BaoHiem (MaBH,SoSo,NgayThamGia,TyLeTrich,MaHD) VALUES ('BH9001',N'x',@h,0.105,'HD0001');
    SELECT 'FAIL' AS [TC-DB09 1 bao hiem moi hop dong];
END TRY
BEGIN CATCH SELECT 'PASS' AS [TC-DB09 1 bao hiem moi hop dong]; END CATCH;
GO
