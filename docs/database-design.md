# Database Design for EV Warranty System (SQL Server)

## 1. Cơ cấu các bảng dữ liệu

### Nhóm 1: Hệ thống & Phân quyền
*   **Roles**: Lưu các chức danh (Admin, EVM Staff, SC Staff, SC Technician).
*   **ServiceCenters**: Danh sách các trung tâm dịch vụ trên toàn quốc.
*   **Users**: Tài khoản người dùng, liên kết với Role và ServiceCenter.

### Nhóm 2: Sản phẩm & Khách hàng
*   **Customers**: Thông tin chủ sở hữu xe.
*   **Vehicles**: Thông tin định danh xe (VIN), Model, ngày mua, và khách hàng sở hữu.
*   **Parts**: Danh mục các loại phụ tùng (ví dụ: Pin 72V, Động cơ 3000W, Bộ điều khiển BMS).
*   **VehicleParts**: Lưu thông tin linh kiện **cụ thể** đang lắp trên xe (ví dụ: Xe VIN X đang lắp Pin có số seri S1).

### Nhóm 3: Quy trình Bảo hành (Core)
*   **WarrantyPolicies**: Chính sách bảo hành (ví dụ: Pin bảo hành 5 năm hoặc 50.000km).
*   **WarrantyClaims**: Các yêu cầu bảo hành được gửi lên.
*   **ClaimAttachments**: Hình ảnh bằng chứng, báo cáo chẩn đoán lỗi.

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
```
