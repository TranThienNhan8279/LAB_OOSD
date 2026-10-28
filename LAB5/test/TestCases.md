# LAB5 – Test case và truy vết

Dữ liệu nền: chạy `sql/02_seed.sql` (ngày trong dữ liệu tính theo ngày hiện tại).
Khách: KH01, KH02 là khách lẻ; KH03, KH04 là khách đoàn. Chuyến CD01 (T01) và CD02 (T02) cùng khởi hành sau 10 ngày; CD03 đã đi xong. PD0001 là đoàn đã đi xong cách đây 20 ngày, mới đặt cọc 15.750.000.
Cột "Thực tế" ghi lại khi chạy trên máy cá nhân.

## 1. Test case ứng dụng

| TC | Chức năng | Dữ liệu vào | Kết quả mong đợi | Thực tế |
|---|---|---|---|---|
| TC-01 | Khách lẻ đăng ký hợp lệ | KH01, chuyến CD01, 4 người | Tạo phiếu PL0001, số tiền = 4 × 5.000.000 = 20.000.000 | |
| TC-02 | Khách lẻ đăng ký đúng 12 người | KH02, CD01, 12 người | Chấp nhận (khách lẻ tối đa 12 người) | |
| TC-03 | Khách lẻ đăng ký 13 người | KH01, CD01, 13 người | Báo lỗi, yêu cầu đăng ký theo đoàn | |
| TC-04 | Khách đoàn đăng ký không bảo hiểm | KH04, tour T01, 20 người, đi sau 14 ngày | Tạo phiếu PD, tiền cọc = 30% × 20 × 5.000.000 = 30.000.000, có phiếu thanh toán "Đặt cọc" | |
| TC-05 | Khách đoàn dưới hoặc bằng 12 người | KH04, T01, 10 người | Báo lỗi "phải trên 12 người" | |
| TC-06 | Đoàn có bảo hiểm thiếu danh sách | KH04, T01, 15 người, có bảo hiểm, nhập 10 người | Báo lỗi, không tạo phiếu (rollback) | |
| TC-07 | Đoàn có bảo hiểm đủ danh sách | KH04, T01, 13 người, nhập đủ 13 người | Tạo phiếu và 13 dòng người đi cùng | |
| TC-08 | Khách lẻ đăng ký theo đoàn | KH01 đăng ký đoàn | Báo lỗi "đây là khách lẻ" | |
| TC-09 | Phân công chuyến | CD01 → NV01 | Thành công | |
| TC-10 | Phân công trùng lịch | CD02 → NV01 (CD01 và CD02 trùng ngày) | Báo lỗi trùng lịch | |
| TC-11 | Phân công nhiều HDV cho đoàn | PD0001 → NV01, rồi → NV02 | Cả 2 đều thành công (đoàn đông có nhiều nhân viên) | |
| TC-12 | Phân công trùng nhân viên cùng đoàn | PD0001 → NV01 lần 2 | Báo lỗi đã được phân công | |
| TC-13 | Quyết toán đoàn đã kết thúc | PD0001 | Thu thêm 15 × 3.500.000 − 15.750.000 = 36.750.000, trạng thái "Đã quyết toán" | |
| TC-14 | Quyết toán đoàn chưa kết thúc | Đoàn đi sau 14 ngày | Báo lỗi tour chưa kết thúc | |
| TC-15 | Quyết toán lần 2 | PD0001 | Báo lỗi đã quyết toán | |
| TC-16 | Tính lương có tour trong tháng | NV01, tháng/năm của chuyến CD03 (30 ngày trước) | Lương = 8.000.000 + 700.000 = 8.700.000 | |
| TC-17 | Tính lương tháng không có tour | NV02, tháng không có tour | Lương = lương căn bản 8.000.000 | |
| TC-18 | Tính lại lương cùng tháng | NV01, cùng tháng TC-16 | Cập nhật dòng cũ, không thêm dòng mới | |
| TC-19 | Khảo sát sau tour | KH01, chuyến CD03, điểm 5 | Tạo phiếu khảo sát | |
| TC-20 | Khảo sát khi tour chưa kết thúc | KH01, chuyến CD01 | Báo lỗi chỉ gửi sau khi tour kết thúc | |

## 2. Test case CSDL (`sql/03_test.sql`)

| TC | Ràng buộc | Mong đợi |
|---|---|---|
| TC-DB01 | Khách lẻ tối đa 12 người | PASS |
| TC-DB02 | Khách đoàn trên 12 người | PASS |
| TC-DB03 | Ngày về không trước ngày đi | PASS |
| TC-DB04 | Khách sạn 2–5 sao | PASS |
| TC-DB05 | Khảo sát thuộc đúng một chuyến hoặc một đoàn | PASS |
| TC-DB06 | Không phân công trùng nhân viên cho cùng đoàn | PASS |
| TC-DB07 | Mỗi nhân viên một dòng lương mỗi tháng | PASS |

## 3. Truy vết

| Yêu cầu | UML (diagrams/LAB5_UML.drawio) | Form → Service | Bảng | Test |
|---|---|---|---|---|
| Thông tin tour, nơi dừng chân, điểm tham quan | Câu 1, 2, 3 (UC01–UC03) | FrmTour → DanhMucService | Tour, DiemDung, PhuongTien, DiemThamQuan, Tour_DiemThamQuan | TC-DB04 |
| Chuyến đi theo lịch công ty | Câu 3 (UC04) | FrmTour → DanhMucService | ChuyenDi | TC-DB03 |
| Khách lẻ đăng ký theo chuyến (tối đa 12 người) | Câu 4, 5 | FrmDangKy → DangKyService.DangKyChuyen | PhieuDangKyChuyen | TC-01, 02, 03, 08, DB01 |
| Khách đoàn trên 12 người, đặt cọc, danh sách bảo hiểm | Câu 4, 5, 6 (Sequence) | FrmDangKy → DangKyService.DangKyDoan | PhieuDangKyDoan, NguoiDiCung, PhieuThanhToanDoan | TC-04, 05, 06, 07, DB02 |
| Phân công HDV không chồng chéo | Câu 4, 5 | FrmPhanCong → PhanCongService | ChuyenDi, PhanCongDoan | TC-09…12, DB06 |
| Quyết toán đoàn sau tour | Câu 5 | FrmDangKy → DangKyService.QuyetToanDoan | PhieuThanhToanDoan | TC-13, 14, 15 |
| Lương = căn bản + theo tour | Câu 3 (UC11) | FrmLuongKhaoSat → LuongKhaoSatService.TinhLuong | LuongThang | TC-16, 17, 18, DB07 |
| Khảo sát sau tour | Câu 4, 5 | FrmLuongKhaoSat → LuongKhaoSatService.GuiKhaoSat | PhieuKhaoSat | TC-19, 20, DB05 |
