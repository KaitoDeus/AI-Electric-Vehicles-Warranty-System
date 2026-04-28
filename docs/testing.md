# 🧪 Kế hoạch và Nhật ký Kiểm thử (Testing Log)

Tài liệu này ghi lại các tính năng đã hoàn thành và danh sách các lỗi đã được xử lý để phục vụ quá trình kiểm thử.

## 🚀 Tính năng đã hoàn thành (Implemented Features)

### 1. Module Xác thực (Authentication)
- [x] **Đăng nhập (Login)**: Xác thực người dùng bằng Username và Password.
- [x] **Mã hóa BCrypt**: Mật khẩu được lưu dưới dạng băm (hashed) an toàn.
- [x] **Cấp phát JWT**: Trả về Token kèm thông tin người dùng (Claims) khi đăng nhập đúng.
- [x] **Phân quyền cơ bản**: Token đã bao gồm Role (Admin/Staff/Customer).

### 2. Quản lý Xe (Vehicle Management - Cơ bản)
- [x] **API Get All**: Lấy danh sách xe từ Database thông qua Unit of Work.
- [x] **Bảo mật API**: Yêu cầu JWT Token (Bearer) để truy cập.

### 3. Quản lý Khách hàng (Customer Management)
- [ ] **API Register**: Thêm mới khách hàng với đầy đủ thông tin.
- [ ] **API Get All Customers**: Lấy danh sách khách hàng (Yêu cầu Token).
- [ ] **Validation**: Kiểm tra lỗi khi để trống Tên hoặc sai định dạng Email.

---

## 🛠 Các lỗi đã xử lý (Bug Fixes)

| ID | Vấn đề (Issue) | Giải pháp (Solution) | Trạng thái |
|:---|:---|:---|:---|
| F01 | Lỗi thiếu `IConfiguration` trong `AuthService` | Thêm `using Microsoft.Extensions.Configuration` và cài đặt Package. | ✅ Fixed |
| F02 | `BCrypt.Verify` trả về false dù mật khẩu đúng | Đồng bộ mã Hash trong DB với chuẩn Salt version của thư viện .NET. | ✅ Fixed |
| F03 | Cột `PasswordHash` bị cắt ngắn chuỗi | Nâng cấp độ dài cột từ `VARCHAR(50)` lên `VARCHAR(255)` trong SQL. | ✅ Fixed |
| F04 | Lỗi `IEnumerable` không hỗ trợ `null` check | Sử dụng `.FirstOrDefault()` từ thư viện LINQ để lấy user duy nhất. | ✅ Fixed |
| F05 | Không thể đăng nhập bằng tài khoản `admin` mẫu | Sai mật khẩu băm trong script seed data hoặc sai salt version. | ❌ Open |

---

## 📋 Nhật ký Kiểm thử Chi tiết (Testing Log - 27/04/2026)

| ID | Test Case | Kết quả | Ghi chú |
|:---|:---|:---|:---|
| TC-C01 | Kiểm tra API Khách hàng (No Token) | ✅ Pass | Hệ thống chặn 401 đúng mong đợi. |
| TC-L01 | Đăng nhập tài khoản `admin` | ❌ Fail | Trả về 401 Unauthorized dù đã thử nhiều mật khẩu phổ biến. |
| TC-C02 | Đăng ký khách hàng mới | ⚠️ Block | Bị chặn do không có Token từ TC-L01. |
| F05 | Swagger không cho gửi Token | Cấu hình `AddSecurityDefinition` và `AddSecurityRequirement` trong `Program.cs`. | ✅ Fixed |
| F06 | Lỗi 401 do Password giả ở DB không hợp lệ BCrypt | Sửa `db/script.sql` và update trực tiếp DB thật với chuỗi hash đúng của `123456`. | ✅ Fixed |
| F07 | Port không đồng bộ (5254 vs 5000) | Sửa lại `launchSettings.json` và file `EVWarranty.WebApi.http` để chạy chuẩn port 5000. | ✅ Fixed |

---

## 📝 Hướng dẫn Kiểm thử (Test Cases)

### TC01: Đăng nhập thành công
1. **Input**: Username `admin`, Password `123456`.
2. **Expected**: Trả về HTTP 200, có chuỗi Token dài.

### TC02: Truy cập API bảo mật
1. **Input**: Gọi `GET /api/Vehicles` không đính kèm Token.
2. **Expected**: Trả về HTTP 401 Unauthorized.
3. **Action**: Đính kèm Token (Bearer) vào Header.
4. **Expected**: Trả về HTTP 200 và danh sách xe.

---
*Cập nhật lần cuối: 27/04/2026 bởi Mentor AI*
