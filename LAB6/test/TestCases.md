# LAB6 – Test case và truy vết

Dữ liệu nền: chạy `sql/02_seed.sql` (ngày tính theo ngày hiện tại).
Hồ sơ HS0001 (BN0001) đã đóng lệ phí năm nay; HS0002 (BN0002) chỉ đóng lệ phí năm ngoái.
PK0001 đã xuất viện, đã thanh toán, có toa, chưa ghi nhận. PK0002 và PK0003 đang điều trị (PK0002 chưa có toa).
Cột "Thực tế" ghi lại khi chạy trên máy cá nhân.

## 1. Test case ứng dụng

| TC | Chức năng | Dữ liệu vào | Kết quả mong đợi | Thực tế |
|---|---|---|---|---|
| TC-01 | Đăng ký lần đầu | Họ tên, ngày sinh, điện thoại hợp lệ | Tạo bệnh nhân, phiếu đăng ký, lệ phí năm nay 200.000 và hồ sơ mới | |
| TC-02 | Đăng ký thiếu họ tên | Họ tên trống | Báo lỗi, không tạo gì | |
| TC-03 | Ngày sinh ở tương lai | Ngày sinh sau hôm nay | Báo lỗi | |
| TC-04 | Đóng lệ phí năm nay | BN0002 | Thêm lệ phí năm hiện tại | |
| TC-05 | Đóng lệ phí lần 2 trong năm | BN0001 | Báo lỗi đã đóng | |
| TC-06 | Tra cứu theo mã hồ sơ | HS0001 | Hiện bệnh nhân BN0001 | |
| TC-07 | Tra cứu theo tên hoặc điện thoại | "Bình" hoặc 0912 | Hiện HS0002 | |
| TC-08 | Xem lịch sử khám | HS0001 | Hiện PK0003, PK0002, PK0001 (mới nhất trước) | |
| TC-09 | Lập phiếu khám | HS0001, BS01, K01, triệu chứng, 37.5 | Tạo phiếu mới, trạng thái "Đang điều trị" | |
| TC-10 | Lập phiếu khi chưa đóng lệ phí | HS0002 | Báo lỗi chưa đóng lệ phí năm nay | |
| TC-11 | Bác sĩ không thuộc khoa | BS02 với K01 | Báo lỗi | |
| TC-12 | Thân nhiệt không hợp lý | 50 | Báo lỗi | |
| TC-13 | Chỉ định xét nghiệm | PK0003, XN02 | Thêm dòng xét nghiệm chưa có kết quả | |
| TC-14 | Chỉ định cho phiếu đã hoàn tất | PK0001 | Báo lỗi phiếu đã hoàn tất | |
| TC-15 | Ghi kết quả xét nghiệm | XN0002, "Bình thường" | Cập nhật kết quả | |
| TC-16 | Lập toa hợp lệ | PK0002, 2 thuốc khác nhau, số lượng > 0 | Tạo toa và các dòng thuốc | |
| TC-17 | Toa không có thuốc | PK0003, danh sách rỗng | Báo lỗi | |
| TC-18 | Số lượng thuốc bằng 0 | PK0003, số lượng 0 | Báo lỗi, không tạo toa | |
| TC-19 | Phiếu đã có toa | PK0001 | Báo lỗi (phiếu đã hoàn tất) | |
| TC-20 | Thanh toán, xuất viện | PK0003, 500.000 | Tạo phiếu thanh toán, phiếu chuyển "Hoàn tất" | |
| TC-21 | Thanh toán lần 2 | PK0003 | Báo lỗi phiếu đã hoàn tất | |
| TC-22 | Ghi nhận sau xuất viện | PK0001, NV01 | Phiếu có người ghi nhận | |
| TC-23 | Ghi nhận khi chưa xuất viện | PK0002, NV01 | Báo lỗi chỉ ghi nhận sau xuất viện và thanh toán | |
| TC-24 | Đặt lịch định kỳ | HS0001, quý và ngày hẹn cùng quý trong tương lai | Tạo lịch "Đã hẹn", không thu phí | |
| TC-25 | Ngày hẹn không thuộc quý | Quý 1, ngày hẹn tháng 5 | Báo lỗi | |
| TC-26 | Đặt trùng quý | Đặt lại cùng quý, cùng năm | Báo lỗi đã có lịch | |
| TC-27 | Khám theo lịch hẹn | Lịch ở TC-24, BS01, K01 | Tạo phiếu "Định kỳ", lịch chuyển "Đã khám" | |
| TC-28 | Khám lại lịch đã khám | Lịch ở TC-27 | Báo lỗi lịch đã khám | |

## 2. Test CSDL

Chạy `sql/03_test.sql`, mỗi dòng kết quả là PASS hoặc FAIL.

| TC | Nội dung |
|---|---|
| TC-DB01 | Lệ phí phải lớn hơn 0 |
| TC-DB02 | Mỗi năm một lệ phí cho một bệnh nhân |
| TC-DB03 | Loại khám chỉ là Điều trị hoặc Định kỳ |
| TC-DB04 | Mỗi phiếu khám chỉ có một toa |
| TC-DB05 | Số lượng thuốc phải lớn hơn 0 |
| TC-DB06 | Quý từ 1 đến 4 |
| TC-DB07 | Xóa toa thì xóa chi tiết thuốc |

## 3. Truy vết

| Yêu cầu | UML (diagrams/LAB6_UML.drawio) | Form → Service | Bảng | Test |
|---|---|---|---|---|
| Phiếu đăng ký, lệ phí năm, cấp hồ sơ | Câu 1, 2, 3 (UC01–UC04) | FrmDangKy → HoSoService.DangKyLanDau | BenhNhan, PhieuDangKy, LePhi, HoSoBenhAn | TC-01…05, DB01, DB02 |
| Tra cứu hồ sơ bằng mã hoặc thông tin khác | Câu 3 (UC05), Câu 4 | FrmDangKy → HoSoService.TraCuu | HoSoBenhAn, BenhNhan | TC-06, 07, 08 |
| Phiếu khám: ngày giờ, bác sĩ, khoa, triệu chứng | Câu 3 (UC06) | FrmKhamBenh → KhamBenhService.LapPhieuKham | PhieuKham | TC-09…12, DB03 |
| Xét nghiệm | Câu 3 (UC07, UC08), Câu 4, 5 | FrmKhamBenh → KhamBenhService | XetNghiem, LoaiXetNghiem | TC-13, 14, 15 |
| Toa thuốc | Câu 6 (Sequence) | FrmKhamBenh → KhamBenhService.LapToaThuoc | ToaThuoc, ChiTietToaThuoc | TC-16…19, DB04, DB05, DB07 |
| Thanh toán viện phí, xuất viện | Câu 3 (UC10) | FrmXuatVien → XuatVienService.ThanhToanXuatVien | ThanhToanVienPhi | TC-20, 21 |
| Ghi nhận sau xuất viện và thanh toán | Câu 3 (UC11) | FrmXuatVien → XuatVienService.GhiNhanVaoHoSo | PhieuKham | TC-22, 23 |
| Khám định kỳ theo quý, miễn phí, có hẹn | Câu 3 (UC12, UC13) | FrmKhamDinhKy → XuatVienService | LichKhamDinhKy, PhieuKham | TC-24…28, DB06 |
