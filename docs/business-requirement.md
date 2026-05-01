# AI Powered EV Warranty Management System
*Phần mềm quản lý bảo hành và kinh doanh xe điện tích hợp AI*

## Actors
- **Customer**: Khách hàng (Người mua xe và sử dụng dịch vụ)
- **SC Staff**: Nhân viên trung tâm dịch vụ
- **SC Technician**: Kỹ thuật viên trung tâm dịch vụ
- **EVM Staff**: Nhân viên hãng sản xuất xe điện
- **Admin**: Quản trị viên hệ thống

---

## 0. Hệ thống Xác thực & Người dùng (Authentication)
- **Đăng nhập & Đăng ký**: Hỗ trợ đăng nhập bằng tài khoản nội bộ và OAuth (Google, Facebook...).
- **Khôi phục mật khẩu**: Chức năng quên mật khẩu qua Email (gửi mã OTP hoặc xác thực qua Google Email).
- **Phân quyền Role-based**: Kiểm soát truy cập chặt chẽ cho từng Actor.

---

## 1. Landing Page & Showroom (Cho mọi đối tượng)
- **Giới thiệu sản phẩm**: Hiển thị danh sách các dòng xe điện, thông số kỹ thuật, hình ảnh 360 độ.
- **Tin tức & Sự kiện**: Các chiến dịch ưu đãi, bài viết giới thiệu công nghệ AI trên xe.
- **Tra cứu bảo hành**: Khách có thể nhập số VIN để kiểm tra thời hạn bảo hành mà không cần đăng nhập.
- **Recommendation System**: Hệ thống gợi ý sản phẩm (xe, phụ kiện) thông minh dựa trên hành vi và sự quan tâm của người dùng.

---

## 2. Chức năng cho Khách hàng (Customer Portal)

### a. Quản lý cá nhân & Sản phẩm yêu thích
- **Yêu thích (Favorite)**: Lưu lại các mẫu xe quan tâm để theo dõi giá hoặc nhận thông báo.
- **Quản lý hồ sơ**: Cập nhật thông tin cá nhân, địa chỉ nhận hàng.

### b. Giỏ hàng & Đặt hàng
- **Giỏ hàng (Cart)**: Thêm/Bớt xe hoặc phụ kiện, tính toán tổng tiền.
- **Đặt hàng (Order)**: Tạo đơn mua xe/phụ kiện.
- **Theo dõi đơn hàng**: Trạng thái từ Chờ xác nhận → Đang chuẩn bị → Đang giao → Hoàn tất.

### c. Thanh toán & Hoá đơn
- **Thanh toán đa kênh**: Tích hợp các cổng thanh toán (VietQR, VNPAY, MoMo).
- **Hoá đơn điện tử (Invoice)**: Tự động xuất hoá đơn sau khi thanh toán thành công, có thể tải về file PDF.

### d. Quản lý xe sở hữu & Bảo hành
- **Danh sách xe**: Xem các xe đã mua và thông tin chi tiết từng xe.
- **Yêu cầu hỗ trợ**: Gửi yêu cầu tư vấn hoặc báo lỗi nhanh cho trung tâm dịch vụ.

### e. Đánh giá & Phản hồi (Rating & Review)
- **Đánh giá sản phẩm**: Khách hàng có thể viết nhận xét, đánh giá (rating) cho các dòng xe và phụ kiện đã trải nghiệm/đã mua.

---

## 3. Chức năng cho Trung tâm dịch vụ (SC Staff, SC Technician)
*(Giữ nguyên các chức năng cũ và bổ sung thêm kết nối với đơn hàng từ khách)*

### a. Quản lý hồ sơ xe & khách hàng
- **Đăng ký xe**: Theo số VIN.
- **Quản lý phụ tùng**: Gắn số seri phụ tùng lắp trên xe.
- **Lịch sử**: Lưu trữ lịch sử dịch vụ và bảo hành.

### b. Xử lý yêu cầu bảo hành
- **Tạo yêu cầu**: Khởi tạo warranty claim gửi lên hãng.
- **Tài liệu đính kèm**: Báo cáo kiểm tra, hình ảnh, thông tin chẩn đoán.
- **Theo dõi trạng thái**: Đã gửi → Chờ duyệt → Được chấp nhận → Đã xử lý.

---

## 4. Chức năng cho Hãng sản xuất xe (EVM Staff, Admin)
*(Giữ nguyên các chức năng cũ và bổ sung quản lý kinh doanh)*

### a. Quản lý kinh doanh & Sản phẩm
- **Quản lý Inventory**: Theo dõi lượng xe và phụ tùng sẵn có trong kho hãng.
- **Quản lý đơn hàng**: Duyệt và điều phối đơn hàng từ khách hàng về các trung tâm phân phối/dịch vụ.

### b. Quản lý yêu cầu bảo hành & AI
- **Phê duyệt**: Tiếp nhận và xét duyệt các yêu cầu từ trung tâm dịch vụ.
- **AI Analytics**: Sử dụng AI để phân tích nguyên nhân lỗi phổ biến và dự báo hỏng hóc từ dữ liệu vận hành.

