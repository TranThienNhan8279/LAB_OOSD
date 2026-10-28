# LAB3 – Hệ thống quản lý khách sạn

- Họ tên: Trần Thiện Nhân
- MSSV: 1250080123
- Lớp: 12_ĐH_CNPM2
- Môn: Phân tích thiết kế hướng đối tượng

## Nội dung
Ứng dụng WinForms (C#, .NET Framework 4.7.2) kết nối SQL Server: đặt phòng, tiện nghi, dịch vụ, trả phòng, hóa đơn, thanh toán, đền bù, thống kê.

## Thư mục
- `diagrams/ERD.drawio`, `ERD.png`: sơ đồ CSDL (vẽ bằng draw.io).
- `src/QuanLyHotel/`: mã nguồn (Forms → Services → Data).

## Cách chạy
1. Tạo CSDL `QuanLyHotelDB` trên SQL Server (script SQL chưa có trong repo, cần bổ sung).
2. Sửa chuỗi kết nối trong `src/QuanLyHotel/QuanLyHotel/App.config`.
3. Mở `QuanLyHotel.sln` bằng Visual Studio 2022, nhấn F5.

## Các Form
FrmMain, FrmDanhMuc, FrmPhongTienNghi, FrmDatPhong, FrmDichVu, FrmTraPhong, FrmThongKe.
