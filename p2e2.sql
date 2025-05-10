CREATE DATABASE MyStoreDB;
GO

USE MyStoreDB;
GO

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL, -- Trong ứng dụng thực tế, hãy lưu mật khẩu đã mã hóa!
    FullName NVARCHAR(100),
    Role NVARCHAR(20)
);

CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    UnitPrice DECIMAL(18, 2) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0
);

CREATE TABLE Agents (
    AgentId INT PRIMARY KEY IDENTITY(1,1),
    AgentName NVARCHAR(100) NOT NULL,
    ContactInfo NVARCHAR(200),
    Email NVARCHAR(100)
);

CREATE TABLE Orders (
    OrderId INT PRIMARY KEY IDENTITY(1,1),
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    AgentId INT FOREIGN KEY REFERENCES Agents(AgentId),
    TotalAmount DECIMAL(18, 2),
    Status NVARCHAR(50) DEFAULT 'Pending'
);

CREATE TABLE OrderDetails (
    OrderDetailId INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL FOREIGN KEY REFERENCES Orders(OrderId),
    ProductId INT NOT NULL FOREIGN KEY REFERENCES Products(ProductId),
    Quantity INT NOT NULL,
    PriceAtPurchase DECIMAL(18, 2) NOT NULL,
    CONSTRAINT CK_Quantity CHECK (Quantity > 0)
);