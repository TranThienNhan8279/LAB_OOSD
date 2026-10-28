# LAB7 - Quản lý nhân sự

- Họ tên: Trần Thiện Nhân
- MSSV: 1250080123
- Lớp: 12_ĐH_CNPM2

## Nội dung
Bài 7 - quản lý nhân sự. Làm đủ 6 câu UML, thiết kế CSDL SQL Server từ mô hình lớp, ứng dụng WinForms (C#, .NET Framework 4.7.2, kiến trúc Forms - Services - Data) và test case.

## Thư mục
| Thư mục | Nội dung |
|---|---|
| `bao_cao/` | File Word trả lời 6 câu |
| `diagrams/` | `LAB7_UML.drawio` (5 trang: class, use case tổng quát, use case phân rã, activity, sequence), `ERD.drawio` và ảnh PNG |
| `sql/` | `01_schema.sql` tạo CSDL, `02_seed.sql` dữ liệu mẫu, `03_test.sql` test ràng buộc |
| `src/QuanLyNhanSu/` | Mã nguồn WinForms |
| `test/TestCases.md` | Test case và bảng truy vết |

## Cách chạy
1. Trong SSMS chạy `sql/01_schema.sql`, rồi `sql/02_seed.sql`.
2. Sửa `Server=` trong `src/QuanLyNhanSu/QuanLyNhanSu/App.config` cho đúng máy.
3. Mở `src/QuanLyNhanSu/QuanLyNhanSu.sln` bằng Visual Studio 2022 và nhấn F5.
4. Chạy `sql/03_test.sql` để kiểm tra ràng buộc, sau đó làm theo `test/TestCases.md`.

## Giả định
- Thử việc tối đa 60 ngày; nhân viên phải đủ 18 tuổi.
- Bảo hiểm trích 10,5% lương (8% BHXH + 1,5% BHYT + 1% BHTN).
- Lương giờ = lương / (26 ngày x 8 giờ); làm thêm tối đa 4 giờ mỗi ngày, hệ số 1.5.
- Khen thưởng tập thể được chia đều cho nhân viên đang làm của phòng ban.
- Tiền thưởng, tiền phạt tính vào tháng của ngày lập phiếu chi, phiếu thu.
- Nhân viên chưa có hợp đồng thì tính lương thử việc, không có phụ cấp và bảo hiểm.
