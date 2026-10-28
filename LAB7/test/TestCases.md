# LAB7 – Test case và truy vết

Dữ liệu nền: chạy `sql/02_seed.sql` (ngày tính theo ngày hiện tại).
- NV0001: chính thức, hợp đồng dài hạn HD0001 (12.000.000), có bảo hiểm, chức vụ Trưởng phòng (phụ cấp 3.000.000).
- NV0002: chính thức, hợp đồng ngắn hạn HD0002 (8.000.000).
- NV0003: đang thử việc (còn 20 ngày). NV0004: thử việc đã hết hạn, chưa ký hợp đồng.
- Tháng trước có sẵn phiếu phụ trội (NV0001 3 giờ, NV0002 2 giờ), thưởng cá nhân 500.000 cho NV0002 và phạt 200.000 của NV0001.
Cột "Thực tế" ghi lại khi chạy trên máy cá nhân.

## 1. Test case ứng dụng

| TC | Chức năng | Dữ liệu vào | Kết quả mong đợi | Thực tế |
|---|---|---|---|---|
| TC-01 | Tuyển dụng hợp lệ | Họ tên, ngày sinh đủ 18 tuổi, PB02, thử việc 30 ngày, lương 6.000.000 | Tạo nhân viên "Thử việc" và quyết định tuyển dụng | |
| TC-02 | Thử việc quá dài | Thử việc 90 ngày | Báo lỗi không quá 60 ngày | |
| TC-03 | Ngày kết thúc trước ngày bắt đầu | Kết thúc < bắt đầu | Báo lỗi | |
| TC-04 | Nhân viên chưa đủ 18 tuổi | Ngày sinh cách đây 16 năm | Báo lỗi | |
| TC-05 | Ký hợp đồng khi chưa hết thử việc | NV0003 | Báo lỗi chưa hết thời gian thử việc | |
| TC-06 | Ký hợp đồng dài hạn sau thử việc | NV0004, dài hạn, bắt đầu hôm nay, lương 9.000.000 | Tạo hợp đồng, nhân viên thành "Chính thức" | |
| TC-07 | Ký hợp đồng ngắn hạn thiếu ngày kết thúc | NV0004, ngắn hạn, ngày kết thúc ≤ ngày bắt đầu | Báo lỗi | |
| TC-08 | Ký khi đang có hợp đồng | NV0001 | Báo lỗi còn hợp đồng hiệu lực | |
| TC-09 | Quyết định chức vụ | NV0002, CV02 | Tạo quyết định | |
| TC-10 | Quyết định chức vụ cho người thử việc | NV0003 | Báo lỗi chỉ áp dụng nhân viên chính thức | |
| TC-11 | Quyết định lương | NV0002, 9.000.000 | Tạo quyết định | |
| TC-12 | Quyết định lương không hợp lệ | Mức lương 0 | Báo lỗi | |
| TC-13 | Bảo hiểm hợp đồng dài hạn | HD ở TC-06 | Tạo bảo hiểm, tỷ lệ 10,5% | |
| TC-14 | Bảo hiểm hợp đồng ngắn hạn | HD0002 | Báo lỗi chỉ hợp đồng dài hạn | |
| TC-15 | Lập kế hoạch tuần | PB02, tuần bắt đầu thứ Hai tuần sau | Tạo kế hoạch | |
| TC-16 | Kế hoạch không bắt đầu thứ Hai | Chọn ngày thứ Tư | Báo lỗi | |
| TC-17 | Kế hoạch trùng tuần | PB01, tuần này | Báo lỗi đã có kế hoạch | |
| TC-18 | Phân công nhân viên khác phòng | KH0001 (PB01), NV0003 | Báo lỗi không thuộc phòng ban | |
| TC-19 | Phân công trùng ca | KH0001, NV0001, ngày và ca của CP0001 | Báo lỗi đã được phân công | |
| TC-20 | Chấm công | NV0002, hôm nay, 8 giờ | Thành công | |
| TC-21 | Chấm công lần 2 trong ngày | Như TC-20 | Báo lỗi đã chấm công | |
| TC-22 | Chấm công 25 giờ | 25 | Báo lỗi | |
| TC-23 | Phiếu phụ trội hợp lệ | PB01, hôm nay, NV0001 2 giờ, NV0002 1 giờ | Tạo phiếu hệ số 1.5 và 2 dòng chi tiết | |
| TC-24 | Phụ trội quá 4 giờ | NV0001 5 giờ | Báo lỗi, không tạo phiếu | |
| TC-25 | Phụ trội nhân viên khác phòng | PB01, NV0003 | Báo lỗi | |
| TC-26 | Khen thưởng cá nhân | NV0001, 1.000.000 | Tạo quyết định và phiếu chi cùng số tiền | |
| TC-27 | Khen thưởng tập thể | PB02, 2.000.000 | Tạo quyết định (MaPB) và phiếu chi | |
| TC-28 | Khen thưởng số tiền 0 | NV0001, 0 | Báo lỗi | |
| TC-29 | Kỷ luật phạt tiền | NV0002, phạt 100.000 | Tạo quyết định và phiếu thu 100.000 | |
| TC-30 | Kỷ luật khiển trách | NV0002, khiển trách | Tạo quyết định, không có phiếu thu | |
| TC-31 | Phạt tiền thiếu số tiền | NV0002, phạt tiền, 0 | Báo lỗi | |
| TC-32 | Bảng lương tháng trước | Tháng trước của ngày chạy | NV0001: 12.000.000 + phụ cấp 3.000.000 + phụ trội 259.615 − bảo hiểm 1.260.000 − phạt 200.000 = 13.799.615 | |
| TC-33 | Bảng lương, nhân viên không bảo hiểm | Như TC-32, dòng NV0002 | 8.000.000 + phụ trội 115.385 + thưởng 500.000 = 8.615.385 | |
| TC-34 | Tính lại cùng tháng | Chạy lại TC-32 | Không thêm dòng, kết quả không đổi | |
| TC-35 | Tháng chưa tới | Tháng sau | Báo lỗi | |

## 2. Test CSDL

Chạy `sql/03_test.sql`, mỗi dòng kết quả là PASS hoặc FAIL.

| TC | Nội dung |
|---|---|
| TC-DB01 | Hợp đồng dài hạn không có ngày kết thúc |
| TC-DB02 | Hợp đồng ngắn hạn phải có ngày kết thúc |
| TC-DB03 | Khen thưởng cá nhân phải có nhân viên, tập thể phải có phòng ban |
| TC-DB04 | Phạt tiền phải có số tiền |
| TC-DB05 | Giờ phụ trội tối đa 4 giờ |
| TC-DB06 | Hệ số phụ trội không thấp hơn 1.5 |
| TC-DB07 | Mỗi ngày một phiếu chấm công |
| TC-DB08 | Không phân công trùng ca |
| TC-DB09 | Mỗi hợp đồng một thông tin bảo hiểm |

## 3. Truy vết

| Yêu cầu | UML (diagrams/LAB7_UML.drawio) | Form → Service | Bảng | Test |
|---|---|---|---|---|
| Tuyển dụng, thử việc, điều khoản | Câu 3 (UC02), Câu 4, 5 | FrmNhanVien → NhanSuService.TuyenDung | NhanVien, QuyetDinhTuyenDung | TC-01…04 |
| Hợp đồng ngắn hạn, dài hạn sau thử việc | Câu 3 (UC03), Câu 4, 5 | FrmNhanVien → NhanSuService.KyHopDong | HopDong | TC-05…08, DB01, DB02 |
| Quyết định chức vụ, phụ cấp, lương | Câu 3 (UC04, UC05) | FrmQuyetDinh → NhanSuService | QuyetDinhChucVu, ChucVu, QuyetDinhLuong | TC-09…12 |
| Bảo hiểm chỉ hợp đồng dài hạn | Câu 3 (UC06) | FrmQuyetDinh → NhanSuService.DangKyBaoHiem | BaoHiem | TC-13, 14, DB09 |
| Kế hoạch phân công tuần | Câu 3 (UC07) | FrmChamCong → ChamCongService | KeHoachTuan, ChiTietPhanCong | TC-15…19, DB08 |
| Chấm công | Câu 3 (UC08) | FrmChamCong → ChamCongService.ChamCong | PhieuChamCong | TC-20…22, DB07 |
| Phiếu phụ trội hệ số 1.5 | Câu 3 (UC09) | FrmChamCong → ChamCongService.LapPhieuPhuTroi | PhieuPhuTroi, ChiTietPhuTroi | TC-23…25, DB05, DB06 |
| Khen thưởng và phiếu chi | Câu 3 (UC10, UC11), Câu 6 | FrmThuongPhat → ThuongPhatService.LapKhenThuong | QuyetDinhKhenThuong, PhieuChi | TC-26, 27, 28, DB03 |
| Kỷ luật và phiếu thu | Câu 3 (UC12, UC13) | FrmThuongPhat → ThuongPhatService.LapKyLuat | QuyetDinhKyLuat, PhieuThu | TC-29, 30, 31, DB04 |
| Bảng lương tháng | Câu 3 (UC14) | FrmBangLuong → LuongService | BangLuong | TC-32…35 |
