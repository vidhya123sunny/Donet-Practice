-- ==========================================
-- Module: Cursors
-- Database: CursorDB
-- ==========================================

IF DB_ID('CursorDB') IS NOT NULL
DROP DATABASE CursorDB;
GO

CREATE DATABASE CursorDB;
GO

USE CursorDB;
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
(3,'Bob','Johnson',3,5500,'2021-07-30');

-- ==========================================
-- Exercise 1
-- Create Cursor
-- ==========================================

DECLARE
    @EmployeeID INT,
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50);

DECLARE EmployeeCursor CURSOR
FOR
SELECT
    EmployeeID,
    FirstName,
    LastName
FROM Employees;

OPEN EmployeeCursor;

FETCH NEXT
FROM EmployeeCursor
INTO
    @EmployeeID,
    @FirstName,
    @LastName;

WHILE @@FETCH_STATUS = 0
BEGIN

    PRINT
    'Employee ID: '
    + CAST(@EmployeeID AS VARCHAR)
    + ' Name: '
    + @FirstName
    + ' '
    + @LastName;

    FETCH NEXT
    FROM EmployeeCursor
    INTO
        @EmployeeID,
        @FirstName,
        @LastName;

END;

CLOSE EmployeeCursor;
DEALLOCATE EmployeeCursor;
GO

-- ==========================================
-- Exercise 2
-- Types of Cursors
-- ==========================================

-- Static Cursor

DECLARE StaticCursor CURSOR STATIC
FOR
SELECT *
FROM Employees;

OPEN StaticCursor;
CLOSE StaticCursor;
DEALLOCATE StaticCursor;
GO

-- Dynamic Cursor

DECLARE DynamicCursor CURSOR DYNAMIC
FOR
SELECT *
FROM Employees;

OPEN DynamicCursor;
CLOSE DynamicCursor;
DEALLOCATE DynamicCursor;
GO

-- Forward Only Cursor

DECLARE ForwardCursor CURSOR FORWARD_ONLY
FOR
SELECT *
FROM Employees;

OPEN ForwardCursor;
CLOSE ForwardCursor;
DEALLOCATE ForwardCursor;
GO

-- Keyset Cursor

DECLARE KeysetCursor CURSOR KEYSET
FOR
SELECT *
FROM Employees;

OPEN KeysetCursor;
CLOSE KeysetCursor;
DEALLOCATE KeysetCursor;
GO