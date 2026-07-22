-- ==========================================
-- Module: Stored Procedures
-- Database: StoredProcedureDB
-- ==========================================

IF DB_ID('StoredProcedureDB') IS NOT NULL
DROP DATABASE StoredProcedureDB;
GO

CREATE DATABASE StoredProcedureDB;
GO

USE StoredProcedureDB;
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
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
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
(
    FirstName,
    LastName,
    DepartmentID,
    Salary,
    JoinDate
)
VALUES
('John','Doe',1,5000,'2020-01-15'),
('Jane','Smith',2,6000,'2019-03-22'),
('Michael','Johnson',3,7000,'2018-07-30'),
('Emily','Davis',4,5500,'2021-11-05');
GO

-- ==========================================
-- Exercise 1
-- Create Stored Procedure
-- ==========================================

CREATE PROCEDURE sp_GetEmployeesByDepartment
    @DepartmentID INT
AS
BEGIN
    SELECT *
    FROM Employees
    WHERE DepartmentID = @DepartmentID;
END;
GO

-- ==========================================
-- Exercise 2
-- Modify Procedure
-- ==========================================

ALTER PROCEDURE sp_GetEmployeesByDepartment
    @DepartmentID INT
AS
BEGIN
    SELECT
        EmployeeID,
        FirstName,
        LastName,
        Salary
    FROM Employees
    WHERE DepartmentID = @DepartmentID;
END;
GO

-- ==========================================
-- Exercise 3
-- Insert Employee Procedure
-- ==========================================

CREATE PROCEDURE sp_InsertEmployee
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @DepartmentID INT,
    @Salary DECIMAL(10,2),
    @JoinDate DATE
AS
BEGIN
    INSERT INTO Employees
    (
        FirstName,
        LastName,
        DepartmentID,
        Salary,
        JoinDate
    )
    VALUES
    (
        @FirstName,
        @LastName,
        @DepartmentID,
        @Salary,
        @JoinDate
    );
END;
GO

-- ==========================================
-- Exercise 4
-- Execute Procedure
-- ==========================================

EXEC sp_GetEmployeesByDepartment 1;
GO

-- ==========================================
-- Exercise 5
-- Employee Count
-- ==========================================

CREATE PROCEDURE sp_GetEmployeeCount
    @DepartmentID INT
AS
BEGIN
    SELECT COUNT(*) AS EmployeeCount
    FROM Employees
    WHERE DepartmentID = @DepartmentID;
END;
GO

-- ==========================================
-- Exercise 6
-- Output Parameter
-- ==========================================

CREATE PROCEDURE sp_TotalSalary
    @DepartmentID INT,
    @TotalSalary DECIMAL(12,2) OUTPUT
AS
BEGIN
    SELECT
        @TotalSalary = SUM(Salary)
    FROM Employees
    WHERE DepartmentID = @DepartmentID;
END;
GO

DECLARE @SalaryTotal DECIMAL(12,2);

EXEC sp_TotalSalary
     1,
     @SalaryTotal OUTPUT;

SELECT @SalaryTotal AS TotalSalary;
GO

-- ==========================================
-- Exercise 7
-- Update Salary
-- ==========================================

CREATE PROCEDURE sp_UpdateEmployeeSalary
    @EmployeeID INT,
    @NewSalary DECIMAL(10,2)
AS
BEGIN
    UPDATE Employees
    SET Salary = @NewSalary
    WHERE EmployeeID = @EmployeeID;
END;
GO

EXEC sp_UpdateEmployeeSalary
     1,
     5500;
GO

-- ==========================================
-- Exercise 8
-- Bonus Procedure
-- ==========================================

CREATE PROCEDURE sp_GiveBonus
    @DepartmentID INT,
    @BonusAmount DECIMAL(10,2)
AS
BEGIN
    UPDATE Employees
    SET Salary = Salary + @BonusAmount
    WHERE DepartmentID = @DepartmentID;
END;
GO

EXEC sp_GiveBonus
     1,
     500;
GO

-- ==========================================
-- Exercise 9
-- Transaction Procedure
-- ==========================================

CREATE PROCEDURE sp_UpdateSalaryTransaction
    @EmployeeID INT,
    @Salary DECIMAL(10,2)
AS
BEGIN
    BEGIN TRANSACTION;

    UPDATE Employees
    SET Salary = @Salary
    WHERE EmployeeID = @EmployeeID;

    COMMIT TRANSACTION;
END;
GO

-- ==========================================
-- Exercise 10
-- Dynamic SQL
-- ==========================================

CREATE PROCEDURE sp_DynamicEmployeeSearch
    @ColumnName VARCHAR(50),
    @Value VARCHAR(100)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);

    SET @SQL =
    'SELECT * FROM Employees
     WHERE ' + @ColumnName +
     ' = ''' + @Value + '''';

    EXEC sp_executesql @SQL;
END;
GO

-- Example
EXEC sp_DynamicEmployeeSearch
     'FirstName',
     'John';
GO

-- ==========================================
-- Exercise 11
-- Error Handling
-- ==========================================

CREATE PROCEDURE sp_SafeSalaryUpdate
    @EmployeeID INT,
    @Salary DECIMAL(10,2)
AS
BEGIN
    BEGIN TRY

        UPDATE Employees
        SET Salary = @Salary
        WHERE EmployeeID = @EmployeeID;

        PRINT 'Salary Updated Successfully';

    END TRY

    BEGIN CATCH

        PRINT 'Error Occurred';

    END CATCH
END;
GO

-- ==========================================
-- Delete Procedure
-- ==========================================

DROP PROCEDURE sp_GetEmployeesByDepartment;
GO