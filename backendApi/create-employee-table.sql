-- Create Employee table with comprehensive schema and relational IDs
-- This script creates the Employee table manually to avoid EF migration conflicts

-- First, ensure the related tables exist (Department, Section, Designation)
-- This script assumes those tables are already created

CREATE TABLE [dbo].[Employees] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [NameBangla] nvarchar(200) NULL,
    [NIDNo] nvarchar(50) NOT NULL,
    [FatherName] nvarchar(200) NOT NULL,
    [MotherName] nvarchar(200) NOT NULL,
    [FatherNameBangla] nvarchar(200) NULL,
    [MotherNameBangla] nvarchar(200) NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Address] nvarchar(500) NOT NULL,
    [JoiningDate] datetime2 NOT NULL,
    [DepartmentId] int NOT NULL,
    [SectionId] int NOT NULL,
    [DesignationId] int NOT NULL,
    [GrossSalary] decimal(18,2) NOT NULL,
    [BasicSalary] decimal(18,2) NOT NULL,
    [BankAccountNo] nvarchar(50) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE()),
    [UpdatedAt] datetime2 NULL,
    [IsActive] bit NOT NULL DEFAULT 1,
    [CreatedBy] nvarchar(128) NULL,
    [UpdatedBy] nvarchar(128) NULL,
    
    -- Primary Key
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    
    -- Foreign Key Constraints
    CONSTRAINT [FK_Employees_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) 
        REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_Sections_SectionId] FOREIGN KEY ([SectionId]) 
        REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_Designations_DesignationId] FOREIGN KEY ([DesignationId]) 
        REFERENCES [Designations] ([Id]) ON DELETE NO ACTION
);

-- Create Indexes for better query performance
CREATE INDEX [IX_Employees_Name] ON [Employees] ([Name]);
CREATE UNIQUE INDEX [IX_Employees_NIDNo] ON [Employees] ([NIDNo]);
CREATE INDEX [IX_Employees_DepartmentId] ON [Employees] ([DepartmentId]);
CREATE INDEX [IX_Employees_SectionId] ON [Employees] ([SectionId]);
CREATE INDEX [IX_Employees_DesignationId] ON [Employees] ([DesignationId]);
CREATE INDEX [IX_Employees_JoiningDate] ON [Employees] ([JoiningDate]);
CREATE INDEX [IX_Employees_IsActive] ON [Employees] ([IsActive]);
CREATE INDEX [IX_Employees_BankAccountNo] ON [Employees] ([BankAccountNo]);

-- Add check constraints for data validation
ALTER TABLE [Employees] ADD CONSTRAINT [CK_Employees_GrossSalary] CHECK ([GrossSalary] >= 0);
ALTER TABLE [Employees] ADD CONSTRAINT [CK_Employees_BasicSalary] CHECK ([BasicSalary] >= 0);
ALTER TABLE [Employees] ADD CONSTRAINT [CK_Employees_DateOfBirth] CHECK ([DateOfBirth] < GETDATE());
ALTER TABLE [Employees] ADD CONSTRAINT [CK_Employees_JoiningDate] CHECK ([JoiningDate] <= GETDATE());

PRINT 'Employee table created successfully with all indexes and constraints.';
PRINT 'Note: This table uses relational IDs for Department, Section, and Designation.';
PRINT 'Note: NameBangla fields should be displayed using SutonnyMJ font.';
PRINT 'Note: NIDNo has a unique constraint to prevent duplicate National ID numbers.';
