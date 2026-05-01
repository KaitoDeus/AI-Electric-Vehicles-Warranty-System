# Database Design for EV Warranty System (SQL Server)

## 1. Cơ cấu các bảng dữ liệu

### Nhóm 1: Hệ thống & Phân quyền
*   **Roles**: Lưu các chức danh (Admin, EVM Staff, SC Staff, SC Technician).
*   **ServiceCenters**: Danh sách các trung tâm dịch vụ trên toàn quốc.
*   **Users**: Tài khoản người dùng, liên kết với Role và ServiceCenter.
*   **OTP_Tokens** & **User_OAuth**: Hỗ trợ quên mật khẩu bằng OTP và đăng nhập qua mạng xã hội (Google, Facebook).

### Nhóm 2: Sản phẩm & Khách hàng
*   **Customers**: Thông tin chủ sở hữu xe.
*   **Vehicles**: Thông tin định danh xe (VIN), Model, ngày mua, và khách hàng sở hữu.
*   **Parts**: Danh mục các loại phụ tùng (ví dụ: Pin 72V, Động cơ 3000W, Bộ điều khiển BMS).
*   **VehicleParts**: Lưu thông tin linh kiện **cụ thể** đang lắp trên xe (ví dụ: Xe VIN X đang lắp Pin có số seri S1).

### Nhóm 3: Quy trình Bảo hành (Core)
*   **WarrantyPolicies**: Chính sách bảo hành (ví dụ: Pin bảo hành 5 năm hoặc 50.000km).
*   **WarrantyClaims**: Các yêu cầu bảo hành được gửi lên.
*   **ClaimAttachments**: Hình ảnh bằng chứng, báo cáo chẩn đoán lỗi.

### Nhóm 4: E-commerce & Customer Portal
*   **Carts**, **Orders**, **Invoices**, **Payments**: Quản lý mua hàng và thanh toán.
*   **Favorites**: Sản phẩm yêu thích.
*   **Reviews**: Đánh giá sản phẩm.

---

## 2. SQL Script (T-SQL)

Dưới đây là mã SQL để bạn tạo Database và các bảng trên SQL Server:

```sql
CREATE DATABASE EV_Warranty_DB;
GO
USE EV_Warranty_DB;
GO

-- 1. Roles & Users
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE ServiceCenters (
    SCID INT PRIMARY KEY IDENTITY(1,1),
    SCName NVARCHAR(200) NOT NULL,
    Address NVARCHAR(500),
    Phone VARCHAR(20),
    Email VARCHAR(100)
);

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Email VARCHAR(100),
    RoleID INT FOREIGN KEY REFERENCES Roles(RoleID),
    SCID INT NULL FOREIGN KEY REFERENCES ServiceCenters(SCID),
    IsActive BIT DEFAULT 1
);

CREATE TABLE OTP_Tokens (
    TokenID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    OTPCode VARCHAR(10) NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    IsUsed BIT DEFAULT 0
);

CREATE TABLE User_OAuth (
    OAuthID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    Provider VARCHAR(50) NOT NULL,
    ProviderUserID VARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 2. Products & Customers
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(20) UNIQUE,
    Email VARCHAR(100),
    Address NVARCHAR(500),
    IDNumber VARCHAR(20) UNIQUE
);

CREATE TABLE Vehicles (
    VIN VARCHAR(17) PRIMARY KEY,
    ModelName NVARCHAR(100) NOT NULL,
    Color NVARCHAR(50),
    PurchaseDate DATE,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    CurrentMileage INT DEFAULT 0,
    Status NVARCHAR(50) -- Active, Recalled, Sold
);

CREATE TABLE Parts (
    PartID INT PRIMARY KEY IDENTITY(1,1),
    PartCode VARCHAR(50) UNIQUE,
    PartName NVARCHAR(200) NOT NULL,
    Category NVARCHAR(100), -- Battery, Motor, BMS...
    Unit NVARCHAR(20)
);

-- Linh kiện cụ thể trên từng xe (có Serial riêng)
CREATE TABLE VehicleParts (
    VehiclePartID INT PRIMARY KEY IDENTITY(1,1),
    VIN VARCHAR(17) FOREIGN KEY REFERENCES Vehicles(VIN),
    PartID INT FOREIGN KEY REFERENCES Parts(PartID),
    SerialNumber VARCHAR(100) UNIQUE NOT NULL,
    InstalledDate DATE,
    Status NVARCHAR(50) -- Normal, Faulty, Replaced
);

-- 3. Warranty & Claims
CREATE TABLE WarrantyPolicies (
    PolicyID INT PRIMARY KEY IDENTITY(1,1),
    PartID INT FOREIGN KEY REFERENCES Parts(PartID),
    ModelName NVARCHAR(100),
    DurationMonths INT,
    MileageLimit INT,
    Conditions NVARCHAR(MAX)
);

CREATE TABLE WarrantyClaims (
    ClaimID INT PRIMARY KEY IDENTITY(1,1),
    VIN VARCHAR(17) FOREIGN KEY REFERENCES Vehicles(VIN),
    SCID INT FOREIGN KEY REFERENCES ServiceCenters(SCID),
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserID),
    Description NVARCHAR(MAX),
    DiagnosticNotes NVARCHAR(MAX),
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Approved, Rejected, Processing, Completed
    CreatedAt DATETIME DEFAULT GETDATE(),
    ApprovedAt DATETIME,
    ApprovedBy INT FOREIGN KEY REFERENCES Users(UserID)
);

CREATE TABLE ClaimAttachments (
    AttachmentID INT PRIMARY KEY IDENTITY(1,1),
    ClaimID INT FOREIGN KEY REFERENCES WarrantyClaims(ClaimID),
    FileType NVARCHAR(50), -- Image, PDF, Log
    FilePath NVARCHAR(1000),
    UploadedAt DATETIME DEFAULT GETDATE()
);

-- 4. E-commerce & Customer Portal (Mới)
CREATE TABLE Favorites (
    FavoriteID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    VIN VARCHAR(17) FOREIGN KEY REFERENCES Vehicles(VIN), -- Lưu VIN mẫu xe quan tâm
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Carts (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME
);

CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    PartID INT NULL FOREIGN KEY REFERENCES Parts(PartID),
    VIN VARCHAR(17) NULL FOREIGN KEY REFERENCES Vehicles(VIN),
    Rating INT CHECK (Rating >= 1 AND Rating <= 5),
    Comment NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE CartItems (
    CartItemID INT PRIMARY KEY IDENTITY(1,1),
    CartID INT FOREIGN KEY REFERENCES Carts(CartID),
    PartID INT NULL FOREIGN KEY REFERENCES Parts(PartID), -- Phụ kiện
    ModelName NVARCHAR(100) NULL, -- Hoặc VIN xe mới
    Quantity INT DEFAULT 1,
    Price DECIMAL(18,2)
);

CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2),
    Status NVARCHAR(50), -- Pending, Paid, Shipping, Completed, Cancelled
    ShippingAddress NVARCHAR(500),
    PaymentMethod NVARCHAR(50) -- VietQR, VNPAY, MoMo
);

CREATE TABLE OrderItems (
    OrderItemID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    PartID INT NULL FOREIGN KEY REFERENCES Parts(PartID),
    ModelName NVARCHAR(100) NULL,
    Quantity INT,
    UnitPrice DECIMAL(18,2)
);

CREATE TABLE Invoices (
    InvoiceID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    InvoiceNumber VARCHAR(50) UNIQUE,
    IssuedDate DATETIME DEFAULT GETDATE(),
    TaxCode VARCHAR(20),
    TotalAmount DECIMAL(18,2),
    FilePath NVARCHAR(1000) -- Lưu link file PDF
);

CREATE TABLE Payments (
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    TransactionID VARCHAR(100), -- ID từ VNPAY/MoMo
    Amount DECIMAL(18,2),
    PaymentDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) -- Success, Failed
);

```
