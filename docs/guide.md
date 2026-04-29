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

## 🚀 11. Lộ trình tiếp theo (Roadmap)

1.  **Phase 3 - Warranty Claims**: Xây dựng nghiệp vụ cốt lõi: Tiếp nhận yêu cầu bảo hành và quy trình xét duyệt.
2.  **Phase 3 - AI Integration**: Tích hợp AI để phân tích lỗi và phát hiện gian lận bảo hành.
3.  **Phase 4 - Frontend**: Phát triển giao diện Dashboard và Portal.

---

_Cập nhật lần cuối: 29/04/2026 bởi Mentor AI_
