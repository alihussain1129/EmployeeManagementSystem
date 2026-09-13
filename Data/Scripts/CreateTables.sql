CREATE DATABASE EmployeeManagementDB;
GO
USE EmployeeManagementDB;
GO

CREATE TABLE Departments (
    DepartmentId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Employees (
    EmployeeId INT PRIMARY KEY IDENTITY,
    FullName NVARCHAR(100) NOT NULL,
    CNIC NVARCHAR(20) UNIQUE,
    Designation NVARCHAR(100),
    DepartmentId INT FOREIGN KEY REFERENCES Departments(DepartmentId),
    JoiningDate DATE NOT NULL,
    BiometricId NVARCHAR(50) UNIQUE,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active'
);

CREATE TABLE Salaries (
    SalaryId INT PRIMARY KEY IDENTITY,
    EmployeeId INT FOREIGN KEY REFERENCES Employees(EmployeeId),
    BasicSalary DECIMAL(12,2) NOT NULL,
    Allowances DECIMAL(12,2) NOT NULL DEFAULT 0,
    Deductions DECIMAL(12,2) NOT NULL DEFAULT 0,
    EffectiveMonth DATE NOT NULL
);

CREATE TABLE Attendance (
    AttendanceId INT PRIMARY KEY IDENTITY,
    EmployeeId INT FOREIGN KEY REFERENCES Employees(EmployeeId),
    PunchTime DATETIME NOT NULL,
    PunchType NVARCHAR(10) NOT NULL,
    DeviceId NVARCHAR(50)
);