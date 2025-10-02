-- Create Designations table with Section relationship
-- This script can be run manually if migrations are having conflicts

-- Check if table already exists
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Designations' AND xtype='U')
BEGIN
    CREATE TABLE [Designations] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [SectionId] int NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [NameBangla] nvarchar(200) NULL,
        [Grade] nvarchar(50) NOT NULL,
        [AttendanceBonus] decimal(18,2) NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedBy] nvarchar(128) NULL,
        [UpdatedBy] nvarchar(128) NULL,
        CONSTRAINT [PK_Designations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Designations_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE CASCADE
    );

    -- Create indexes
    CREATE INDEX [IX_Designations_Name] ON [Designations] ([Name]);
    CREATE INDEX [IX_Designations_SectionId] ON [Designations] ([SectionId]);
    CREATE INDEX [IX_Designations_Grade] ON [Designations] ([Grade]);
    CREATE INDEX [IX_Designations_IsActive] ON [Designations] ([IsActive]);
    CREATE UNIQUE INDEX [IX_Designation_SectionId_Name] ON [Designations] ([SectionId], [Name]);

    PRINT 'Designations table created successfully with all indexes and foreign key constraints.';
END
ELSE
BEGIN
    PRINT 'Designations table already exists.';
END

-- Insert sample designations for Software Development Section (assuming Section ID = 1)
IF EXISTS (SELECT * FROM [Sections] WHERE [Id] = 1 AND [Name] = 'Software Development')
BEGIN
    -- Senior Software Engineer
    IF NOT EXISTS (SELECT * FROM [Designations] WHERE [SectionId] = 1 AND [Name] = 'Senior Software Engineer')
    BEGIN
        INSERT INTO [Designations] ([SectionId], [Name], [NameBangla], [Grade], [AttendanceBonus], [CreatedAt], [IsActive])
        VALUES (1, 'Senior Software Engineer', 'wmwbqi mdUIq¨vj BwÄwbqvi', 'A', 5000.00, GETUTCDATE(), 1);
        
        PRINT 'Senior Software Engineer designation created successfully.';
    END

    -- Software Engineer
    IF NOT EXISTS (SELECT * FROM [Designations] WHERE [SectionId] = 1 AND [Name] = 'Software Engineer')
    BEGIN
        INSERT INTO [Designations] ([SectionId], [Name], [NameBangla], [Grade], [AttendanceBonus], [CreatedAt], [IsActive])
        VALUES (1, 'Software Engineer', 'mdUIq¨vj BwÄwbqvi', 'B', 3000.00, GETUTCDATE(), 1);
        
        PRINT 'Software Engineer designation created successfully.';
    END

    -- Junior Software Engineer
    IF NOT EXISTS (SELECT * FROM [Designations] WHERE [SectionId] = 1 AND [Name] = 'Junior Software Engineer')
    BEGIN
        INSERT INTO [Designations] ([SectionId], [Name], [NameBangla], [Grade], [AttendanceBonus], [CreatedAt], [IsActive])
        VALUES (1, 'Junior Software Engineer', 'RywbAi mdUIq¨vj BwÄwbqvi', 'C', 2000.00, GETUTCDATE(), 1);
        
        PRINT 'Junior Software Engineer designation created successfully.';
    END

    -- Team Lead
    IF NOT EXISTS (SELECT * FROM [Designations] WHERE [SectionId] = 1 AND [Name] = 'Team Lead')
    BEGIN
        INSERT INTO [Designations] ([SectionId], [Name], [NameBangla], [Grade], [AttendanceBonus], [CreatedAt], [IsActive])
        VALUES (1, 'Team Lead', 'UxW jxW', 'A+', 7500.00, GETUTCDATE(), 1);
        
        PRINT 'Team Lead designation created successfully.';
    END
END
ELSE
BEGIN
    PRINT 'Software Development section not found. Please create sections first.';
END
