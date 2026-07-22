-- ==========================================
-- Module: Functions
-- Database: FunctionDB
-- ==========================================

IF DB_ID('FunctionDB') IS NOT NULL
DROP DATABASE FunctionDB;
GO

CREATE DATABASE FunctionDB;
GO

USE FunctionDB;
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
(2,'IT'),
(3,'Finance');

INSERT INTO Employees
VALUES
(1,'John','Doe',1,5000,'2020-01-15'),
(2,'Jane','Smith',2,6000,'2019-03-22'),
(3,'Bob','Johnson',3,5500,'2021-07-01');
GO

-- ==========================================
-- Exercise 1
-- Scalar Function
-- ==========================================

CREATE FUNCTION fn_CalculateAnnualSalary
(
    @Salary DECIMAL(10,2)
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    RETURN @Salary * 12;
END;
GO

SELECT
    EmployeeID,
    dbo.fn_CalculateAnnualSalary(Salary)
    AS AnnualSalary
FROM Employees;
GO

-- ==========================================
-- Exercise 2
-- Table Valued Function
-- ==========================================

CREATE FUNCTION fn_GetEmployeesByDepartment
(
    @DepartmentID INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT *
    FROM Employees
    WHERE DepartmentID = @DepartmentID
);
GO

SELECT *
FROM dbo.fn_GetEmployeesByDepartment(2);
GO

-- ==========================================
-- Exercise 3
-- Bonus Function
-- ==========================================

CREATE FUNCTION fn_CalculateBonus
(
    @Salary DECIMAL(10,2)
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    RETURN @Salary * 0.10;
END;
GO

SELECT
    EmployeeID,
    dbo.fn_CalculateBonus(Salary)
    AS Bonus
FROM Employees;
GO

-- ==========================================
-- Exercise 4
-- Modify Bonus Function
-- ==========================================

ALTER FUNCTION fn_CalculateBonus
(
    @Salary DECIMAL(10,2)
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    RETURN @Salary * 0.15;
END;
GO

SELECT
    EmployeeID,
    dbo.fn_CalculateBonus(Salary)
    AS ModifiedBonus
FROM Employees;
GO

-- ==========================================
-- Exercise 5
-- Execute Annual Salary Function
-- ==========================================

SELECT
    EmployeeID,
    dbo.fn_CalculateAnnualSalary(Salary)
    AS AnnualSalary
FROM Employees;
GO

-- ==========================================
-- Exercise 6
-- Specific Employee Annual Salary
-- ==========================================

SELECT
    EmployeeID,
    dbo.fn_CalculateAnnualSalary(Salary)
    AS AnnualSalary
FROM Employees
WHERE EmployeeID = 1;
GO

-- ==========================================
-- Exercise 7
-- Finance Department Employees
-- ==========================================

SELECT *
FROM dbo.fn_GetEmployeesByDepartment(3);
GO

-- ==========================================
-- Exercise 8
-- Nested Function
-- ==========================================

CREATE FUNCTION fn_CalculateTotalCompensation
(
    @Salary DECIMAL(10,2)
)
RETURNS DECIMAL(10,2)
AS
BEGIN

    RETURN
    dbo.fn_CalculateAnnualSalary(@Salary)
    +
    dbo.fn_CalculateBonus(@Salary);

END;
GO

SELECT
    EmployeeID,
    dbo.fn_CalculateTotalCompensation(Salary)
    AS TotalCompensation
FROM Employees;
GO

-- ==========================================
-- Exercise 9
-- Modify Nested Function
-- ==========================================

ALTER FUNCTION fn_CalculateTotalCompensation
(
    @Salary DECIMAL(10,2)
)
RETURNS DECIMAL(10,2)
AS
BEGIN

    RETURN
    (@Salary * 12)
    +
    (@Salary * 0.20);

END;
GO

SELECT
    EmployeeID,
    dbo.fn_CalculateTotalCompensation(Salary)
    AS UpdatedCompensation
FROM Employees;
GO

-- ==========================================
-- Exercise 10
-- Delete Function
-- ==========================================

DROP FUNCTION fn_CalculateBonus;
GO