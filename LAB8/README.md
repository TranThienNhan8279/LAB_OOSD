# LAB8 – Quản lý cửa hàng nước giải khát

- Họ tên: Trần Thiện Nhân
- MSSV: 1250080123
- Lớp: 12_ĐH_CNPM2
- Môn: Phân tích thiết kế hướng đối tượng

## Nội dung
Phân tích, thiết kế UML, CSDL SQL Server và ứng dụng WinForms (C#, .NET Framework 4.7.2, kiến trúc Forms → Services → Data).

Quy tắc chính: bán hàng theo tồn kho, thiếu hàng thì lập phiếu hẹn giao (nhiều đợt), đơn đặt hàng nhà cung ứng giao tối đa 3 đợt, chỉ khách quen được ghi nợ, mỗi hóa đơn tối đa 1 phiếu ghi nợ.

## Thư mục
| Thư mục | Nội dung |
|---|---|
| `bao_cao/` | File Word trả lời 6 câu thực hành |
| `diagrams/` | `Bai2_UML.drawio` (5 trang: use case tổng quát, class, use case phân rã, activity, sequence), `ERD.drawio`, ảnh PNG |
| `sql/` | `01_schema.sql` tạo CSDL, `02_seed.sql` dữ liệu mẫu, `03_test.sql` test ràng buộc |
| `src/CuaHangNuocGiaiKhat/` | Mã nguồn WinForms |
| `test/TestCases.md` | Test case và bảng truy vết yêu cầu → UML → Form/Service → bảng → test |

## Cách chạy
1. Trong SSMS chạy lần lượt `sql/01_schema.sql`, `sql/02_seed.sql`.
2. Sửa `Server=` trong `src/CuaHangNuocGiaiKhat/App.config` cho đúng máy.
3. Mở `CuaHangNuocGiaiKhat.csproj` bằng Visual Studio 2022, nhấn F5.
4. Kiểm thử: chạy `sql/03_test.sql` (kết quả PASS/FAIL), rồi làm theo `test/TestCases.md`.

## Giả định
- Đợt giao thứ 3 của đơn đặt hàng phải giao đủ phần còn lại.
- Khách quen và khách vãng lai gộp vào bảng `KhachHang` (cột `LoaiKhach`).
