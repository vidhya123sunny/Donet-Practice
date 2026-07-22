-- ==========================================
-- Module: Triggers
-- Database: TriggerDB
-- ==========================================

USE master;
GO

IF DB_ID('TriggerDB') IS NOT NULL
BEGIN
    ALTER DATABASE TriggerDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TriggerDB;
END;
GO

CREATE DATABASE TriggerDB;
GO

USE TriggerDB;
GO

-- ==========================================
-- TABLES
-- ==========================================

DROP TABLE IF EXISTS dbo.EmployeeChanges;
DROP TABLE IF EXISTS dbo.Employees;
DROP TABLE IF EXISTS dbo.Departments;

CREATE TABLE dbo.Departments
(
    DepartmentID INT PRIMARY KEY,
    DepartmentName VARCHAR(100)
);

CREATE TABLE dbo.Employees
(
    EmployeeID INT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    DepartmentID INT,
    Salary DECIMAL(10,2),
    JoinDate DATE,
    FOREIGN KEY (DepartmentID)
    REFERENCES dbo.Departments(DepartmentID)
);

CREATE TABLE dbo.EmployeeChanges
(
    ChangeID INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeID INT,
    OldSalary DECIMAL(10,2),
    NewSalary DECIMAL(10,2),
    ChangeDate DATETIME DEFAULT GETDATE()
);

-- ==========================================
-- SAMPLE DATA
-- ==========================================

INSERT INTO dbo.Departments (DepartmentID, DepartmentName)
VALUES
(1,'HR'),
(2,'Finance'),
(3,'IT'),
(4,'Marketing');

INSERT INTO dbo.Employees
    (EmployeeID, FirstName, LastName, DepartmentID, Salary, JoinDate)
VALUES
(1,'John','Doe',1,5000,'2022-01-15'),
(2,'Jane','Smith',2,6000,'2021-03-22'),
(3,'Michael','Johnson',3,7000,'2020-07-30'),
(4,'Emily','Davis',4,5500,'2019-11-05');
GO

-- ==========================================
-- Exercise 1
-- AFTER Trigger
-- ==========================================

DROP TRIGGER IF EXISTS dbo.trg_AfterSalaryUpdate;
GO

CREATE TRIGGER dbo.trg_AfterSalaryUpdate
ON dbo.Employees
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(Salary)
        RETURN;

    INSERT INTO dbo.EmployeeChanges
    (
        EmployeeID,
        OldSalary,
        NewSalary
    )

    SELECT
        d.EmployeeID,
        d.Salary,
        i.Salary
    FROM deleted d
    JOIN inserted i
        ON d.EmployeeID = i.EmployeeID
    WHERE d.Salary <> i.Salary
       OR (d.Salary IS NULL AND i.Salary IS NOT NULL)
       OR (d.Salary IS NOT NULL AND i.Salary IS NULL);

END;
GO

UPDATE dbo.Employees
SET Salary = 7500
WHERE EmployeeID = 1;
GO

SELECT * FROM dbo.EmployeeChanges;
GO

-- ==========================================
-- Exercise 2
-- INSTEAD OF DELETE Trigger
-- ==========================================

CREATE TRIGGER dbo.trg_PreventDelete
ON dbo.Employees
INSTEAD OF DELETE
AS
BEGIN
    SET NOCOUNT ON;

    PRINT 'Employee deletion is not allowed.';

END;
GO

-- Test
DELETE FROM dbo.Employees
WHERE EmployeeID = 1;
GO

-- ==========================================
-- Exercise 3
-- LOGON Trigger
-- ==========================================

IF HAS_PERMS_BY_NAME(NULL, NULL, 'CONTROL SERVER') = 1
BEGIN
    EXEC(N'
        CREATE OR ALTER TRIGGER trg_LogonRestriction
        ON ALL SERVER
        FOR LOGON
        AS
        BEGIN
            IF DATEPART(HOUR, GETDATE()) BETWEEN 2 AND 3
               AND IS_SRVROLEMEMBER(''sysadmin'') = 0
            BEGIN
                ROLLBACK;
            END;
        END;
    ');

    -- Keep this training trigger disabled to avoid locking out users.
    DISABLE TRIGGER trg_LogonRestriction ON ALL SERVER;
END
ELSE
BEGIN
    PRINT 'Exercise 3 skipped: CONTROL SERVER permission is required.';
END;
GO

-- ==========================================
-- Exercise 4
-- Modify Trigger
-- ==========================================

ALTER TRIGGER dbo.trg_AfterSalaryUpdate
ON dbo.Employees
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(Salary)
        RETURN;

    PRINT 'Salary Updated Successfully';

    INSERT INTO dbo.EmployeeChanges
    (
        EmployeeID,
        OldSalary,
        NewSalary
    )

    SELECT
        d.EmployeeID,
        d.Salary,
        i.Salary
    FROM deleted d
    JOIN inserted i
        ON d.EmployeeID = i.EmployeeID
    WHERE d.Salary <> i.Salary
       OR (d.Salary IS NULL AND i.Salary IS NOT NULL)
       OR (d.Salary IS NOT NULL AND i.Salary IS NULL);

END;
GO

-- ==========================================
-- Exercise 5
-- Delete Trigger
-- ==========================================

DROP TRIGGER dbo.trg_PreventDelete;
GO

-- ==========================================
-- Exercise 6
-- Computed Column Trigger
-- ==========================================

IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Employees' AND COLUMN_NAME = 'AnnualSalary'
)
ALTER TABLE dbo.Employees
ADD AnnualSalary DECIMAL(12,2);
GO

EXEC sp_executesql N'
    UPDATE dbo.Employees
    SET AnnualSalary = Salary * 12;
';
GO

DECLARE @triggerSQL NVARCHAR(MAX) = N'
CREATE TRIGGER dbo.trg_UpdateAnnualSalary
ON dbo.Employees
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(Salary)
        RETURN;

    UPDATE e
    SET e.AnnualSalary = i.Salary * 12
    FROM dbo.Employees e
    JOIN inserted i
        ON e.EmployeeID = i.EmployeeID;
END;
';
EXEC sp_executesql @triggerSQL;
GO

UPDATE dbo.Employees
SET Salary = 8000
WHERE EmployeeID = 2;
GO

DECLARE @sql NVARCHAR(MAX) = N'';
IF COL_LENGTH('dbo.Employees', 'AnnualSalary') IS NOT NULL
BEGIN
    SET @sql = N'SELECT EmployeeID, Salary, AnnualSalary FROM dbo.Employees;';
END
ELSE
BEGIN
    SET @sql = N'SELECT EmployeeID, Salary, (Salary * 12) AS AnnualSalary FROM dbo.Employees;';
END
EXEC sp_executesql @sql;
GO

-- Remove the server-scoped training trigger so it cannot affect later sessions.
IF HAS_PERMS_BY_NAME(NULL, NULL, 'CONTROL SERVER') = 1
   AND EXISTS
   (
       SELECT 1
       FROM sys.server_triggers
       WHERE name = 'trg_LogonRestriction'
   )
BEGIN
    DROP TRIGGER trg_LogonRestriction ON ALL SERVER;
END;
GO
