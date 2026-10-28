# LAB6 - Quản lý hồ sơ bệnh án của bệnh nhân

- Họ tên: Trần Thiện Nhân
- MSSV: 1250080123
- Lớp: 12_ĐH_CNPM2

## Nội dung
Bài 5 - quản lý hồ sơ bệnh án của bệnh nhân. Làm đủ 6 câu UML, thiết kế CSDL SQL Server từ mô hình lớp, ứng dụng WinForms (C#, .NET Framework 4.7.2, kiến trúc Forms - Services - Data) và test case.

## Thư mục
| Thư mục | Nội dung |
|---|---|
| `bao_cao/` | File Word trả lời 6 câu |
| `diagrams/` | `LAB6_UML.drawio` (5 trang: class, use case tổng quát, use case phân rã, activity, sequence), `ERD.drawio` và ảnh PNG |
| `sql/` | `01_schema.sql` tạo CSDL, `02_seed.sql` dữ liệu mẫu, `03_test.sql` test ràng buộc |
| `src/QuanLyBenhAn/` | Mã nguồn WinForms |
| `test/TestCases.md` | Test case và bảng truy vết |

## Cách chạy
1. Trong SSMS chạy `sql/01_schema.sql`, rồi `sql/02_seed.sql`.
2. Sửa `Server=` trong `src/QuanLyBenhAn/QuanLyBenhAn/App.config` cho đúng máy.
3. Mở `src/QuanLyBenhAn/QuanLyBenhAn.sln` bằng Visual Studio 2022 và nhấn F5.
4. Chạy `sql/03_test.sql` để kiểm tra ràng buộc, sau đó làm theo `test/TestCases.md`.

## Giả định
- Lệ phí quản lý hằng năm là 200.000đ (đề không nêu số tiền).
- Phải đóng lệ phí của năm hiện tại thì mới được lập phiếu khám và đặt lịch định kỳ.
- Khi khám định kỳ, phiếu khám được lưu ở trạng thái "Hoàn tất" và loại "Định kỳ", không thu phí.
- Thân nhiệt hợp lệ từ 30 đến 43 độ C.
