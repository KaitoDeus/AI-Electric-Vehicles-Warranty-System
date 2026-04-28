USE master;
GO

-- 1. Tạo Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EV_Warranty_DB')
BEGIN
    CREATE DATABASE EV_Warranty_DB;
END
GO

USE EV_Warranty_DB;
GO

-- 2. Xóa bảng cũ nếu tồn tại (theo thứ tự ngược lại của khóa ngoại để tránh lỗi)
IF OBJECT_ID('ClaimStatusHistory', 'U') IS NOT NULL DROP TABLE ClaimStatusHistory;
IF OBJECT_ID('ClaimDetails', 'U') IS NOT NULL DROP TABLE ClaimDetails;
IF OBJECT_ID('ClaimAttachments', 'U') IS NOT NULL DROP TABLE ClaimAttachments;
IF OBJECT_ID('WarrantyClaims', 'U') IS NOT NULL DROP TABLE WarrantyClaims;
IF OBJECT_ID('WarrantyPolicies', 'U') IS NOT NULL DROP TABLE WarrantyPolicies;
IF OBJECT_ID('CampaignVehicles', 'U') IS NOT NULL DROP TABLE CampaignVehicles;
IF OBJECT_ID('Campaigns', 'U') IS NOT NULL DROP TABLE Campaigns;
IF OBJECT_ID('SCInventory', 'U') IS NOT NULL DROP TABLE SCInventory;
IF OBJECT_ID('VehicleParts', 'U') IS NOT NULL DROP TABLE VehicleParts;
IF OBJECT_ID('Parts', 'U') IS NOT NULL DROP TABLE Parts;
IF OBJECT_ID('Vehicles', 'U') IS NOT NULL DROP TABLE Vehicles;
IF OBJECT_ID('Customers', 'U') IS NOT NULL DROP TABLE Customers;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('ServiceCenters', 'U') IS NOT NULL DROP TABLE ServiceCenters;
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles;
GO

-- 3. Tạo các bảng

-- Nhóm 1: Hệ thống & Phân quyền
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

-- Nhóm 2: Sản phẩm & Kho
CREATE TABLE Customers (
    CustomerID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
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
    CustomerID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Customers(CustomerID),
    CurrentMileage INT DEFAULT 0,
    Status NVARCHAR(50) DEFAULT 'Active'
);

CREATE TABLE Parts (
    PartID INT PRIMARY KEY IDENTITY(1,1),
    PartCode VARCHAR(50) UNIQUE,
    PartName NVARCHAR(200) NOT NULL,
    Category NVARCHAR(100), -- Battery, Motor, BMS, Inverter
    Unit NVARCHAR(20)
);

-- Quản lý tồn kho linh kiện tại từng Service Center
CREATE TABLE SCInventory (
    SCID INT FOREIGN KEY REFERENCES ServiceCenters(SCID),
    PartID INT FOREIGN KEY REFERENCES Parts(PartID),
    Quantity INT DEFAULT 0,
    LastUpdated DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (SCID, PartID)
);

-- Linh kiện cụ thể đang lắp trên từng xe (có Serial riêng)
CREATE TABLE VehicleParts (
    VehiclePartID INT PRIMARY KEY IDENTITY(1,1),
    VIN VARCHAR(17) FOREIGN KEY REFERENCES Vehicles(VIN),
    PartID INT FOREIGN KEY REFERENCES Parts(PartID),
    SerialNumber VARCHAR(100) UNIQUE NOT NULL,
    InstalledDate DATE DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Normal'
);

-- Nhóm 3: Quy trình Bảo hành
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

-- Chi tiết các linh kiện được xử lý trong một Claim
CREATE TABLE ClaimDetails (
    DetailID INT PRIMARY KEY IDENTITY(1,1),
    ClaimID INT FOREIGN KEY REFERENCES WarrantyClaims(ClaimID),
    VehiclePartID INT FOREIGN KEY REFERENCES VehicleParts(VehiclePartID), -- Linh kiện cũ bị lỗi
    ActionType NVARCHAR(50), -- Replacement, Repair, Inspection
    NewSerialNumber VARCHAR(100) NULL, -- Lưu seri linh kiện mới nếu thay thế
    Note NVARCHAR(MAX)
);

CREATE TABLE ClaimAttachments (
    AttachmentID INT PRIMARY KEY IDENTITY(1,1),
    ClaimID INT FOREIGN KEY REFERENCES WarrantyClaims(ClaimID),
    FileType NVARCHAR(50), -- Image, PDF, Log
    FilePath NVARCHAR(1000),
    UploadedAt DATETIME DEFAULT GETDATE()
);

-- Lịch sử thay đổi trạng thái (Audit Log) cho Claim
CREATE TABLE ClaimStatusHistory (
    HistoryID INT PRIMARY KEY IDENTITY(1,1),
    ClaimID INT FOREIGN KEY REFERENCES WarrantyClaims(ClaimID),
    FromStatus NVARCHAR(50),
    ToStatus NVARCHAR(50),
    ChangedBy INT FOREIGN KEY REFERENCES Users(UserID),
    ChangedAt DATETIME DEFAULT GETDATE(),
    Comment NVARCHAR(MAX)
);

-- Nhóm 4: Chiến dịch Triệu hồi (Recall)
CREATE TABLE Campaigns (
    CampaignID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX),
    StartDate DATE,
    EndDate DATE,
    Type NVARCHAR(50) DEFAULT 'Recall'
);

CREATE TABLE CampaignVehicles (
    CampaignID INT FOREIGN KEY REFERENCES Campaigns(CampaignID),
    VIN VARCHAR(17) FOREIGN KEY REFERENCES Vehicles(VIN),
    Status NVARCHAR(50) DEFAULT 'Not Started',
    CompletedAt DATETIME,
    PRIMARY KEY (CampaignID, VIN)
);
GO

-- 4. Chèn dữ liệu mẫu (Seed Data)
INSERT INTO Roles (RoleName) VALUES (N'Admin'), (N'EVM Staff'), (N'SC Staff'), (N'SC Technician');

INSERT INTO ServiceCenters (SCName, Address, Phone, Email) VALUES 
(N'VinFast Hà Nội - Long Biên', N'Số 1 Nguyễn Văn Linh, Hà Nội', '0243123456', 'sc.longbien@ev.com'),
(N'VinFast TP.HCM - Landmark 81', N'208 Nguyễn Hữu Cảnh, TP.HCM', '0283999888', 'sc.landmark@ev.com');

INSERT INTO Users (Username, PasswordHash, FullName, RoleID, SCID) VALUES 
('admin', '$2a$11$XWr3ekIg5a9ZBRxz7WY3/uv1qDdEumWrVU20HLhsTDbqpB/XrY2my', N'Quản trị viên', 1, NULL),
('evm_staff', '$2a$11$XWr3ekIg5a9ZBRxz7WY3/uv1qDdEumWrVU20HLhsTDbqpB/XrY2my', N'Nhân viên EVM', 2, NULL),
('sc_staff', '$2a$11$XWr3ekIg5a9ZBRxz7WY3/uv1qDdEumWrVU20HLhsTDbqpB/XrY2my', N'Nhân viên SC', 3, 1),
('sc_tech', '$2a$11$XWr3ekIg5a9ZBRxz7WY3/uv1qDdEumWrVU20HLhsTDbqpB/XrY2my', N'Kỹ thuật viên SC', 4, 1);

INSERT INTO Parts (PartCode, PartName, Category, Unit) VALUES 
('BAT-72V-LFP', N'Pin LFP 72V 100Ah', 'Battery', N'Cái'),
('MOT-3000W-MID', N'Động cơ Mid-Drive 3000W', 'Motor', N'Cái'),
('BMS-V1', N'Bộ quản lý pin VinFast V1', 'BMS', N'Cái');

-- Khởi tạo tồn kho mẫu cho trạm Long Biên
INSERT INTO SCInventory (SCID, PartID, Quantity) VALUES (1, 1, 10), (1, 2, 5);

PRINT 'Database script updated and executed successfully!';
GO
