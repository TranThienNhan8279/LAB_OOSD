# LAB5 - Quản lý công ty du lịch

- Họ tên: Trần Thiện Nhân
- MSSV: 1250080123
- Lớp: 12_ĐH_CNPM2

## Nội dung
Bài 6: làm 6 câu (class diagram, use case tổng quát, use case phân rã, activity, sequence), thiết kế CSDL SQL Server, làm app WinForms C# và viết test case.

## File
- `bao_cao/`: file Word bài làm
- `diagrams/`: các sơ đồ draw.io, ERD và ảnh png
- `sql/`: `01_schema.sql` tạo bảng, `02_seed.sql` dữ liệu mẫu, `03_test.sql` để test
- `src/QuanLyDuLich/`: code
- `test/TestCases.md`: test case

## Cách chạy
- Chạy `01_schema.sql` rồi `02_seed.sql` trong SSMS
- Sửa `Server=` trong `App.config` cho đúng máy
- Mở file `.sln` bằng Visual Studio rồi F5

## Một số chỗ đề không nói rõ nên em tự quy ước
- Khách lẻ tối đa 12 người, trên 12 người tính là khách đoàn.
- Đoàn đặt cọc 30% tổng tiền.
- Lương theo tour tính vào tháng của ngày đi.
