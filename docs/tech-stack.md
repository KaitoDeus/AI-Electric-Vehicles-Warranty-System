# Technology Stack - AI Powered EV Warranty System

Dưới đây là bộ công nghệ đề xuất dựa trên ngôn ngữ chủ đạo là **C#** và cơ sở dữ liệu **SQL Server**.

## 1. Backend (Core Engine)
*   **Framework:** .NET 8 Web API.
*   **Database Provider:** Microsoft.EntityFrameworkCore.SqlServer.
*   **Authentication & Security:**
    *   JWT Bearer Authentication.
    *   OAuth 2.0 (Google/Facebook Login).
    *   MailKit / SendGrid (Gửi Email OTP khôi phục mật khẩu).
*   **Architecture:** Clean Architecture hoặc Onion Architecture (giúp code dễ bảo trì, tách biệt logic nghiệp vụ và dữ liệu).
*   **AI & ML Integration:**
    *   **Semantic Kernel (Microsoft):** Tích hợp AI (GPT, Gemini) vào logic C#.
    *   **ML.NET / Python:** Cho Recommendation System gợi ý sản phẩm và dự báo hỏng hóc đơn giản.

## 2. Frontend (Giao diện người dùng)
*   **Admin/Staff Portal:** **Blazor WebAssembly**.
    *   *Ưu điểm:* Sử dụng chung các Model C# với Backend, không cần viết lại định dạng dữ liệu, tốc độ phát triển cực nhanh.
*   **UI Library:** MudBlazor hoặc Ant Design for Blazor (giúp giao diện chuyên nghiệp, premium).

## 3. Database & Caching
*   **Primary DB:** SQL Server 2022 (Docker).
*   **Caching:** Redis (cho các dữ liệu ít thay đổi như danh mục linh kiện, chính sách bảo hành, giỏ hàng).
*   **Migrations:** EF Core Migrations (quản lý phiên bản DB ngay trong code C#).

## 4. Payment & Invoicing (Mới)
*   **Payment Gateways:** VietQR, VNPAY, MoMo (Tích hợp qua SDK hoặc API).
*   **Invoicing:** `QuestPDF` (Thư viện mạnh mẽ để tạo hoá đơn PDF từ code C#).

## 5. DevOps & Deployment
*   **Containerization:** Docker, Docker Compose.
*   **Logging:** Serilog (lưu log ra file hoặc SQL Server để theo dõi lỗi).
*   **API Documentation:** Swagger (OpenAPI).

---

## Lộ trình triển khai (Next Steps)
1.  **Khởi tạo Solution:** Tạo ASP.NET Core Web API project.
2.  **Scaffold Database:** Sử dụng EF Core để tạo các Class từ database đã có.
3.  **Identity Setup:** Cài đặt hệ thống đăng nhập và phân quyền (RBAC).
4.  **Module Claim:** Xây dựng API cho nghiệp vụ tạo và duyệt yêu cầu bảo hành.
