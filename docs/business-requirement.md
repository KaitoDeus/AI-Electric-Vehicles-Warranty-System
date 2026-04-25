# AI Powered EV Warranty Management System
*Phần mềm quản lý bảo hành xe điện từ hãng*

## Actors
- **SC Staff**: Nhân viên trung tâm dịch vụ
- **SC Technician**: Kỹ thuật viên trung tâm dịch vụ
- **EVM Staff**: Nhân viên hãng sản xuất xe điện
- **Admin**: Quản trị viên hệ thống

---

## 1. Chức năng cho Trung tâm dịch vụ (SC Staff, SC Technician)

### a. Quản lý hồ sơ xe & khách hàng
- **Đăng ký xe**: Theo số VIN.
- **Quản lý phụ tùng**: Gắn số seri phụ tùng lắp trên xe.
- **Lịch sử**: Lưu trữ lịch sử dịch vụ và bảo hành.

### b. Xử lý yêu cầu bảo hành
- **Tạo yêu cầu**: Khởi tạo warranty claim gửi lên hãng.
- **Tài liệu đính kèm**: Báo cáo kiểm tra, hình ảnh, thông tin chẩn đoán.
- **Theo dõi trạng thái**: Đã gửi → Chờ duyệt → Được chấp nhận → Đã xử lý.

### c. Thực hiện bảo hành
- **Nhận phụ tùng**: Tiếp nhận linh kiện thay thế từ hãng.
- **Quản lý tiến độ**: Theo dõi quá trình sửa chữa hoặc thay thế.
- **Bàn giao**: Cập nhật kết quả bảo hành và thực hiện bàn giao xe cho khách.

### d. Thực hiện chiến dịch từ hãng (Recall/Service Campaigns)
- **Tiếp nhận danh sách**: Nhận danh sách xe thuộc diện triệu hồi hoặc chiến dịch dịch vụ.
- **Thông báo & Lịch hẹn**: Gửi thông báo cho khách hàng, quản lý lịch hẹn.
- **Xử lý & Báo cáo**: Thực hiện xử lý theo hướng dẫn và báo cáo kết quả về hãng.

### e. Quản lý nội bộ
- **Phân công**: Giao việc cho kỹ thuật viên xử lý từng ca bảo hành.
- **Hiệu suất**: Theo dõi thời gian và hiệu quả xử lý.
- **Lưu trữ**: Hồ sơ bảo hành phục vụ công tác kiểm tra và báo cáo định kỳ.

---

## 2. Chức năng cho Hãng sản xuất xe (EVM Staff, Admin)

### a. Quản lý sản phẩm & phụ tùng
- **Cơ sở dữ liệu**: Quản lý bộ phận xe điện (Pin, Mô-tơ, BMS, Inverter, Bộ sạc...).
- **Định danh**: Liên kết số seri phụ tùng với số VIN của xe.
- **Chính sách**: Thiết lập và quản lý thời hạn, phạm vi, điều kiện bảo hành.

### b. Quản lý yêu cầu bảo hành
- **Phê duyệt**: Tiếp nhận và xét duyệt các yêu cầu từ trung tâm dịch vụ.
- **Luồng xử lý**: Tiếp nhận → Xác thực → Xử lý → Hoàn tất.
- **Tài chính**: Quản lý chi phí bảo hành do hãng chi trả.
- **Chiến dịch**: Khởi tạo và quản lý các chương trình Recall/Service campaign.

### c. Chuỗi cung ứng phụ tùng bảo hành
- **Tồn kho**: Quản lý kho linh kiện phục vụ bảo hành.
- **Phân bổ**: Điều phối phụ tùng thay thế cho các trung tâm dịch vụ.
- **Cảnh báo**: Tự động thông báo khi tồn kho thấp.

### d. Báo cáo & Phân tích
- **Thống kê**: Phân tích tỷ lệ hỏng hóc theo model, loại phụ tùng hoặc khu vực địa lý.
- **AI Analytics**: Sử dụng AI để phân tích nguyên nhân lỗi phổ biến và dự báo chi phí bảo hành trong tương lai.
