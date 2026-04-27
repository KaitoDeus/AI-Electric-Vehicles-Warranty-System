# 📋 EV Warranty System - Project Task Checklist

Dưới đây là lộ trình chi tiết của dự án, giúp theo dõi những gì đã xong và những gì cần làm tiếp theo.

## ✅ Giai đoạn 1: Khởi tạo & Thiết kế (Hoàn thành)
- [x] Phân tích nghiệp vụ hệ thống (Business Requirements).
- [x] Thiết kế cơ sở dữ liệu (ERD & SQL Script).
- [x] Thiết lập môi trường Docker (SQL Server 2022).
- [x] Lựa chọn Tech Stack (.NET 8, Clean Architecture).
- [x] Khởi tạo Solution và 4 Projects theo mô hình Clean Architecture.
- [x] Scaffold Database vào code C#.
- [x] Tái cấu trúc Entities vào lớp Domain.

## 🚧 Giai đoạn 2: Phát triển Backend Core (Đang thực hiện)
- [x] Triển khai Generic Repository & Unit of Work mẫu.
- [x] Thiết lập Pipeline Logging Middleware (URL & Method).
- [x] Cấu hình Dependency Injection (DI) trong WebApi.
- [x] Thiết lập Authentication & Authorization (JWT Token).
- [x] Cài đặt AutoMapper và FluentValidation.
- [x] Viết API quản lý Người dùng (Chức năng Login hoàn tất).
- [x] Viết API quản lý Xe (Vehicle) - Cơ bản (Lấy danh sách).
- [ ] Viết API quản lý Khách hàng (Customer) và Đăng ký (Register).

## 🛠️ Giai đoạn 3: Nghiệp vụ Bảo hành & AI (Sắp tới)
- [ ] Xây dựng quy trình tạo yêu cầu bảo hành (Warranty Claim).
- [ ] Module quản lý hình ảnh/tài liệu đính kèm (Claim Attachments).
- [ ] Module quản lý tồn kho tại Service Center (SC Inventory).
- [ ] Tích hợp AI Engine để phân tích và dự báo lỗi hỏng hóc.
- [ ] Xây dựng báo cáo thống kê (Dashboard).

## 🌐 Giai đoạn 4: Frontend & Kiểm thử (Tương lai)
- [ ] Xây dựng giao diện Admin/Staff bằng Blazor (hoặc React).
- [ ] Viết Unit Test cho các lớp nghiệp vụ.
- [ ] Kiểm thử tích hợp (Integration Testing).
- [ ] Tối ưu hóa hiệu năng & Caching (Redis).

---

## 🚀 Mục tiêu hiện tại
*Hoàn thành việc triển khai Repository Pattern để bắt đầu viết các API đầu tiên.*
