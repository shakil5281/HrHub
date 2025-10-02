-- Create Sections table with Department relationship
-- This script can be run manually if migrations are having conflicts

-- Check if table already exists
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Sections' AND xtype='U')
BEGIN
    CREATE TABLE [Sections] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [DepartmentId] int NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [NameBangla] nvarchar(200) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedBy] nvarchar(128) NULL,
        [UpdatedBy] nvarchar(128) NULL,
        CONSTRAINT [PK_Sections] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sections_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE CASCADE
    );

    -- Create indexes
    CREATE INDEX [IX_Sections_Name] ON [Sections] ([Name]);
    CREATE INDEX [IX_Sections_DepartmentId] ON [Sections] ([DepartmentId]);
    CREATE INDEX [IX_Sections_IsActive] ON [Sections] ([IsActive]);
    CREATE UNIQUE INDEX [IX_Section_DepartmentId_Name] ON [Sections] ([DepartmentId], [Name]);

    PRINT 'Sections table created successfully with all indexes and foreign key constraints.';
END
ELSE
BEGIN
    PRINT 'Sections table already exists.';
END

-- Insert sample sections for IT Department (assuming IT Department has ID = 1)
IF EXISTS (SELECT * FROM [Departments] WHERE [Id] = 1 AND [Name] = 'Information Technology')
BEGIN
    -- Software Development Section
    IF NOT EXISTS (SELECT * FROM [Sections] WHERE [DepartmentId] = 1 AND [Name] = 'Software Development')
    BEGIN
        INSERT INTO [Sections] ([DepartmentId], [Name], [NameBangla], [CreatedAt], [IsActive])
        VALUES (1, 'Software Development', 'mdUIq¨vj †W‡fjc‡g›U', GETUTCDATE(), 1);
        
        PRINT 'Software Development section created successfully.';
    END

    -- System Administration Section
    IF NOT EXISTS (SELECT * FROM [Sections] WHERE [DepartmentId] = 1 AND [Name] = 'System Administration')
    BEGIN
        INSERT INTO [Sections] ([DepartmentId], [Name], [NameBangla], [CreatedAt], [IsActive])
        VALUES (1, 'System Administration', 'wm‡÷g A¨vWwgwb‡÷ªkb', GETUTCDATE(), 1);
        
        PRINT 'System Administration section created successfully.';
    END

    -- Network & Security Section
    IF NOT EXISTS (SELECT * FROM [Sections] WHERE [DepartmentId] = 1 AND [Name] = 'Network & Security')
    BEGIN
        INSERT INTO [Sections] ([DepartmentId], [Name], [NameBangla], [CreatedAt], [IsActive])
        VALUES (1, 'Network & Security', '†bUIqvK© I wbivcËv', GETUTCDATE(), 1);
        
        PRINT 'Network & Security section created successfully.';
    END
END
ELSE
BEGIN
    PRINT 'IT Department not found. Please create IT Department first.';
END
