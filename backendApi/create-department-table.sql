-- Create Departments table
-- This script can be run manually if migrations are having conflicts

-- Check if table already exists
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Departments' AND xtype='U')
BEGIN
    CREATE TABLE [Departments] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [NameBangla] nvarchar(200) NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedBy] nvarchar(128) NULL,
        [UpdatedBy] nvarchar(128) NULL,
        CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
    );

    -- Create indexes
    CREATE INDEX [IX_Departments_Name] ON [Departments] ([Name]);
    CREATE INDEX [IX_Departments_IsActive] ON [Departments] ([IsActive]);

    PRINT 'Departments table created successfully with all indexes.';
END
ELSE
BEGIN
    PRINT 'Departments table already exists.';
END

-- Insert IT Department
IF NOT EXISTS (SELECT * FROM [Departments] WHERE [Name] = 'Information Technology')
BEGIN
    INSERT INTO [Departments] ([Name], [NameBangla], [CreatedAt], [IsActive])
    VALUES ('Information Technology', 'Z_¨cÖhyw³wZ', GETUTCDATE(), 1);
    
    PRINT 'IT Department created successfully.';
END
ELSE
BEGIN
    PRINT 'IT Department already exists.';
END
