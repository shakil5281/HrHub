-- Create UserCompanies table for many-to-many relationship between Users and Companies
-- This script can be run manually if migrations are having conflicts

-- Check if table already exists
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserCompanies' AND xtype='U')
BEGIN
    CREATE TABLE [UserCompanies] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [UserId] nvarchar(128) NOT NULL,
        [CompanyId] int NOT NULL,
        [AssignedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [AssignedBy] nvarchar(128) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        CONSTRAINT [PK_UserCompanies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserCompanies_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserCompanies_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE
    );

    -- Create indexes
    CREATE INDEX [IX_UserCompanies_CompanyId] ON [UserCompanies] ([CompanyId]);
    CREATE INDEX [IX_UserCompanies_IsActive] ON [UserCompanies] ([IsActive]);
    CREATE INDEX [IX_UserCompanies_UserId] ON [UserCompanies] ([UserId]);
    CREATE UNIQUE INDEX [IX_UserCompany_UserId_CompanyId] ON [UserCompanies] ([UserId], [CompanyId]);

    PRINT 'UserCompanies table created successfully with all indexes and constraints.';
END
ELSE
BEGIN
    PRINT 'UserCompanies table already exists.';
END
