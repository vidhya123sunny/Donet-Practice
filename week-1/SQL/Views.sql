-- ==========================================
-- Module: Views
-- Database: ViewsDB
-- ==========================================

IF DB_ID('ViewsDB') IS NOT NULL
DROP DATABASE ViewsDB;
GO

CREATE DATABASE ViewsDB;
GO

USE ViewsDB;
GO

-- ==========================================
-- TABLES
-- ==========================================

CREATE TABLE Departments
(
    DepartmentID INT PRIMARY KEY,
    DepartmentName VARCHAR(100)
);

CREATE TABLE Employees
(
    EmployeeID INT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    DepartmentID INT,
    Salary DECIMAL(10,2),
    JoinDate DATE,
    FOREIGN KEY (DepartmentID)
    REFERENCES Departments(DepartmentID)
);

-- ==========================================
-- SAMPLE DATA
-- ==========================================

INSERT INTO Departments
VALUES
(1,'HR'),
(2,'Finance'),
(3,'IT'),
(4,'Marketing');

INSERT INTO Employees
VALUES
(1,'John','Doe',1,5000,'2020-01-15'),
(2,'Jane','Smith',2,6000,'2019-03-22'),
(3,'Michael','Johnson',3,7000,'2018-07-30'),
(4,'Emily','Davis',4,5500,'2021-11-05');
GO

-- ==========================================
-- Exercise 1
-- Create Simple View
-- ==========================================

CREATE VIEW vw_EmployeeBasicInfo
AS
SELECT
    E.EmployeeID,
    E.FirstName,
    E.LastName,
    D.DepartmentName
FROM Employees E
JOIN Departments D
ON E.DepartmentID = D.DepartmentID;
GO

SELECT * FROM vw_EmployeeBasicInfo;
GO

-- ==========================================
-- Exercise 2
-- Computed Column Full Name
-- ==========================================

CREATE VIEW vw_EmployeeFullName
AS
SELECT
    EmployeeID,
    FirstName + ' ' + LastName AS FullName
FROM Employees;
GO

SELECT * FROM vw_EmployeeFullName;
GO

-- ==========================================
-- Exercise 3
-- Annual Salary
-- ==========================================

CREATE VIEW vw_EmployeeAnnualSalary
AS
SELECT
    EmployeeID,
    FirstName,
    LastName,
    Salary * 12 AS AnnualSalary
FROM Employees;
GO

SELECT * FROM vw_EmployeeAnnualSalary;
GO

-- ==========================================
-- Exercise 4
-- Employee Report
-- ==========================================

CREATE VIEW vw_EmployeeReport
AS
SELECT
    E.EmployeeID,
    E.FirstName + ' ' + E.LastName AS FullName,
    D.DepartmentName,
    E.Salary * 12 AS AnnualSalary,
    (E.Salary * 12) * 0.10 AS Bonus
FROM Employees E
JOIN Departments D
ON E.DepartmentID = D.DepartmentID;
GO

SELECT * FROM vw_EmployeeReport;