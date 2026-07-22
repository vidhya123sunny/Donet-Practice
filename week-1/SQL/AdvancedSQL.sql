-- ==========================================
-- Module: Advanced SQL
-- Database: AdvancedSQLDB
-- ==========================================

IF DB_ID('AdvancedSQLDB') IS NOT NULL
DROP DATABASE AdvancedSQLDB;
GO

CREATE DATABASE AdvancedSQLDB;
GO

USE AdvancedSQLDB;
GO

-- ==========================================
-- TABLES
-- ==========================================

CREATE TABLE Customers
(
    CustomerID INT PRIMARY KEY,
    CustomerName VARCHAR(100),
    Region VARCHAR(50)
);

CREATE TABLE Products
(
    ProductID INT PRIMARY KEY,
    ProductName VARCHAR(100),
    Category VARCHAR(50),
    Price DECIMAL(10,2)
);

CREATE TABLE Orders
(
    OrderID INT PRIMARY KEY,
    CustomerID INT,
    OrderDate DATE,
    FOREIGN KEY (CustomerID)
    REFERENCES Customers(CustomerID)
);

CREATE TABLE OrderDetails
(
    OrderDetailID INT PRIMARY KEY,
    OrderID INT,
    ProductID INT,
    Quantity INT,
    FOREIGN KEY (OrderID)
    REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID)
    REFERENCES Products(ProductID)
);

-- ==========================================
-- SAMPLE DATA
-- ==========================================

INSERT INTO Customers
VALUES
(1,'Prem','South'),
(2,'Rahul','North'),
(3,'Anil','South');

INSERT INTO Products
VALUES
(101,'Laptop','Electronics',50000),
(102,'Mouse','Electronics',500),
(103,'Keyboard','Electronics',1000),
(104,'Chair','Furniture',4000),
(105,'Table','Furniture',6000);

INSERT INTO Orders
VALUES
(1,1,'2025-01-10'),
(2,2,'2025-01-11'),
(3,1,'2025-01-12'),
(4,3,'2025-01-13');

INSERT INTO OrderDetails
VALUES
(1,1,101,2),
(2,1,102,5),
(3,2,104,3),
(4,3,105,2),
(5,4,101,1);

-- ==========================================
-- Exercise 1
-- Ranking and Window Functions
-- ==========================================

SELECT
    ProductID,
    ProductName,
    Category,
    Price,

    ROW_NUMBER() OVER
    (
        PARTITION BY Category
        ORDER BY Price DESC
    ) AS RowNum,

    RANK() OVER
    (
        PARTITION BY Category
        ORDER BY Price DESC
    ) AS RankNum,

    DENSE_RANK() OVER
    (
        PARTITION BY Category
        ORDER BY Price DESC
    ) AS DenseRankNum

FROM Products;

-- ==========================================
-- Exercise 2
-- GROUPING SETS, ROLLUP, CUBE
-- ==========================================

SELECT
    C.Region,
    P.Category,
    SUM(OD.Quantity) AS TotalQuantity
FROM Customers C
JOIN Orders O
ON C.CustomerID = O.CustomerID
JOIN OrderDetails OD
ON O.OrderID = OD.OrderID
JOIN Products P
ON P.ProductID = OD.ProductID
GROUP BY GROUPING SETS
(
    (C.Region),
    (P.Category),
    (C.Region,P.Category)
);

SELECT
    C.Region,
    P.Category,
    SUM(OD.Quantity) AS TotalQuantity
FROM Customers C
JOIN Orders O
ON C.CustomerID = O.CustomerID
JOIN OrderDetails OD
ON O.OrderID = OD.OrderID
JOIN Products P
ON P.ProductID = OD.ProductID
GROUP BY ROLLUP
(
    C.Region,
    P.Category
);

SELECT
    C.Region,
    P.Category,
    SUM(OD.Quantity) AS TotalQuantity
FROM Customers C
JOIN Orders O
ON C.CustomerID = O.CustomerID
JOIN OrderDetails OD
ON O.OrderID = OD.OrderID
JOIN Products P
ON P.ProductID = OD.ProductID
GROUP BY CUBE
(
    C.Region,
    P.Category
);

-- ==========================================
-- Exercise 3
-- Recursive CTE
-- ==========================================

WITH CalendarCTE AS
(
    SELECT CAST('2025-01-01' AS DATE) AS CalendarDate

    UNION ALL

    SELECT DATEADD(DAY,1,CalendarDate)
    FROM CalendarCTE
    WHERE CalendarDate < '2025-01-31'
)
SELECT *
FROM CalendarCTE
OPTION (MAXRECURSION 31);

-- ==========================================
-- Exercise 3
-- MERGE
-- ==========================================

CREATE TABLE StagingProducts
(
    ProductID INT,
    ProductName VARCHAR(100),
    Category VARCHAR(50),
    Price DECIMAL(10,2)
);

INSERT INTO StagingProducts
VALUES
(101,'Laptop','Electronics',55000),
(106,'Monitor','Electronics',12000);

MERGE Products AS Target
USING StagingProducts AS Source
ON Target.ProductID = Source.ProductID

WHEN MATCHED THEN
UPDATE
SET Target.Price = Source.Price

WHEN NOT MATCHED THEN
INSERT
(
    ProductID,
    ProductName,
    Category,
    Price
)
VALUES
(
    Source.ProductID,
    Source.ProductName,
    Source.Category,
    Source.Price
);

-- ==========================================
-- Exercise 4
-- PIVOT
-- ==========================================

SELECT *
FROM
(
    SELECT
        ProductID,
        MONTH(O.OrderDate) AS OrderMonth,
        Quantity
    FROM Orders O
    JOIN OrderDetails OD
    ON O.OrderID = OD.OrderID
) AS SourceTable

PIVOT
(
    SUM(Quantity)
    FOR OrderMonth IN
    (
        [1],[2],[3],[4],[5],[6],
        [7],[8],[9],[10],[11],[12]
    )
) AS PivotTable;

-- ==========================================
-- Exercise 4
-- UNPIVOT
-- ==========================================

SELECT ProductID,
       MonthNo,
       Quantity
FROM
(
    SELECT *
    FROM
    (
        SELECT
            ProductID,
            MONTH(O.OrderDate) AS OrderMonth,
            Quantity
        FROM Orders O
        JOIN OrderDetails OD
        ON O.OrderID = OD.OrderID
    ) AS SourceTable

    PIVOT
    (
        SUM(Quantity)
        FOR OrderMonth IN
        (
            [1],[2],[3],[4],[5],[6],
            [7],[8],[9],[10],[11],[12]
        )
    ) AS P
) X

UNPIVOT
(
    Quantity
    FOR MonthNo IN
    (
        [1],[2],[3],[4],[5],[6],
        [7],[8],[9],[10],[11],[12]
    )
) U;

-- ==========================================
-- Exercise 5
-- CTE
-- ==========================================

WITH CustomerOrderCounts AS
(
    SELECT
        CustomerID,
        COUNT(*) AS OrderCount
    FROM Orders
    GROUP BY CustomerID
)

SELECT
    C.CustomerID,
    C.CustomerName,
    CO.OrderCount
FROM Customers C
JOIN CustomerOrderCounts CO
ON C.CustomerID = CO.CustomerID
WHERE CO.OrderCount > 1;