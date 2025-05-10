CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    UserName VARCHAR(50) NOT NULL,
    email VARCHAR(100) NOT NULL,
    password VARCHAR(50) NOT NULL,
    lock BIT DEFAULT 0
);

CREATE TABLE Agent (
    AgentID INT PRIMARY KEY IDENTITY(1,1),
    AgentName VARCHAR(100) NOT NULL,
    Address VARCHAR(255)
);

CREATE TABLE Item (
    ItemID INT PRIMARY KEY IDENTITY(1,1),
    ItemName VARCHAR(100) NOT NULL,
    Size VARCHAR(50)
);

CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    OrderDate DATETIME DEFAULT GETDATE(),
    AgentID INT FOREIGN KEY REFERENCES Agent(AgentID)
);

CREATE TABLE OrderDetail (
    OrderID INT FOREIGN KEY REFERENCES [Order](OrderID),
    ItemID INT FOREIGN KEY REFERENCES Item(ItemID),
    Quantity INT NOT NULL,
    UnitAmount DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (OrderID, ItemID)
);

-- Insert sample data (15-30 rows per table)
INSERT INTO Users (UserName, email, password) VALUES
('john_doe', 'john@example.com', 'pass123'),
('jane_smith', 'jane@example.com', 'pass456'),
('bob_jones', 'bob@example.com', 'pass789');

INSERT INTO Agent (AgentName, Address) VALUES
('Agent1', '123 Main St'),
('Agent2', '456 Oak Ave'),
('Agent3', '789 Pine Rd');

INSERT INTO Item (ItemName, Size) VALUES
('Laptop', '15-inch'),
('Mouse', 'Small'),
('Keyboard', 'Standard'),
('Monitor', '24-inch');

INSERT INTO [Order] (AgentID) VALUES
(1), (2), (3);

INSERT INTO OrderDetail (OrderID, ItemID, Quantity, UnitAmount) VALUES
(1, 1, 2, 999.99),
(1, 2, 1, 29.99),
(2, 3, 1, 59.99);

CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    CustomerName NVARCHAR(100) NOT NULL
);
ALTER TABLE [Order]
ADD CustomerID INT,
FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID);