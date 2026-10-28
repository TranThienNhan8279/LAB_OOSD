# LAB5 - Quản lý công ty du lịch

- Họ tên: Trần Thiện Nhân
- MSSV: 1250080123
- Lớp: 12_ĐH_CNPM2

## Nội dung
Bài 6 - quản lý công ty du lịch. Làm đủ 6 câu UML, thiết kế CSDL SQL Server từ mô hình lớp, ứng dụng WinForms (C#, .NET Framework 4.7.2, kiến trúc Forms - Services - Data) và test case.

## Thư mục
| Thư mục | Nội dung |
|---|---|
| `bao_cao/` | File Word trả lời 6 câu |
| `diagrams/` | `LAB5_UML.drawio` (5 trang: class, use case tổng quát, use case phân rã, activity, sequence), `ERD.drawio` và ảnh PNG |
| `sql/` | `01_schema.sql` tạo CSDL, `02_seed.sql` dữ liệu mẫu, `03_test.sql` test ràng buộc |
| `src/QuanLyDuLich/` | Mã nguồn WinForms |
| `test/TestCases.md` | Test case và bảng truy vết |

## Cách chạy
1. Trong SSMS chạy `sql/01_schema.sql`, rồi `sql/02_seed.sql`.
2. Sửa `Server=` trong `src/QuanLyDuLich/QuanLyDuLich/App.config` cho đúng máy.
3. Mở `src/QuanLyDuLich/QuanLyDuLich.sln` bằng Visual Studio 2022 và nhấn F5.
4. Chạy `sql/03_test.sql` để kiểm tra ràng buộc, sau đó làm theo `test/TestCases.md`.

## Giả định
- Khách lẻ tối đa 12 người, khách đoàn trên 12 người (đề không nói rõ đúng 12 người).
- Tiền đặt cọc của đoàn là 30% tổng tiền (đề không nêu mức cọc).
- Lương theo tour tính vào tháng của ngày đi.
