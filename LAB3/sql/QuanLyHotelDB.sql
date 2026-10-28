/* LAB3 - Hệ thống quản lý khách sạn
   Tạo CSDL QuanLyHotelDB theo ERD (diagrams/ERD.drawio) và mã nguồn. Chạy trong SSMS. */
IF DB_ID(N'QuanLyHotelDB') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyHotelDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyHotelDB;
END
GO
CREATE DATABASE QuanLyHotelDB;
GO
USE QuanLyHotelDB;
GO

CREATE TABLE KhuVuc (
    MaKhuVuc VARCHAR(10) NOT NULL PRIMARY KEY,
    TenKhuVuc NVARCHAR(100) NOT NULL
);
CREATE TABLE NhanVien (
    MaNV VARCHAR(10) NOT NULL PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    VaiTro NVARCHAR(50) NOT NULL,
    SoDienThoai VARCHAR(15) NULL
);
CREATE TABLE KhachHang (
    MaKhach VARCHAR(10) NOT NULL PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SoCMND VARCHAR(20) NOT NULL UNIQUE,
    QuocTich NVARCHAR(50) NOT NULL DEFAULT N'Việt Nam',
    SoDienThoai VARCHAR(15) NULL
);
CREATE TABLE LoaiTienNghi (
    MaLoaiTN VARCHAR(10) NOT NULL PRIMARY KEY,
    TenLoaiTN NVARCHAR(100) NOT NULL
);
CREATE TABLE DichVu (
    MaDV VARCHAR(10) NOT NULL PRIMARY KEY,
    TenDV NVARCHAR(100) NOT NULL,
    DonViTinh NVARCHAR(30) NOT NULL,
    DonGia DECIMAL(18,0) NOT NULL CHECK (DonGia >= 0)
);

CREATE TABLE Phong (
    SoPhong VARCHAR(10) NOT NULL PRIMARY KEY,
    MaKhuVuc VARCHAR(10) NOT NULL REFERENCES KhuVuc(MaKhuVuc),
    SoNguoiToiDa INT NOT NULL CHECK (SoNguoiToiDa > 0),
    DonGiaNgay DECIMAL(18,0) NOT NULL CHECK (DonGiaNgay >= 0),
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Trống',
    CONSTRAINT CK_Phong_TT CHECK (TrangThai IN (N'Trống', N'Đã đặt', N'Đang ở', N'Bảo trì'))
);
CREATE TABLE TienNghi (
    MaTienNghi VARCHAR(10) NOT NULL PRIMARY KEY,
    MaLoaiTN VARCHAR(10) NOT NULL REFERENCES LoaiTienNghi(MaLoaiTN),
    SoThuTu INT NOT NULL,
    TinhTrangHienTai NVARCHAR(50) NOT NULL DEFAULT N'Tốt',
    CONSTRAINT UQ_TienNghi UNIQUE (MaLoaiTN, SoThuTu)
);
CREATE TABLE QuyDinhDenBu (
    MaQuyDinh VARCHAR(10) NOT NULL PRIMARY KEY,
    MaLoaiTN VARCHAR(10) NOT NULL REFERENCES LoaiTienNghi(MaLoaiTN),
    MucDoThietHai NVARCHAR(50) NOT NULL,
    MucDenBu DECIMAL(18,0) NOT NULL CHECK (MucDenBu >= 0),
    CONSTRAINT UQ_QuyDinhDenBu UNIQUE (MaLoaiTN, MucDoThietHai)
);
CREATE TABLE PhieuLapDat (
    SoPhieuLapDat VARCHAR(10) NOT NULL PRIMARY KEY,
    MaTienNghi VARCHAR(10) NOT NULL REFERENCES TienNghi(MaTienNghi),
    SoPhong VARCHAR(10) NOT NULL REFERENCES Phong(SoPhong),
    NgayLap DATE NOT NULL,
    TinhTrang NVARCHAR(50) NULL,
    MaNV VARCHAR(10) NOT NULL REFERENCES NhanVien(MaNV),
    GhiChu NVARCHAR(200) NULL
);

CREATE TABLE PhieuDatPhong (
    SoPhieuDat VARCHAR(10) NOT NULL PRIMARY KEY,
    MaKhach VARCHAR(10) NOT NULL REFERENCES KhachHang(MaKhach),
    MaNVLeTan VARCHAR(10) NOT NULL REFERENCES NhanVien(MaNV),
    NgayLap DATETIME NOT NULL DEFAULT GETDATE(),
    NgayNhan DATE NOT NULL,
    NgayTraDuKien DATE NOT NULL,
    NgayNhanThucTe DATETIME NULL,
    NgayTraThucTe DATETIME NULL,
    TienCoc DECIMAL(18,0) NOT NULL DEFAULT 0 CHECK (TienCoc >= 0),
    KenhDat NVARCHAR(30) NOT NULL DEFAULT N'Trực tiếp',
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Đã đặt',
    CONSTRAINT CK_PDP_Ngay CHECK (NgayTraDuKien > NgayNhan),
    CONSTRAINT CK_PDP_Kenh CHECK (KenhDat IN (N'Trực tiếp', N'Điện thoại', N'Website')),
    CONSTRAINT CK_PDP_TT CHECK (TrangThai IN (N'Đã đặt', N'Đang ở', N'Đã trả', N'Hủy'))
);
-- một phiếu đặt có nhiều phòng; mỗi phòng ghi số người ở
CREATE TABLE ChiTietDatPhong (
    SoPhieuDat VARCHAR(10) NOT NULL REFERENCES PhieuDatPhong(SoPhieuDat) ON DELETE CASCADE,
    SoPhong VARCHAR(10) NOT NULL REFERENCES Phong(SoPhong),
    SoNguoi INT NOT NULL CHECK (SoNguoi > 0),
    PRIMARY KEY (SoPhieuDat, SoPhong)
);
CREATE TABLE NguoiLuuTru (
    SoPhieuDat VARCHAR(10) NOT NULL,
    SoPhong VARCHAR(10) NOT NULL,
    SoCMND VARCHAR(20) NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    QuocTich NVARCHAR(50) NOT NULL DEFAULT N'Việt Nam',
    PRIMARY KEY (SoPhieuDat, SoPhong, SoCMND),
    FOREIGN KEY (SoPhieuDat, SoPhong) REFERENCES ChiTietDatPhong(SoPhieuDat, SoPhong) ON DELETE CASCADE
);

CREATE TABLE PhieuSuDungDV (
    SoPhieuSDDV VARCHAR(10) NOT NULL PRIMARY KEY,
    SoPhieuDat VARCHAR(10) NOT NULL,
    SoPhong VARCHAR(10) NOT NULL,
    NgaySuDung DATETIME NOT NULL,
    MaNV VARCHAR(10) NOT NULL REFERENCES NhanVien(MaNV),
    FOREIGN KEY (SoPhieuDat, SoPhong) REFERENCES ChiTietDatPhong(SoPhieuDat, SoPhong)
);
CREATE TABLE ChiTietPhieuSuDungDV (
    SoPhieuSDDV VARCHAR(10) NOT NULL REFERENCES PhieuSuDungDV(SoPhieuSDDV) ON DELETE CASCADE,
    MaDV VARCHAR(10) NOT NULL REFERENCES DichVu(MaDV),
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia DECIMAL(18,0) NOT NULL CHECK (DonGia >= 0),
    ThanhTien AS (SoLuong * DonGia),
    PRIMARY KEY (SoPhieuSDDV, MaDV)
);

CREATE TABLE HoaDon (
    SoHoaDon VARCHAR(10) NOT NULL PRIMARY KEY,
    SoPhieuDat VARCHAR(10) NOT NULL REFERENCES PhieuDatPhong(SoPhieuDat),
    NgayLap DATETIME NOT NULL DEFAULT GETDATE(),
    MaNV VARCHAR(10) NOT NULL REFERENCES NhanVien(MaNV),
    SoNgayTinhTien INT NOT NULL CHECK (SoNgayTinhTien >= 1),
    TienPhong DECIMAL(18,0) NOT NULL CHECK (TienPhong >= 0),
    TienDichVu DECIMAL(18,0) NOT NULL DEFAULT 0 CHECK (TienDichVu >= 0),
    TongTien AS (TienPhong + TienDichVu),
    TrangThai NVARCHAR(20) NOT NULL DEFAULT N'Chưa thanh toán',
    CONSTRAINT CK_HD_TT CHECK (TrangThai IN (N'Chưa thanh toán', N'Đã thanh toán')),
    CONSTRAINT UQ_HoaDon_PhieuDat UNIQUE (SoPhieuDat)
);
CREATE TABLE ThanhToan (
    MaThanhToan VARCHAR(10) NOT NULL PRIMARY KEY,
    SoHoaDon VARCHAR(10) NOT NULL REFERENCES HoaDon(SoHoaDon),
    NgayThanhToan DATETIME NOT NULL DEFAULT GETDATE(),
    HinhThuc NVARCHAR(30) NOT NULL,
    SoTien DECIMAL(18,0) NOT NULL CHECK (SoTien > 0),
    CONSTRAINT CK_TT_HinhThuc CHECK (HinhThuc IN (N'Tiền mặt', N'Chuyển khoản', N'Thẻ', N'Ví điện tử'))
);

CREATE TABLE PhieuDenBu (
    SoPhieuDenBu VARCHAR(10) NOT NULL PRIMARY KEY,
    SoPhieuDat VARCHAR(10) NOT NULL,
    SoPhong VARCHAR(10) NOT NULL,
    NgayLap DATETIME NOT NULL DEFAULT GETDATE(),
    MaNV VARCHAR(10) NOT NULL REFERENCES NhanVien(MaNV),
    TongTien DECIMAL(18,0) NOT NULL DEFAULT 0 CHECK (TongTien >= 0),
    FOREIGN KEY (SoPhieuDat, SoPhong) REFERENCES ChiTietDatPhong(SoPhieuDat, SoPhong)
);
CREATE TABLE ChiTietPhieuDenBu (
    SoPhieuDenBu VARCHAR(10) NOT NULL REFERENCES PhieuDenBu(SoPhieuDenBu) ON DELETE CASCADE,
    MaTienNghi VARCHAR(10) NOT NULL REFERENCES TienNghi(MaTienNghi),
    MucDoThietHai NVARCHAR(50) NOT NULL,
    SoTien DECIMAL(18,0) NOT NULL CHECK (SoTien >= 0),
    PRIMARY KEY (SoPhieuDenBu, MaTienNghi)
);
GO

/* Dữ liệu mẫu */
INSERT INTO KhuVuc VALUES ('KV01', N'Tầng 1'), ('KV02', N'Tầng 2'), ('KV03', N'Tầng 3');
INSERT INTO NhanVien VALUES ('NV01', N'Nguyễn Thị Mai', N'Lễ tân', '0901000001'), ('NV02', N'Trần Văn Hùng', N'Kế toán', '0901000002'), ('NV03', N'Lê Quốc Việt', N'Buồng phòng', '0901000003');
INSERT INTO KhachHang VALUES ('KH01', N'Phạm Thanh Sơn', '079090000001', N'Việt Nam', '0912000001'), ('KH02', N'John Smith', 'P1234567', N'Mỹ', '0912000002');
INSERT INTO LoaiTienNghi VALUES ('TN01', N'Điều hòa'), ('TN02', N'Tivi'), ('TN03', N'Tủ lạnh');
INSERT INTO DichVu VALUES ('DV01', N'Giặt ủi', N'kg', 30000), ('DV02', N'Nước suối', N'chai', 10000), ('DV03', N'Ăn sáng', N'suất', 80000);
INSERT INTO Phong (SoPhong,MaKhuVuc,SoNguoiToiDa,DonGiaNgay) VALUES
 ('101','KV01',2,500000), ('102','KV01',3,700000), ('201','KV02',2,550000), ('202','KV02',4,900000), ('301','KV03',2,600000);
INSERT INTO TienNghi VALUES ('TN0001','TN01',1,N'Tốt'), ('TN0002','TN02',1,N'Tốt'), ('TN0003','TN03',1,N'Tốt');
INSERT INTO QuyDinhDenBu VALUES ('QD01','TN01',N'Hư nhẹ',300000), ('QD02','TN01',N'Hỏng nặng',5000000), ('QD03','TN02',N'Hỏng nặng',4000000), ('QD04','TN03',N'Hỏng nặng',3000000);
INSERT INTO PhieuLapDat VALUES ('LD01','TN0001','101',GETDATE(),N'Tốt','NV03',N'Lắp mới');
GO
