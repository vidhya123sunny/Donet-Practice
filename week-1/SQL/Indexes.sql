-- ==========================================
-- Module: Indexes
-- Database: IndexDB
-- ==========================================

USE master;
GO

IF DB_ID('IndexDB') IS NOT NULL
BEGIN
    ALTER DATABASE IndexDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE IndexDB;
END;
GO

CREATE DATABASE IndexDB;
GO

USE IndexDB;
GO

-- ==========================================
-- TABLE CREATION
-- ==========================================

-- If the Employees table already exists (from a previous run), drop it so
-- the script can be re-run without errors.
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Employees;
END;

CREATE TABLE dbo.Employees
(
    -- Keep the primary-key constraint nonclustered so Exercise 1 can
    -- explicitly create the table's clustered index.
    EmployeeID INT PRIMARY KEY NONCLUSTERED,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Department VARCHAR(50),
    Salary DECIMAL(10,2)
);

INSERT INTO dbo.Employees
    (EmployeeID, FirstName, LastName, Department, Salary)
VALUES
(1,'John','Doe','HR',5000),
(2,'Jane','Smith','IT',7000),
(3,'Bob','Johnson','Finance',6500),
(4,'Emily','Davis','IT',8000),
(5,'Michael','Brown','HR',5500);

-- ==========================================
-- Exercise 1
-- Create Clustered Index
-- ==========================================

CREATE CLUSTERED INDEX IX_EmployeeID
ON dbo.Employees(EmployeeID);

-- ==========================================
-- Exercise 2
-- Create Non-Clustered Index
-- ==========================================

CREATE NONCLUSTERED INDEX IX_LastName
ON dbo.Employees(LastName);

-- ==========================================
-- Exercise 3
-- Create Unique Index
-- ==========================================

CREATE UNIQUE INDEX IX_UniqueName
ON dbo.Employees(FirstName, LastName);

-- ==========================================
-- Exercise 4
-- Create Composite Index
-- ==========================================

CREATE NONCLUSTERED INDEX IX_DepartmentSalary
ON dbo.Employees(Department, Salary);

-- ==========================================
-- Exercise 5
-- View Existing Indexes
-- ==========================================

SELECT
    name AS IndexName,
    type_desc AS IndexType
FROM sys.indexes
WHERE object_id = OBJECT_ID('dbo.Employees');

-- ==========================================
-- Exercise 6
-- Test Index Performance
-- ==========================================

SELECT *
FROM dbo.Employees
WHERE LastName = 'Smith';

-- ==========================================
-- Exercise 7
-- Drop Index
-- ==========================================

DROP INDEX IX_LastName
ON dbo.Employees;
