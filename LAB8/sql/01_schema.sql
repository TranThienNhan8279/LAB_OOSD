/* LAB8 - Cua hang nuoc giai khat - SQL Server
   Chay trong SSMS. Tao CSDL QuanLyNuocGiaiKhat (16 lop -> 14 bang, KhachQuen/KhachVangLai gop vao KhachHang). */
IF DB_ID(N'QuanLyNuocGiaiKhat') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyNuocGiaiKhat SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyNuocGiaiKhat;
END
GO
CREATE DATABASE QuanLyNuocGiaiKhat;
GO
USE QuanLyNuocGiaiKhat;
GO

CREATE TABLE NhaCungUng (
    MaNCC        VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenNCC       NVARCHAR(100) NOT NULL,
    DiaChi       NVARCHAR(200) NULL,
    SoDienThoai  VARCHAR(15)   NULL
);

-- NCC (1) cung ung (1..*) Loai
CREATE TABLE LoaiNuocGiaiKhat (
    MaLoai   VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenLoai  NVARCHAR(100) NOT NULL,
    MaNCC    VARCHAR(10)   NOT NULL REFERENCES NhaCungUng(MaNCC)
);

CREATE TABLE NuocGiaiKhat (
    MaNGK      VARCHAR(10)   NOT NULL PRIMARY KEY,
    MaLoai     VARCHAR(10)   NOT NULL REFERENCES LoaiNuocGiaiKhat(MaLoai),
    TenNGK     NVARCHAR(100) NOT NULL,
    DonViTinh  NVARCHAR(20)  NOT NULL,
    DonGiaBan  DECIMAL(18,0) NOT NULL CHECK (DonGiaBan >= 0),
    SoLuongTon INT           NOT NULL DEFAULT 0 CHECK (SoLuongTon >= 0)
);

CREATE TABLE NhanVien (
    MaNV        VARCHAR(10)   NOT NULL PRIMARY KEY,
    HoTen       NVARCHAR(100) NOT NULL,
    NgaySinh    DATE          NULL,
    DiaChi      NVARCHAR(200) NULL,
    SoDienThoai VARCHAR(15)   NULL,
    ChucVu      NVARCHAR(50)  NULL
);

-- Generalization KhachHang <- KhachQuen | KhachVangLai : 1 bang, cot phan loai
CREATE TABLE KhachHang (
    MaKH        VARCHAR(10)   NOT NULL PRIMARY KEY,
    HoTen       NVARCHAR(100) NOT NULL,
    DiaChi      NVARCHAR(200) NULL,
    SoDienThoai VARCHAR(15)   NULL,
    LoaiKhach   VARCHAR(10)   NOT NULL DEFAULT 'VANGLAI' CHECK (LoaiKhach IN ('QUEN','VANGLAI')),
    NgayDangKy  DATE          NULL,
    CONSTRAINT CK_KhachQuen_NgayDK CHECK (LoaiKhach = 'VANGLAI' OR NgayDangKy IS NOT NULL)
);

-- ===== Mua hang tu NCC =====
CREATE TABLE DonDatHang (
    MaDDH     VARCHAR(10)  NOT NULL PRIMARY KEY,
    MaNCC     VARCHAR(10)  NOT NULL REFERENCES NhaCungUng(MaNCC),
    MaNV      VARCHAR(10)  NOT NULL REFERENCES NhanVien(MaNV),
    NgayDat   DATE         NOT NULL,
    NgayNhan  DATE         NULL,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Chờ giao'
        CHECK (TrangThai IN (N'Chờ giao', N'Đang giao', N'Đã giao đủ'))
);
CREATE TABLE ChiTietDonDatHang (
    MaCTDDH    VARCHAR(10) NOT NULL PRIMARY KEY,
    MaDDH      VARCHAR(10) NOT NULL REFERENCES DonDatHang(MaDDH),
    MaNGK      VARCHAR(10) NOT NULL REFERENCES NuocGiaiKhat(MaNGK),
    SoLuongDat INT         NOT NULL CHECK (SoLuongDat > 0),
    CONSTRAINT UQ_CTDDH UNIQUE (MaDDH, MaNGK)
);
-- 1 don dat hang giao toi da 3 dot
CREATE TABLE PhieuGiaoHang (
    MaPGH    VARCHAR(10)   NOT NULL PRIMARY KEY,
    MaDDH    VARCHAR(10)   NOT NULL REFERENCES DonDatHang(MaDDH),
    DotGiao  TINYINT       NOT NULL CHECK (DotGiao BETWEEN 1 AND 3),
    NgayGiao DATE          NOT NULL,
    TongTien DECIMAL(18,0) NOT NULL DEFAULT 0,
    CONSTRAINT UQ_PGH_Dot UNIQUE (MaDDH, DotGiao)
);
CREATE TABLE ChiTietPhieuGiao (
    MaCTPGH    VARCHAR(10)   NOT NULL PRIMARY KEY,
    MaPGH      VARCHAR(10)   NOT NULL REFERENCES PhieuGiaoHang(MaPGH),
    MaNGK      VARCHAR(10)   NOT NULL REFERENCES NuocGiaiKhat(MaNGK),
    SoLuongGiao INT          NOT NULL CHECK (SoLuongGiao > 0),
    DonGiaNhap DECIMAL(18,0) NOT NULL CHECK (DonGiaNhap >= 0),
    ThanhTien  AS (SoLuongGiao * DonGiaNhap) PERSISTED,
    CONSTRAINT UQ_CTPGH UNIQUE (MaPGH, MaNGK)
);

-- ===== Ban hang =====
CREATE TABLE HoaDon (
    MaHD          VARCHAR(10)   NOT NULL PRIMARY KEY,
    MaKH          VARCHAR(10)   NOT NULL REFERENCES KhachHang(MaKH),
    MaNV          VARCHAR(10)   NOT NULL REFERENCES NhanVien(MaNV),
    NgayLap       DATE          NOT NULL,
    TongTien      DECIMAL(18,0) NOT NULL DEFAULT 0,
    TrangThaiGiao NVARCHAR(30)  NOT NULL DEFAULT N'Đã giao đủ'
        CHECK (TrangThaiGiao IN (N'Đã giao đủ', N'Còn thiếu hàng'))
);
CREATE TABLE ChiTietHoaDon (
    MaCTHD        VARCHAR(10)   NOT NULL PRIMARY KEY,
    MaHD          VARCHAR(10)   NOT NULL REFERENCES HoaDon(MaHD),
    MaNGK         VARCHAR(10)   NOT NULL REFERENCES NuocGiaiKhat(MaNGK),
    SoLuong       INT           NOT NULL CHECK (SoLuong > 0),
    DonGia        DECIMAL(18,0) NOT NULL CHECK (DonGia >= 0),
    ThanhTien     AS (SoLuong * DonGia) PERSISTED,
    SoLuongDaGiao INT           NOT NULL DEFAULT 0,
    CONSTRAINT CK_CTHD_DaGiao CHECK (SoLuongDaGiao BETWEEN 0 AND SoLuong),
    CONSTRAINT UQ_CTHD UNIQUE (MaHD, MaNGK)
);
CREATE TABLE PhieuHenGiao (
    MaPhieuHen  VARCHAR(10)  NOT NULL PRIMARY KEY,
    MaHD        VARCHAR(10)  NOT NULL REFERENCES HoaDon(MaHD),
    NgayLap     DATE         NOT NULL,
    NgayHenGiao DATE         NOT NULL,
    TrangThai   NVARCHAR(30) NOT NULL DEFAULT N'Chờ giao'
        CHECK (TrangThai IN (N'Chờ giao', N'Đã giao')),
    CONSTRAINT CK_Hen_Ngay CHECK (NgayHenGiao >= NgayLap)
);
CREATE TABLE ChiTietPhieuHen (
    MaCTPH     VARCHAR(10) NOT NULL PRIMARY KEY,
    MaPhieuHen VARCHAR(10) NOT NULL REFERENCES PhieuHenGiao(MaPhieuHen),
    MaNGK      VARCHAR(10) NOT NULL REFERENCES NuocGiaiKhat(MaNGK),
    SoLuongHen INT         NOT NULL CHECK (SoLuongHen > 0),
    CONSTRAINT UQ_CTPH UNIQUE (MaPhieuHen, MaNGK)
);

-- ===== Ghi no (chi khach quen; moi hoa don toi da 1 phieu no) =====
CREATE TABLE PhieuGhiNo (
    MaPhieuNo     VARCHAR(10)   NOT NULL PRIMARY KEY,
    MaKH          VARCHAR(10)   NOT NULL REFERENCES KhachHang(MaKH),
    MaNV          VARCHAR(10)   NOT NULL REFERENCES NhanVien(MaNV),
    MaHD          VARCHAR(10)   NOT NULL REFERENCES HoaDon(MaHD),
    NgayLap       DATE          NOT NULL,
    SoTienNo      DECIMAL(18,0) NOT NULL CHECK (SoTienNo > 0),
    NgayDuKienTra DATE          NOT NULL,
    LyDoNo        NVARCHAR(200) NULL,
    CONSTRAINT UQ_No_HoaDon UNIQUE (MaHD),
    CONSTRAINT CK_No_Ngay CHECK (NgayDuKienTra >= NgayLap)
);
GO
