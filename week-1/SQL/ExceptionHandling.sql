-- ==========================================
-- Module: Exception Handling
-- Database: ExceptionDB
-- ==========================================

USE master;
GO

IF DB_ID('ExceptionDB') IS NOT NULL
BEGIN
    ALTER DATABASE ExceptionDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ExceptionDB;
END;
GO

CREATE DATABASE ExceptionDB;
GO

USE ExceptionDB;
GO

IF OBJECT_ID('dbo.AuditLog','U') IS NOT NULL DROP TABLE dbo.AuditLog;
GO
IF OBJECT_ID('dbo.Employees','U') IS NOT NULL DROP TABLE dbo.Employees;
GO
IF OBJECT_ID('dbo.Departments','U') IS NOT NULL DROP TABLE dbo.Departments;
GO

-- ==========================================
-- TABLES
-- ==========================================

CREATE TABLE dbo.Departments
(
    DepartmentID INT PRIMARY KEY,
    DepartmentName VARCHAR(100) NOT NULL
);
GO

CREATE TABLE dbo.Employees
(
    EmployeeID INT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100) UNIQUE,
    Salary DECIMAL(10,2),
    DepartmentID INT,
    FOREIGN KEY (DepartmentID)
    REFERENCES dbo.Departments(DepartmentID)
);
GO

CREATE TABLE dbo.AuditLog
(
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    Action VARCHAR(100),
    ErrorMessage VARCHAR(4000),
    ActionDate DATETIME DEFAULT GETDATE()
);
GO

-- ==========================================
-- SAMPLE DATA
-- ==========================================

INSERT INTO dbo.Departments (DepartmentID, DepartmentName)
VALUES
(1,'HR'),
(2,'IT'),
(3,'Finance');

INSERT INTO dbo.Employees
    (EmployeeID, FirstName, LastName, Email, Salary, DepartmentID)
VALUES
(1,'John','Doe','john@gmail.com',5000,1),
(2,'Jane','Smith','jane@gmail.com',6000,2);
GO

-- ==========================================
-- Exercise 1
-- Basic TRY...CATCH
-- ==========================================

CREATE OR ALTER PROCEDURE dbo.AddEmployee
    @EmployeeID INT,
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @Email VARCHAR(100),
    @Salary DECIMAL(10,2),
    @DepartmentID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        INSERT INTO dbo.Employees
            (EmployeeID, FirstName, LastName, Email, Salary, DepartmentID)
        VALUES
        (
            @EmployeeID,
            @FirstName,
            @LastName,
            @Email,
            @Salary,
            @DepartmentID
        );

    END TRY

    BEGIN CATCH

        INSERT INTO dbo.AuditLog
        (
            Action,
            ErrorMessage
        )
        VALUES
        (
            'Insert Employee',
            ERROR_MESSAGE()
        );

        THROW;

    END CATCH

END;
GO

-- ==========================================
-- Exercise 2
-- THROW
-- ==========================================

ALTER PROCEDURE dbo.AddEmployee
    @EmployeeID INT,
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @Email VARCHAR(100),
    @Salary DECIMAL(10,2),
    @DepartmentID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        INSERT INTO dbo.Employees
            (EmployeeID, FirstName, LastName, Email, Salary, DepartmentID)
        VALUES
        (
            @EmployeeID,
            @FirstName,
            @LastName,
            @Email,
            @Salary,
            @DepartmentID
        );

    END TRY

    BEGIN CATCH

        INSERT INTO dbo.AuditLog
        (
            Action,
            ErrorMessage
        )
        VALUES
        (
            'Insert Employee',
            ERROR_MESSAGE()
        );

        THROW;

    END CATCH

END;
GO

-- ==========================================
-- Exercise 3
-- RAISERROR
-- ==========================================

ALTER PROCEDURE dbo.AddEmployee
    @EmployeeID INT,
    @FirstName VARCHAR(50),
    @LastName VARCHAR(50),
    @Email VARCHAR(100),
    @Salary DECIMAL(10,2),
    @DepartmentID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        IF @Salary <= 0
        BEGIN
            RAISERROR
            (
                'Salary must be greater than zero.',
                16,
                1
            );
        END

        INSERT INTO dbo.Employees
            (EmployeeID, FirstName, LastName, Email, Salary, DepartmentID)
        VALUES
        (
            @EmployeeID,
            @FirstName,
            @LastName,
            @Email,
            @Salary,
            @DepartmentID
        );

    END TRY

    BEGIN CATCH

        INSERT INTO dbo.AuditLog
        (
            Action,
            ErrorMessage
        )
        VALUES
        (
            'Add Employee (Exercise 3)',
            ERROR_MESSAGE()
        );

        THROW;

    END CATCH

END;
GO

-- ==========================================
-- Exercise 4
-- Nested TRY...CATCH
-- ==========================================

CREATE OR ALTER PROCEDURE dbo.TransferEmployee
    @EmployeeID INT,
    @DepartmentID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        BEGIN TRY

            IF NOT EXISTS
            (
                SELECT *
                FROM dbo.Departments
                WHERE DepartmentID = @DepartmentID
            )
            BEGIN

                RAISERROR
                (
                    'Department does not exist.',
                    16,
                    1
                );

            END

            UPDATE dbo.Employees
            SET DepartmentID = @DepartmentID
            WHERE EmployeeID = @EmployeeID;

        END TRY

        BEGIN CATCH

            INSERT INTO dbo.AuditLog
            (
                Action,
                ErrorMessage
            )
            VALUES
            (
                'Transfer Employee',
                ERROR_MESSAGE()
            );

            THROW;

        END CATCH

    END TRY

    BEGIN CATCH

        PRINT ERROR_MESSAGE();
        THROW;

    END CATCH

END;
GO

-- ==========================================
-- Exercise 5
-- Transaction with Error Logging
-- ==========================================

CREATE OR ALTER PROCEDURE dbo.BatchInsertEmployees
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        INSERT INTO dbo.Employees
            (EmployeeID, FirstName, LastName, Email, Salary, DepartmentID)
        VALUES
        (
            3,
            'Bob',
            'Johnson',
            'bob@gmail.com',
            5500,
            3
        );

        INSERT INTO dbo.Employees
            (EmployeeID, FirstName, LastName, Email, Salary, DepartmentID)
        VALUES
        (
            4,
            'Emily',
            'Davis',
            'emily@gmail.com',
            6500,
            2
        );

        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        INSERT INTO dbo.AuditLog
        (
            Action,
            ErrorMessage
        )
        VALUES
        (
            'Batch Insert',
            ERROR_MESSAGE()
        );

        THROW;

    END CATCH

END;
GO

-- ==========================================
-- Exercise 6
-- Dynamic RAISERROR
-- ==========================================

CREATE OR ALTER PROCEDURE dbo.ValidateSalary
    @Salary DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Salary < 0
    BEGIN

        RAISERROR
        (
            'Negative salary not allowed.',
            16,
            1
        );
        RETURN;

    END

    ELSE IF @Salary < 1000
    BEGIN

        RAISERROR
        (
            'Salary is very low.',
            10,
            1
        );
        RETURN;

    END

    ELSE
    BEGIN

        PRINT 'Salary Accepted';

    END

END;
GO

-- ==========================================
-- TESTING
-- ==========================================

BEGIN TRY
    EXEC dbo.ValidateSalary @Salary = -100;
END TRY
BEGIN CATCH
    PRINT ERROR_MESSAGE();
END CATCH;
GO

BEGIN TRY
    EXEC dbo.ValidateSalary @Salary = 500;
END TRY
BEGIN CATCH
    PRINT ERROR_MESSAGE();
END CATCH;
GO

BEGIN TRY
    EXEC dbo.ValidateSalary @Salary = 5000;
END TRY
BEGIN CATCH
    PRINT ERROR_MESSAGE();
END CATCH;
GO

SELECT *
FROM dbo.AuditLog;
GO

-- Verify employee data
SELECT *
FROM dbo.Employees;
GO

USE master;
GO

