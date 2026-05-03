# 🛡️ C# Mentor Guide: Building the EV Warranty System

Chào mừng bạn đến với lộ trình xây dựng hệ thống quản lý bảo hành xe điện bằng **.NET 8**. Bản hướng dẫn này được thiết kế theo tiêu chuẩn chuyên nghiệp, tập trung vào **Clean Architecture** và các **Best Practices**.

---

## 🏗️ 1. Kiến trúc hệ thống (Clean Architecture)

Để hệ thống dễ bảo trì, chúng ta sẽ chia Solution thành 4 lớp:

1.  **Domain:** Chứa các thực thể (Entities), interface và logic cốt lõi. (Không phụ thuộc vào bất kỳ lớp nào).
2.  **Application:** Chứa các Service xử lý nghiệp vụ (Business Logic), DTOs và Mapping.
3.  **Infrastructure:** Chứa code kết nối Database (EF Core), Mail Service, AI Service.
4.  **WebAPI:** Lớp ngoài cùng, cung cấp các Endpoint cho Frontend gọi vào.

---

## 🚀 2. Các bước khởi tạo dự án

Mở Terminal tại thư mục gốc và chạy chuỗi lệnh sau để tạo cấu trúc chuẩn:

```bash
# 1. Tạo Solution
dotnet new sln -n EVWarrantySystem

# 2. Tạo các lớp kiến trúc
dotnet new classlib -n EVWarranty.Domain
dotnet new classlib -n EVWarranty.Application
dotnet new classlib -n EVWarranty.Infrastructure
dotnet new webapi -n EVWarranty.WebApi

# 3. Thêm các project vào Solution
dotnet sln add EVWarranty.Domain
dotnet sln add EVWarranty.Application
dotnet sln add EVWarranty.Infrastructure
dotnet sln add EVWarranty.WebApi

# 4. Thiết lập phụ thuộc (References)
dotnet add EVWarranty.Application reference EVWarranty.Domain
dotnet add EVWarranty.Infrastructure reference EVWarranty.Application
dotnet add EVWarranty.WebApi reference EVWarranty.Infrastructure
```

---

## 💾 3. Kết nối Database (Database-First & Scaffold)

### ❓ Scaffold là gì?

**Scaffold** là kỹ thuật "phản chiếu" (Reverse Engineering) từ Database có sẵn vào code. Thay vì viết tay từng Class, lệnh này sẽ tự động tạo ra các Class C# tương ứng với các bảng trong SQL Server, giúp tiết kiệm 90% thời gian thiết lập ban đầu.

### 📦 Các thư viện cần thiết

Để làm việc với SQL Server và chạy được lệnh Scaffold, bạn cần cài đặt các gói NuGet sau vào project **Infrastructure**:

```bash
# Di chuyển vào thư mục Infrastructure
cd EVWarranty.Infrastructure

# Cài đặt các gói (Lưu ý chọn phiên bản phù hợp với .NET của bạn, ví dụ 9.0.2)
dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 9.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design -v 9.0.2
dotnet add package Microsoft.EntityFrameworkCore.Tools -v 9.0.2
```

### ⚡ Lệnh chạy Scaffold

Sau khi cài đặt xong thư viện, chạy lệnh này để "bê" Database vào code:

```bash
dotnet ef dbcontext scaffold "Server=localhost,1433;Database=EV_Warranty_DB;User Id=sa;Password=Password123;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Data --no-onconfiguring
```

_Lưu ý: Tham số `--no-onconfiguring` giúp tách biệt chuỗi kết nối ra khỏi code để bảo mật hơn._

---

## 🛡️ 4. Middleware & Request Pipeline

### ❓ Middleware là gì?

Middleware là những đoạn code nằm trong pipeline (đường ống) xử lý yêu cầu của ASP.NET Core. Mỗi middleware có thể thực hiện logic trước và sau khi request đi qua nó (như Logging, Authentication, Error Handling).

### 📝 Triển khai Request Logging Middleware

1. **Cấu trúc:** Đặt trong `EVWarranty.WebApi/Middlewares/RequestLoggingMiddleware.cs`.
2. **Tính năng:** Hiển thị Method (màu sắc), URL, Status Code và thời gian xử lý (ms).

**Cách đăng ký trong `Program.cs`:**

```csharp
using EVWarranty.WebApi.Middlewares;

var app = builder.Build();

// Đăng ký ngay sau khi build app để bắt được mọi request
app.UseMiddleware<RequestLoggingMiddleware>();
```

---

## 🛠️ 5. Nguyên tắc Code "Sạch" & Triển khai Boilerplate

### A. Sử dụng Repository Pattern & Unit of Work

Đừng gọi trực tiếp DbContext trong Controller. Hãy bọc nó qua các Repository để sau này nếu bạn đổi DB, bạn không phải sửa code ở tầng WebAPI.

**IGenericRepository.cs (Domain):**

```csharp
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
}
```

**IUnitOfWork.cs (Domain):**

```csharp
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Vehicle> Vehicles { get; }
    IGenericRepository<User> Users { get; }
    Task<int> CompleteAsync();
}
```

### B. Dependency Injection (DI)

Đăng ký trong `Program.cs`:

```csharp
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

---

## 🔐 6. Hướng dẫn triển khai tính năng Login

### Các bước thực hiện:

1.  **Thiết kế DTOs**: Tạo `LoginRequest` và `LoginResponse`.
2.  **AuthService**: Xử lý logic kiểm tra mật khẩu bằng BCrypt.
3.  **Generate JWT**: Tạo chuỗi Token mã hóa.
4.  **AuthController**: Endpoint `login` để trả về Token cho người dùng.

### 🚩 Thử thách tạo JWT:

```csharp
private string GenerateJwtToken(User user)
{
    var jwtSettings = _configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
    // ... logic tạo claims và token ...
    return tokenHandler.WriteToken(token);
}
```

---

## 🛠️ 7. Hướng dẫn API quản lý Khách hàng (Customer)

Chào mừng bạn đến với module **Quản lý Khách hàng**. Chúng ta thực hiện theo 6 bước:

### 🛡️ 7. Quy trình triển khai Module mới (Chuẩn Mentor)

Khi phát triển một module mới (ví dụ: Warranty Claims), hãy tuân thủ các bước sau để đảm bảo tính **Clean** và **Secure**:

- **Bước 1 (Định danh):** Luôn sử dụng `Guid` (Uniqueidentifier) cho Khóa chính (PK) để tránh lộ dữ liệu kinh doanh qua ID tuần tự.
- **Bước 2 (DTOs):** Thiết kế DTOs tách biệt hoàn toàn với Entity. Tuyệt đối không trả về Entity trực tiếp ra API.
- **Bước 3 (Validation):** Sử dụng `FluentValidation` để áp dụng quy chuẩn dữ liệu:
  - **Email:** Phải có định dạng đầy đủ (ví dụ: `.com`, `.vn`).
  - **IDNumber:** Phải là 12 chữ số (Chuẩn CCCD Việt Nam).
  - **Phone:** 10-11 chữ số.
- **Bước 4 (Manual Mapping):** Thay vì dùng AutoMapper, hãy viết code mapping thủ công trong Service. Việc này giúp bạn kiểm soát hoàn toàn dữ liệu và dễ dàng Debug khi có lỗi.
- **Bước 5 (Authorization):** Luôn gắn nhãn `[Authorize(Roles = "...")]` cho Controller. Phải xác định rõ "Ai được quyền làm gì" trước khi viết code logic.
- **Bước 6 (Service & Repository):** Triển khai logic qua Interface và sử dụng `UnitOfWork` để quản lý Transaction.

---

### 🔐 8. Hệ thống Phân quyền (Role-based)

Dự án sử dụng cơ chế phân quyền dựa trên Role được lưu trong JWT Token. Các Role hiện có:

- `Admin`: Toàn quyền hệ thống.
- `SC Staff`: Nhân viên trung tâm dịch vụ (Quản lý xe, khách hàng, tạo yêu cầu bảo hành).
- `SC Technician`: Kỹ thuật viên (Kiểm tra xe, cập nhật trạng thái sửa chữa).
- `EVM Staff`: Nhân viên hãng (Duyệt yêu cầu bảo hành, quản lý chiến dịch).

**Cách sử dụng trong Controller:**

```csharp
[Authorize(Roles = "Admin,SC Staff")]
public class YourController : ControllerBase { ... }
```

---

### 🖥️ 9. Hướng dẫn chạy dự án (Run & Test)

1.  **Database:** Luôn chạy file `db/script.sql` mới nhất khi có thay đổi cấu trúc GUID.
2.  **Docker:** SQL Server 2022 phải đang chạy (`docker-compose up -d`).
3.  **Run:** Sử dụng lệnh `dotnet watch run` tại thư mục `EVWarranty.WebApi`.
4.  **Swagger:** Truy cập `http://localhost:5000/swagger`.
5.  **Authorization Test:**
    - Dùng tài khoản `evm_staff` để test lỗi **403 Forbidden** khi vào API Khách hàng.
    - Dùng tài khoản `sc_staff` hoặc `admin` để truy cập thành công.
    - Chú ý: Token có thời hạn 30 phút, nếu bị lỗi 401 hãy đăng nhập lại để làm mới Token.

---

## 🛡️ 10. Global Exception Handling

Hệ thống sử dụng `IExceptionHandler` (.NET 8) để bắt mọi lỗi Runtime và trả về định dạng `ProblemDetails` chuẩn RFC 7807. Điều này giúp bảo mật thông tin hệ thống và cung cấp thông báo lỗi nhất quán cho Frontend.

- **Handler**: `GlobalExceptionHandler.cs` trong thư mục Middlewares.
- **Đăng ký**: Đã cấu hình `AddExceptionHandler` và `UseExceptionHandler` trong `Program.cs`.

---

## 📅 11. Lộ trình hoàn thiện chi tiết (30 Ngày)

Dưới đây là kế hoạch chi tiết để bạn hoàn thiện dự án, tập trung vào cả E-commerce và Nghiệp vụ Bảo hành.

### 🏁 Tuần 1: Cấu trúc E-commerce & Auth mở rộng

- **Ngày 1:** Cập nhật Database với các bảng mới (`Carts`, `Orders`, `Invoices`, `Favorites`, `Payments`, `Reviews`, `OTP_Tokens`, `User_OAuth`). Chạy lại Scaffold.
- **Ngày 2:** Mở rộng `AuthService`: OTP quên mật khẩu, OAuth (Google), xử lý đăng ký tài khoản Customer.
- **Ngày 3:** Triển khai API Sản phẩm cho Landing Page, tích hợp Recommendation System cơ bản.
- **Ngày 4:** Xây dựng API Sản phẩm yêu thích (Favorite) và Đánh giá (Rating & Review).
- **Ngày 5:** Tối ưu hóa hiệu năng truy vấn cho danh mục sản phẩm (Sử dụng Eager Loading và DTOs).

### 🛒 Tuần 2: Giỏ hàng & Quy trình Đặt hàng

- **Ngày 6:** Triển khai API Giỏ hàng (Cart): Lưu trữ các sản phẩm khách muốn mua.
- **Ngày 7:** Xử lý logic tính toán tổng tiền, thuế và phí vận chuyển trong Giỏ hàng.
- **Ngày 8:** Triển khai API Đặt hàng (Order): Chuyển đổi từ Cart sang Order, trừ tồn kho (Inventory).
- **Ngày 9:** Xây dựng hệ thống Trạng thái đơn hàng: Chờ xác nhận, Đã thanh toán, Đang giao...
- **Ngày 10:** API Lịch sử đơn hàng dành cho khách hàng.
- **Ngày 11-12:** Viết Unit Test cho logic Đặt hàng và trừ kho.

### 💳 Tuần 3: Thanh toán, Hoá đơn & Landing Page

- **Ngày 13:** Tích hợp Sandbox cho cổng thanh toán (VietQR hoặc VNPAY).
- **Ngày 14:** Triển khai API xử lý kết quả thanh toán (IPN - Instant Payment Notification).
- **Ngày 15:** Xây dựng Service xuất hoá đơn (Invoice): Sử dụng thư viện `QuestPDF` để tạo file PDF.
- **Ngày 16:** API tải hoá đơn và gửi mail xác nhận đơn hàng kèm hoá đơn đính kèm.
- **Ngày 17:** Phát triển Landing Page (Frontend cơ bản): Hiển thị xe, tra cứu bảo hành qua VIN.
- **Ngày 18:** Hoàn thiện giao diện Giỏ hàng và Thanh toán trên Frontend.

### 🛠️ Tuần 4: Nghiệp vụ Bảo hành & Tích hợp AI

- **Ngày 19:** Triển khai API Tạo yêu cầu bảo hành (Warranty Claim) từ Trung tâm dịch vụ.
- **Ngày 20:** Module quản lý tệp đính kèm (Hình ảnh, Log xe) cho mỗi Claim.
- **Ngày 21:** Xây dựng luồng Phê duyệt bảo hành dành cho nhân viên hãng (EVM Staff).
- **Ngày 22:** Tích hợp AI (Semantic Kernel): Phân tích nội dung lỗi và phát hiện gian lận.
- **Ngày 23:** Module quản lý tồn kho phụ tùng tại Service Center.
- **Ngày 24:** Dashboard thống kê doanh thu và tỷ lệ bảo hành (Admin).
- **Ngày 25:** Tối ưu hóa Database (Indexing) và Caching (Redis).

### 🚀 Tuần cuối: Kiểm thử & Triển khai

- **Ngày 26:** Kiểm thử bảo mật: Check phân quyền Roles trên từng Endpoint.
- **Ngày 27:** Kiểm thử tích hợp: Mua xe -> Thanh toán -> Bảo hành.
- **Ngày 28:** Viết tài liệu hướng dẫn sử dụng cho các bộ phận.
- **Ngày 29:** Cấu hình Docker Compose hoàn chỉnh cho toàn bộ hệ thống.
- **Ngày 30:** Triển khai Staging và đóng gói dự án.

---

### 💡 Lời khuyên từ Mentor

- **Luôn Git Commit:** Commit mỗi ngày với message rõ ràng.
- **Kiểm tra Log:** Theo dõi `Serilog` để phát hiện lỗi sớm.
- **Bảo mật:** Sử dụng `.env` để quản lý các Key bí mật.

---

## 🔐 12. Mở rộng Hệ thống Auth (OTP & Google OAuth)

Chào mừng bạn đến với bước nâng cấp quan trọng cho hệ thống bảo mật. Đây là lúc chúng ta đưa ứng dụng lên chuẩn "Production" với các tính năng hiện đại.

### A. Chức năng Quên mật khẩu (OTP)

Quy trình này yêu cầu sự phối hợp giữa Database, Email Service và Logic xử lý Token.

**1. Chuẩn bị Database:**
Bạn cần bảng `OtpTokens` để lưu trữ mã OTP tạm thời:

- `UserId`: Liên kết với người dùng.
- `OtpCode`: Mã số (thường 6 số).
- `ExpiryTime`: Thời điểm hết hạn (nên để 5-10 phút).
- `IsUsed`: Đánh dấu mã đã sử dụng.

**2. Triển khai Email Service (Infrastructure):**

- Sử dụng thư viện `MailKit` hoặc `SendGrid`.
- Cấu hình SMTP (Gmail App Password hoặc dịch vụ bên thứ 3) trong `appsettings.json`.
- Interface `IEmailService` nên có hàm `SendEmailAsync(to, subject, body)`.

**3. Logic nghiệp vụ (Application):**

- **Bước 1 (Request OTP):** Kiểm tra Email tồn tại -> Tạo mã ngẫu nhiên -> Lưu vào `OtpTokens` -> Gửi Email.
- **Bước 2 (Verify & Reset):** Người dùng nhập mã OTP và Mật khẩu mới -> Kiểm tra mã (đúng, chưa dùng, còn hạn) -> Cập nhật `PasswordHash` trong bảng `Users` -> Đánh dấu mã `IsUsed = true`.

### B. Đăng nhập Google OAuth (Google Login)

Chúng ta sử dụng luồng **Implicit Flow** hoặc **Authorization Code Flow** tùy thuộc vào Frontend, nhưng Backend sẽ đóng vai trò xác thực "ID Token".

**1. Cấu hình Google Cloud Console:**

- Tạo dự án mới -> APIs & Services -> Credentials.
- Tạo **OAuth 2.0 Client ID** (Web application).
- Lấy `ClientId` và `ClientSecret`.

**2. Thư viện hỗ trợ:**
Bạn cần gói NuGet `Google.Apis.Auth` trong project **Application** (Đã có sẵn trong cấu hình của bạn).

**3. Quy trình xác thực:**

1. **Frontend:** Gọi Google SDK, người dùng đăng nhập và nhận về một chuỗi `idToken`.
2. **Backend (API):** Nhận `idToken` từ Frontend.
3. **Verify:** Sử dụng `GoogleJsonWebSignature.ValidateAsync(idToken)` để kiểm tra tính hợp lệ của Token từ server Google.
4. **Logic User:**
   - Nếu Email trong Token đã có trong DB: Tiến hành tạo JWT và đăng nhập.
   - Nếu chưa có: Tạo tài khoản mới cho người dùng này (với mật khẩu ngẫu nhiên hoặc bỏ trống) rồi trả về JWT.

### 💡 Lời khuyên của Mentor:

- **Bảo mật:** Không bao giờ trả về mã OTP trong phản hồi API.
- **Giới hạn:** Nên có cơ chế "Rate Limit" để tránh người dùng yêu cầu gửi OTP liên tục (Spam).
- **Trải nghiệm:** Khi đăng nhập Google thành công lần đầu, hãy đảm bảo bạn lưu lại `GoogleId` để liên kết tài khoản chính xác cho những lần sau.

---

## 🚀 13. Triển khai Refresh Token (Cơ chế duy trì đăng nhập)

Đây là bước nâng cấp giúp người dùng không phải đăng nhập lại sau mỗi 30 phút.

### 1. Cấu trúc bảng Database bổ sung

Bạn cần thêm bảng `RefreshTokens` để lưu trữ các mã định danh dài hạn:

```sql
CREATE TABLE RefreshTokens (
    TokenID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    Token VARCHAR(255) NOT NULL,
    ExpiryTime DATETIME NOT NULL,
    IsRevoked BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
```

### 2. Luồng hoạt động (Workflow)

1.  **Khi Login thành công:** Backend trả về đồng thời `AccessToken` (30 phút) và `RefreshToken` (7 ngày).
2.  **Khi AccessToken hết hạn:** Frontend gửi `RefreshToken` lên một Endpoint mới (ví dụ: `/api/auth/refresh`).
3.  **Backend kiểm tra:**
    *   Nếu `RefreshToken` còn hạn và chưa bị thu hồi (`IsRevoked = 0`).
    *   Thì cấp một cặp `AccessToken` và `RefreshToken` mới.
4.  **Khi Logout:** Backend đánh dấu `IsRevoked = 1` cho Token đó.

### 💡 Lời khuyên của Mentor:

- **Bảo mật:** `RefreshToken` nên được lưu trong **HttpOnly Cookie** ở phía Frontend để chống lại tấn công XSS.
- **Xoay vòng (Rotation):** Mỗi lần Refresh thành công, hãy vô hiệu hóa Token cũ và cấp Token mới hoàn toàn để tăng tính an toàn.

---

_Cập nhật lần cuối: 03/05/2026 bởi Mentor AI_
