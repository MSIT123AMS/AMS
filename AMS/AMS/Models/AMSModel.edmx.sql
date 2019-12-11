
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/11/2019 11:26:21
-- Generated from EDMX file: C:\Users\User\Source\Repos\MSIT123AMS\AMS\AMS\AMS\Models\AMSModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [AMSDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Annouuncements]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Annouuncements];
GO
IF OBJECT_ID(N'[dbo].[Attendances]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Attendances];
GO
IF OBJECT_ID(N'[dbo].[ClockInApply]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ClockInApply];
GO
IF OBJECT_ID(N'[dbo].[Departments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Departments];
GO
IF OBJECT_ID(N'[dbo].[Employees]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Employees];
GO
IF OBJECT_ID(N'[dbo].[LeaveRequests]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LeaveRequests];
GO
IF OBJECT_ID(N'[dbo].[OverTimeRequest]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OverTimeRequest];
GO
IF OBJECT_ID(N'[dbo].[ReviewStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ReviewStatus];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[WorkingDaySchedule]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkingDaySchedule];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ClockInApply'
CREATE TABLE [dbo].[ClockInApply] (
    [EmployeeID] nvarchar(50)  NOT NULL,
    [OnDuty] datetime  NOT NULL,
    [OffDuty] datetime  NULL,
    [ReviewStatusID] int  NULL,
    [RequestDate] datetime  NULL,
    [ReviewTime] datetime  NULL
);
GO

-- Creating table 'Departments'
CREATE TABLE [dbo].[Departments] (
    [DepartmentID] int IDENTITY(1,1) NOT NULL,
    [Manager] nchar(10)  NULL,
    [DepartmentName] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'OverTimeRequest'
CREATE TABLE [dbo].[OverTimeRequest] (
    [OverTimeRequestID] varchar(50)  NOT NULL,
    [EmployeeID] nvarchar(50)  NOT NULL,
    [RequestTime] datetime  NOT NULL,
    [StartTime] datetime  NOT NULL,
    [EndTime] datetime  NOT NULL,
    [OverTimePay] bit  NOT NULL,
    [OverTimeReason] nvarchar(max)  NOT NULL,
    [ReviewStatusID] int  NOT NULL,
    [ReviewTime] datetime  NULL
);
GO

-- Creating table 'ReviewStatus'
CREATE TABLE [dbo].[ReviewStatus] (
    [ReviewStatusID] int  NOT NULL,
    [ReviewStatus1] nvarchar(10)  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [EmployeeID] nvarchar(50)  NOT NULL,
    [Password] varbinary(max)  NOT NULL,
    [LockOff] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [LockEnabled] bit  NULL,
    [WrongTimes] int  NULL
);
GO

-- Creating table 'WorkingDaySchedule'
CREATE TABLE [dbo].[WorkingDaySchedule] (
    [Date] datetime  NOT NULL,
    [WorkingDay] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'LeaveRequests'
CREATE TABLE [dbo].[LeaveRequests] (
    [LeaveRequestID] nvarchar(50)  NOT NULL,
    [EmployeeID] nvarchar(50)  NOT NULL,
    [RequestTime] datetime  NOT NULL,
    [StartTime] datetime  NOT NULL,
    [EndTime] datetime  NOT NULL,
    [LeaveType] nvarchar(50)  NOT NULL,
    [LeaveReason] nvarchar(max)  NULL,
    [ReviewStatusID] int  NOT NULL,
    [ReviewTime] datetime  NULL,
    [Attachment] varbinary(max)  NULL
);
GO

-- Creating table 'Employees'
CREATE TABLE [dbo].[Employees] (
    [EmployeeID] nvarchar(50)  NOT NULL,
    [EmployeeName] nvarchar(20)  NOT NULL,
    [IDNumber] nvarchar(10)  NOT NULL,
    [DeputyPhone] nvarchar(24)  NULL,
    [Deputy] nvarchar(20)  NULL,
    [Marital] nvarchar(10)  NULL,
    [Email] nvarchar(50)  NULL,
    [Birthday] datetime  NOT NULL,
    [Leaveday] datetime  NULL,
    [Hireday] datetime  NOT NULL,
    [Address] nvarchar(60)  NOT NULL,
    [DepartmentID] int  NOT NULL,
    [Phone] nvarchar(24)  NOT NULL,
    [Photo] varbinary(max)  NULL,
    [JobStaus] nvarchar(10)  NULL,
    [JobTitle] nvarchar(20)  NULL,
    [EnglishName] nvarchar(50)  NULL,
    [gender] bit  NULL,
    [Notes] nvarchar(max)  NULL,
    [LineID] nvarchar(50)  NULL,
    [Education] nvarchar(50)  NULL,
    [FaceID] nvarchar(50)  NULL
);
GO

-- Creating table 'Annouuncements'
CREATE TABLE [dbo].[Annouuncements] (
    [AnnouuncementID] int IDENTITY(1,1) NOT NULL,
    [EmployeeID] nvarchar(50)  NOT NULL,
    [Title] nvarchar(50)  NOT NULL,
    [Detail] nvarchar(max)  NOT NULL,
    [Importance] nvarchar(10)  NOT NULL,
    [AnnounceTime] datetime  NULL
);
GO

-- Creating table 'Attendances'
CREATE TABLE [dbo].[Attendances] (
    [EmployeeID] nvarchar(50)  NOT NULL,
    [Date] datetime  NOT NULL,
    [OnDuty] datetime  NULL,
    [station] nvarchar(10)  NULL,
    [OffDuty] datetime  NULL,
    [savehours] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [EmployeeID], [OnDuty] in table 'ClockInApply'
ALTER TABLE [dbo].[ClockInApply]
ADD CONSTRAINT [PK_ClockInApply]
    PRIMARY KEY CLUSTERED ([EmployeeID], [OnDuty] ASC);
GO

-- Creating primary key on [DepartmentID] in table 'Departments'
ALTER TABLE [dbo].[Departments]
ADD CONSTRAINT [PK_Departments]
    PRIMARY KEY CLUSTERED ([DepartmentID] ASC);
GO

-- Creating primary key on [OverTimeRequestID] in table 'OverTimeRequest'
ALTER TABLE [dbo].[OverTimeRequest]
ADD CONSTRAINT [PK_OverTimeRequest]
    PRIMARY KEY CLUSTERED ([OverTimeRequestID] ASC);
GO

-- Creating primary key on [ReviewStatusID] in table 'ReviewStatus'
ALTER TABLE [dbo].[ReviewStatus]
ADD CONSTRAINT [PK_ReviewStatus]
    PRIMARY KEY CLUSTERED ([ReviewStatusID] ASC);
GO

-- Creating primary key on [EmployeeID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([EmployeeID] ASC);
GO

-- Creating primary key on [Date] in table 'WorkingDaySchedule'
ALTER TABLE [dbo].[WorkingDaySchedule]
ADD CONSTRAINT [PK_WorkingDaySchedule]
    PRIMARY KEY CLUSTERED ([Date] ASC);
GO

-- Creating primary key on [LeaveRequestID] in table 'LeaveRequests'
ALTER TABLE [dbo].[LeaveRequests]
ADD CONSTRAINT [PK_LeaveRequests]
    PRIMARY KEY CLUSTERED ([LeaveRequestID] ASC);
GO

-- Creating primary key on [EmployeeID] in table 'Employees'
ALTER TABLE [dbo].[Employees]
ADD CONSTRAINT [PK_Employees]
    PRIMARY KEY CLUSTERED ([EmployeeID] ASC);
GO

-- Creating primary key on [AnnouuncementID] in table 'Annouuncements'
ALTER TABLE [dbo].[Annouuncements]
ADD CONSTRAINT [PK_Annouuncements]
    PRIMARY KEY CLUSTERED ([AnnouuncementID] ASC);
GO

-- Creating primary key on [EmployeeID], [Date] in table 'Attendances'
ALTER TABLE [dbo].[Attendances]
ADD CONSTRAINT [PK_Attendances]
    PRIMARY KEY CLUSTERED ([EmployeeID], [Date] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------