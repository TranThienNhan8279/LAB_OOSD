# LAB8 – Test case và truy vết

Dữ liệu nền: chạy `sql/02_seed.sql`. Tồn đầu: NGK01 = 100 (10.000đ), NGK02 = 50 (10.000đ), NGK03 = 20 (5.000đ), NGK04 = 5 (12.000đ).
KH01 = khách quen, KH02 = khách vãng lai. Các test chạy theo thứ tự trên cùng một CSDL mới.
Kết quả thực tế ghi ở cột cuối khi chạy trên máy cá nhân (chụp ảnh đưa vào `bao_cao/`).

## 1. Test case ứng dụng (Form → Service)

| TC | Chức năng | Dữ liệu vào | Kết quả mong đợi | Thực tế |
|---|---|---|---|---|
| TC-01 | Bán đủ hàng | KH02 mua 10 NGK01 | HD0001 tổng 100.000, "Đã giao đủ", tồn NGK01 = 90, không có phiếu hẹn | |
| TC-02 | Bán thiếu hàng, có ngày hẹn | KH01 mua 8 NGK04, hẹn sau 3 ngày | HD0002 tổng 96.000, "Còn thiếu hàng", đã giao 5, tồn NGK04 = 0, PH0001 hẹn 3 chai | |
| TC-03 | Thiếu hàng, không chọn ngày hẹn | KH02 mua 30 NGK03 (tồn 20), bỏ tick ngày hẹn | Báo lỗi "cần chọn ngày hẹn giao", không tạo hóa đơn, tồn NGK03 vẫn 20 (rollback) | |
| TC-04 | Giao hẹn một phần (nhiều đợt) | Đặt và nhận 2 NGK04 qua FrmMuaHang (tồn = 2), giao PH0001, chọn ngày hẹn mới | Giao 2, PH0001 "Đã giao", tạo PH0002 còn thiếu 1, HD0002 vẫn "Còn thiếu hàng", tồn NGK04 = 0 | |
| TC-05 | Giao hẹn đủ hàng | Đặt và nhận thêm 10 NGK04 (tồn = 10), giao PH0002 | PH0002 "Đã giao", HD0002 "Đã giao đủ", đã giao 8/8, tồn NGK04 = 9 | |
| TC-06 | Giao hẹn khi tồn = 0 | Giao phiếu hẹn khi tồn kho = 0 | Báo lỗi "Tồn kho chưa có hàng để giao" | |
| TC-07 | Lập đơn đặt hàng | NCC01, NV02: NGK01 × 300, NGK02 × 100 | DDH mới trạng thái "Chờ giao" | |
| TC-08 | Giao đợt 1 | DDH mới: NGK01 × 100, giá nhập 8.000 | PGH đợt 1, tổng 800.000, tồn NGK01 +100, DDH "Đang giao" | |
| TC-09 | Giao vượt số đặt | Đợt 2: NGK01 × 250 (còn lại 200) | Báo lỗi vượt số còn lại | |
| TC-10 | Giao đủ trong 3 đợt | Đợt 2: NGK01 × 100, NGK02 × 50; đợt 3: NGK01 × 100, NGK02 × 50 | DDH mới "Đã giao đủ", có NgayNhan | |
| TC-11 | Đợt 3 chưa đủ | Làm lại trên đơn mới: đợt 3 giao thiếu | Báo lỗi "đợt 3 phải giao đủ" | |
| TC-12 | Đợt thứ 4 | Giao thêm cho đơn đã giao đủ | Báo lỗi (đã giao đủ / tối đa 3 đợt) | |
| TC-13 | Ghi nợ hợp lệ | HD0002 (KH01 quen), nợ 50.000, hạn sau 30 ngày | PN0001 được tạo | |
| TC-14 | Ghi nợ khách vãng lai | HD0001 (KH02) | Báo lỗi "Chỉ khách quen mới được ghi nợ" | |
| TC-15 | Ghi nợ lần 2 cùng hóa đơn | HD0002 lần nữa | Báo lỗi "đã có phiếu ghi nợ" | |
| TC-16 | Nợ vượt tổng hóa đơn | HD mới 100.000, nợ 150.000 | Báo lỗi vượt tổng tiền | |
| TC-17 | Số tiền nợ ≤ 0 | Nợ = 0 | Báo lỗi "phải > 0" | |
| TC-18 | Ngày trả trong quá khứ | Hạn = hôm qua | Báo lỗi ngày dự kiến trả | |
| TC-19 | Thêm NGK trùng mã | Mã NGK01 | Báo lỗi mã đã tồn tại | |
| TC-20 | Giá bán âm | Giá = -1 | Báo lỗi giá >= 0 | |

## 2. Test case CSDL (`sql/03_test.sql`)

| TC | Ràng buộc | Kết quả mong đợi |
|---|---|---|
| TC-DB01 | ThanhTien = SoLuong × DonGia, trừ tồn khi bán | PASS |
| TC-DB02 | Tồn kho không âm (CHECK) | PASS |
| TC-DB03 | Mỗi đơn đặt hàng tối đa 3 đợt giao (CHECK + UNIQUE) | PASS |
| TC-DB04 | Mỗi hóa đơn tối đa 1 phiếu ghi nợ (UNIQUE) | PASS |
| TC-DB05 | SoLuongDaGiao ≤ SoLuong (CHECK) | PASS |
| TC-DB06 | Khách quen bắt buộc có NgayDangKy (CHECK) | PASS |

## 3. Ma trận truy vết

| Yêu cầu nghiệp vụ | UML (diagrams/Bai2_UML.drawio) | Form → Service | Bảng CSDL | Test |
|---|---|---|---|---|
| Quản lý danh mục nhãn hiệu, NCC, nước giải khát, khách hàng | Câu 1 Use Case; Câu 2–3 Class | FrmDanhMuc → DanhMucService | LoaiNuocGiaiKhat, NhaCungUng, NuocGiaiKhat, KhachHang, NhanVien | TC-19, TC-20, TC-DB06 |
| Đặt hàng nhà cung ứng | Câu 4 Use Case phân rã | FrmMuaHang → MuaHangService.TaoDonDatHang | DonDatHang, ChiTietDonDatHang | TC-07 |
| Giao hàng tối đa 3 đợt, tăng tồn | Câu 3 Class (DonDatHang–PhieuGiaoHang 1–0..3) | FrmMuaHang → MuaHangService.LapPhieuGiao | PhieuGiaoHang, ChiTietPhieuGiao, NuocGiaiKhat | TC-08…TC-12, TC-DB03 |
| Bán hàng theo tồn kho | Câu 4, Câu 5 Activity | FrmBanHang → BanHangService.LapHoaDon | HoaDon, ChiTietHoaDon, NuocGiaiKhat | TC-01, TC-02, TC-03, TC-DB01, TC-DB02 |
| Phiếu hẹn giao nhiều đợt | Câu 4, Câu 5 Activity | FrmPhieuHen → BanHangService.GiaoHangHen | PhieuHenGiao, ChiTietPhieuHen, ChiTietHoaDon | TC-04, TC-05, TC-06, TC-DB05 |
| Phiếu ghi nợ cho khách quen | Câu 3 Class (KhachQuen); Câu 6 Sequence | FrmGhiNo → GhiNoService.LapPhieuGhiNo | PhieuGhiNo, KhachHang | TC-13…TC-18, TC-DB04 |
