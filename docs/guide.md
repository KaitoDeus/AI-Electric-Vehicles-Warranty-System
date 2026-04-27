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

---

## 🛠️ 4. Nguyên tắc Code "Sạch" & Tối ưu

### A. Sử dụng Repository Pattern & Unit of Work

Đừng gọi trực tiếp DbContext trong Controller. Hãy bọc nó qua các Repository để sau này nếu bạn đổi DB (ví dụ sang PostgreSQL), bạn không phải sửa code ở tầng WebAPI.

### B. Dependency Injection (DI)

Luôn đăng ký các Service trong `Program.cs`.

- **Scoped:** Cho các dịch vụ xử lý Database (mỗi request một instance).
- **Singleton:** Cho các dịch vụ dùng chung toàn hệ thống (như AI Engine).

### C. Xử lý bất đồng bộ (Async/Await)

Luôn sử dụng `Task`, `async`, `await` cho mọi thao tác IO (đọc ghi DB, gọi API ngoài) để tránh làm nghẽn hệ thống khi có hàng nghìn nhân viên truy cập cùng lúc.

### D. Tối ưu AI Analytics

Khi viết module AI dự báo, hãy sử dụng **Background Tasks** (Worker Service) để xử lý các phép toán nặng ngầm, tránh làm người dùng phải chờ đợi trên giao diện.

---

## 📝 5. Lời khuyên từ Mentor

1.  **Đừng viết logic ở Controller:** Controller chỉ nên làm nhiệm vụ nhận dữ liệu và trả về kết quả. Toàn bộ tính toán hãy đẩy vào tầng **Application**.
2.  **Sử dụng AutoMapper:** Để chuyển đổi giữa Entity (Database) sang DTO (Data Transfer Object) cho Frontend, giúp bảo mật dữ liệu nhạy cảm (như PasswordHash).
3.  **Fluent Validation:** Sử dụng thư viện này để kiểm tra dữ liệu đầu vào (ví dụ: Số VIN phải đúng 17 ký tự) một cách chuyên nghiệp.

---

## 🛠️ 6. Triển khai Code mẫu (Boilerplate)

Dưới đây là cách triển khai chuẩn cho các nguyên tắc đã nêu ở Phần 4.

### A. Generic Repository (Dùng chung cho mọi bảng)

Tạo file `IGenericRepository.cs` trong tầng **Domain** (hoặc Application):

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

Triển khai tại tầng **Infrastructure**:

```csharp
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly EvWarrantyDbContext _context;
    public GenericRepository(EvWarrantyDbContext context) => _context = context;

    public async Task<T?> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();
    public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);
    public void Update(T entity) => _context.Set<T>().Update(entity);
    public void Delete(T entity) => _context.Set<T>().Remove(entity);
}
```

### B. Unit of Work (Quản lý Transaction)

Giúp đảm bảo tính toàn vẹn dữ liệu. Tạo trong tầng **Application**:

```csharp
public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Vehicle> Vehicles { get; }
    IGenericRepository<WarrantyClaim> Claims { get; }
    Task<int> CompleteAsync(); // Lưu tất cả thay đổi
}
```

### C. Đăng ký Dependency Injection (DI)

Trong file `Program.cs` của project **WebApi**, hãy thêm các dòng sau:

```csharp
// Đăng ký DbContext
builder.Services.AddDbContext<EvWarrantyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký Repository & Unit of Work
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

---

## 🛡️ 7. Bảo mật & Hiệu năng

1.  **appsettings.json:** Tuyệt đối không để Password ở dạng văn bản thuần khi deploy thật. Sử dụng **User Secrets** hoặc **Environment Variables**.
2.  **Pagination:** Luôn phân trang cho các API lấy danh sách (GetAll) để tránh làm sập server khi dữ liệu xe điện lên đến hàng triệu chiếc.
3.  **Logging:** Cài đặt gói `Serilog.AspNetCore` để ghi lại lịch sử lỗi.

---

---

## 🛡️ 8. Middleware & Request Pipeline

### ❓ Middleware là gì?

Middleware là những đoạn code nằm trong pipeline (đường ống) xử lý yêu cầu của ASP.NET Core. Mỗi middleware có thể thực hiện logic trước và sau khi request đi qua nó (như Logging, Authentication, Error Handling).

### 📝 Triển khai Request Logging Middleware

Tôi đã cài đặt cho bạn một middleware để theo dõi mọi truy cập vào hệ thống ngay tại terminal.

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

## 🖥️ 9. Hướng dẫn chạy dự án trên Visual Studio 2022

Để bắt đầu phát triển và kiểm thử API, hãy làm theo các bước sau:

1.  **Mở Solution:** Nhấp đúp vào file `EVWarrantySystem.sln`.
2.  **Set Startup Project:**
    - Chuột phải vào project **`EVWarranty.WebApi`**.
    - Chọn **"Set as Startup Project"**.
3.  **Kiểm tra Infrastructure:**
    - Đảm bảo Docker Container chứa SQL Server đang chạy (`docker-compose up -d`).
    - Kiểm tra chuỗi kết nối trong `appsettings.json` đã khớp với thông tin Docker.
4.  **Chạy ứng dụng:**
    - Nhấn **F5** (Debug) hoặc **Ctrl + F5** (Run).
5.  **Truy cập Swagger:**
    - Trình duyệt sẽ mở ra, hãy truy cập đường dẫn `https://localhost:<port>/swagger` để bắt đầu test API.

---

## 🔐 10. Hướng dẫn triển khai tính năng Login (Đang thực hiện)

Chúng ta đang xây dựng luồng xác thực cho hệ thống. Dưới đây là trình tự các bước đã và đang thực hiện:

### ✅ Các bước Mentor đã chuẩn bị:

1.  **Cài đặt thư viện**: Đã thêm các gói cần thiết (`AutoMapper`, `FluentValidation`, `BCrypt`, `JWT`, `Configuration`) vào lớp **Application**.
2.  **Cấu hình Repository**: Thêm `FindAsync` vào `IGenericRepository` và cập nhật `GenericRepository` để hỗ trợ tìm kiếm User.
3.  **Thiết kế DTOs**: Tạo `LoginRequest` và `LoginResponse` để định nghĩa dữ liệu trao đổi.
4.  **Định nghĩa Interface**: Tạo `IAuthService` để quy định các phương thức nghiệp vụ.

### ✅ Thử thách bạn đã hoàn thành:

1.  **Cập nhật Unit of Work**: Bạn đã khai báo và khởi tạo repository `Users`.
2.  **Khởi tạo AuthService**: Bạn đã tạo lớp `AuthService` và xử lý thành công lỗi thư viện.
3.  **Viết Logic cho `LoginAsync`**: Bạn đã triển khai xong phần kiểm tra Username và Password.
4.  **Đăng ký DI**: Bạn đã đăng ký `IAuthService` vào `Program.cs` chính xác.
5.  **Tạo AuthController**: Bạn đã tạo Controller và Endpoint `login` hoàn chỉnh. Rất tuyệt vời!

### 🚩 Thử thách CUỐI CÙNG cho tính năng Login:

#### 1. Thêm `using System.Linq;`:
Trong file `AuthService.cs`, hãy thêm `using System.Linq;` ở đầu file để lệnh `.FirstOrDefault()` có thể hoạt động.

#### 2. Viết hàm `GenerateJwtToken` (Trong `AuthService.cs`):
Đây là bước "phù phép" để biến thông tin User thành một chuỗi mã hóa an toàn. Bạn hãy thêm các directive cần thiết (`using System.Security.Claims;`, `using System.IdentityModel.Tokens.Jwt;`, `using System.Text;`, `using Microsoft.IdentityModel.Tokens;`) và thêm hàm private này vào cuối lớp `AuthService`:

```csharp
private string GenerateJwtToken(User user)
{
    var jwtSettings = _configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Role, "Admin") // Tạm thời để Admin
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddHours(3),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        Issuer = jwtSettings["Issuer"],
        Audience = jwtSettings["Audience"]
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}
```

#### 3. Gọi hàm tạo Token:
Trong hàm `LoginAsync`, hãy thay thế `Token = ""` bằng `Token = GenerateJwtToken(user)`.

---

### Mentor's Challenge:

🎉 **Bạn đã hoàn thành xuất sắc thử thách này!** Bạn đã tự tạo thành công `VehiclesController`, sử dụng `IUnitOfWork` để lấy danh sách xe và thậm chí đã biết áp dụng `[Authorize]` để bảo vệ API của mình.

### Tiếp theo bạn muốn tôi thực hiện bước nào?

- [x] Tôi đã viết lệnh tạo dự án thực tế cho bạn?
- [x] Tôi đã viết code mẫu cho lớp **Domain** (Thực thể và Logic)?
- [x] Triển khai Repository & Unit of Work (Hoàn thành).
- [x] Thiết lập Logging Middleware (Hoàn thành).
- [x] Cấu hình Swagger và JWT Authentication (Hoàn thành).
- [x] Tôi sẽ hướng dẫn bạn viết API Login để lấy Token? (Hoàn thành)
