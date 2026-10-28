USE QuanLyNuocGiaiKhat;
GO
INSERT INTO NhaCungUng VALUES
 ('NCC01', N'Công ty Coca-Cola VN',  N'Thủ Đức, TP.HCM', '0281111111'),
 ('NCC02', N'Công ty Suntory PepsiCo', N'Bình Dương',      '0282222222');
INSERT INTO LoaiNuocGiaiKhat VALUES
 ('L01', N'Nước ngọt có gas', 'NCC01'),
 ('L02', N'Nước suối',        'NCC02'),
 ('L03', N'Nước trà',         'NCC02');
INSERT INTO NuocGiaiKhat VALUES
 ('NGK01','L01',N'Coca-Cola lon 330ml', N'Lon',  10000, 100),
 ('NGK02','L01',N'Sprite lon 330ml',    N'Lon',  10000,  50),
 ('NGK03','L02',N'Aquafina 500ml',      N'Chai',  5000,  20),
 ('NGK04','L03',N'Trà Ô Long 455ml',    N'Chai', 12000,   5);
INSERT INTO NhanVien VALUES
 ('NV01', N'Nguyễn Văn An',  '1999-03-10', N'Q.1',  '0901000001', N'Bán hàng'),
 ('NV02', N'Trần Thị Bình',  '1998-07-21', N'Q.3',  '0901000002', N'Thủ kho');
INSERT INTO KhachHang (MaKH,HoTen,DiaChi,SoDienThoai,LoaiKhach,NgayDangKy) VALUES
 ('KH01', N'Lê Văn Cường', N'Q.5',  '0912000001', 'QUEN',   '2026-01-15'),
 ('KH02', N'Phạm Thị Dung',N'Q.10', '0912000002', 'VANGLAI', NULL);
GO
