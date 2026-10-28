/* Kich ban kiem thu muc CSDL (xem test/TestCases.md). Chay sau 01 va 02.
   Moi test in PASS/FAIL. Cac test "mong doi loi" dung TRY/CATCH. */
USE QuanLyNuocGiaiKhat;
GO
SET NOCOUNT ON;

-- TC-DB01: hoa don + chi tiet (tinh ThanhTien), giam ton kho khi ban du hang
BEGIN TRAN;
INSERT INTO HoaDon (MaHD,MaKH,MaNV,NgayLap,TongTien) VALUES ('HD01','KH01','NV01','2026-10-01',100000);
INSERT INTO ChiTietHoaDon (MaCTHD,MaHD,MaNGK,SoLuong,DonGia,SoLuongDaGiao) VALUES ('CT01','HD01','NGK01',10,10000,10);
UPDATE NuocGiaiKhat SET SoLuongTon = SoLuongTon - 10 WHERE MaNGK='NGK01';
SELECT CASE WHEN (SELECT ThanhTien FROM ChiTietHoaDon WHERE MaCTHD='CT01')=100000
             AND (SELECT SoLuongTon FROM NuocGiaiKhat WHERE MaNGK='NGK01')=90
            THEN 'PASS' ELSE 'FAIL' END AS [TC-DB01 ban du hang];
COMMIT;

-- TC-DB02: khong cho ton kho am
BEGIN TRY
    UPDATE NuocGiaiKhat SET SoLuongTon = SoLuongTon - 999 WHERE MaNGK='NGK04';
    SELECT 'FAIL' AS [TC-DB02 ton am];
END TRY
BEGIN CATCH
    SELECT 'PASS' AS [TC-DB02 ton am];
END CATCH;

-- TC-DB03: don dat hang chi co toi da 3 dot giao (dot 4 bi chan)
INSERT INTO DonDatHang (MaDDH,MaNCC,MaNV,NgayDat) VALUES ('DDH01','NCC01','NV02','2026-10-01');
INSERT INTO PhieuGiaoHang (MaPGH,MaDDH,DotGiao,NgayGiao) VALUES ('PGH01','DDH01',1,'2026-10-02'),('PGH02','DDH01',2,'2026-10-03'),('PGH03','DDH01',3,'2026-10-04');
BEGIN TRY
    INSERT INTO PhieuGiaoHang (MaPGH,MaDDH,DotGiao,NgayGiao) VALUES ('PGH04','DDH01',4,'2026-10-05');
    SELECT 'FAIL' AS [TC-DB03 toi da 3 dot];
END TRY
BEGIN CATCH
    SELECT 'PASS' AS [TC-DB03 toi da 3 dot];
END CATCH;

-- TC-DB04: 1 hoa don chi co 1 phieu ghi no
INSERT INTO PhieuGhiNo (MaPhieuNo,MaKH,MaNV,MaHD,NgayLap,SoTienNo,NgayDuKienTra,LyDoNo)
VALUES ('PN01','KH01','NV01','HD01','2026-10-01',50000,'2026-10-31',N'Khách quen mua chịu');
BEGIN TRY
    INSERT INTO PhieuGhiNo (MaPhieuNo,MaKH,MaNV,MaHD,NgayLap,SoTienNo,NgayDuKienTra)
    VALUES ('PN02','KH01','NV01','HD01','2026-10-02',10000,'2026-10-31');
    SELECT 'FAIL' AS [TC-DB04 1 HD - 1 phieu no];
END TRY
BEGIN CATCH
    SELECT 'PASS' AS [TC-DB04 1 HD - 1 phieu no];
END CATCH;

-- TC-DB05: so luong da giao khong vuot so luong mua
BEGIN TRY
    UPDATE ChiTietHoaDon SET SoLuongDaGiao = 11 WHERE MaCTHD='CT01';
    SELECT 'FAIL' AS [TC-DB05 da giao <= mua];
END TRY
BEGIN CATCH
    SELECT 'PASS' AS [TC-DB05 da giao <= mua];
END CATCH;

-- TC-DB06: khach quen bat buoc co ngay dang ky
BEGIN TRY
    INSERT INTO KhachHang (MaKH,HoTen,LoaiKhach) VALUES ('KH99',N'Test','QUEN');
    SELECT 'FAIL' AS [TC-DB06 khach quen can NgayDangKy];
END TRY
BEGIN CATCH
    SELECT 'PASS' AS [TC-DB06 khach quen can NgayDangKy];
END CATCH;
GO
-- Don dep du lieu test (chay lai 02_seed neu can)
DELETE FROM PhieuGhiNo; DELETE FROM ChiTietHoaDon; DELETE FROM HoaDon;
DELETE FROM PhieuGiaoHang; DELETE FROM DonDatHang;
UPDATE NuocGiaiKhat SET SoLuongTon = CASE MaNGK WHEN 'NGK01' THEN 100 WHEN 'NGK02' THEN 50 WHEN 'NGK03' THEN 20 ELSE 5 END;
GO
