USE HrHubDB;
GO

-- Create ZkDevices table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ZkDevices' AND xtype='U')
BEGIN
    CREATE TABLE [ZkDevices] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [DeviceName] nvarchar(50) NOT NULL,
        [IpAddress] nvarchar(15) NOT NULL,
        [Port] int NOT NULL,
        [SerialNumber] nvarchar(50) NULL,
        [ProductName] nvarchar(50) NULL,
        [MachineNumber] nvarchar(50) NULL,
        [UserCount] int NOT NULL DEFAULT 0,
        [AdminCount] int NOT NULL DEFAULT 0,
        [FpCount] int NOT NULL DEFAULT 0,
        [FcCount] int NOT NULL DEFAULT 0,
        [PasswordCount] int NOT NULL DEFAULT 0,
        [LogCount] int NOT NULL DEFAULT 0,
        [IsConnected] bit NOT NULL DEFAULT 0,
        [LastConnectionTime] datetime2 NULL,
        [LastLogDownloadTime] datetime2 NULL,
        [Location] nvarchar(100) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] nvarchar(50) NULL,
        [UpdatedBy] nvarchar(50) NULL,
        CONSTRAINT [PK_ZkDevices] PRIMARY KEY ([Id])
    );

    -- Create unique indexes
    CREATE UNIQUE INDEX [IX_ZkDevices_IpAddress] ON [ZkDevices] ([IpAddress]);
    CREATE UNIQUE INDEX [IX_ZkDevices_SerialNumber] ON [ZkDevices] ([SerialNumber]) WHERE [SerialNumber] IS NOT NULL;

    PRINT 'ZkDevices table created successfully!';
END
ELSE
BEGIN
    PRINT 'ZkDevices table already exists.';
END
GO

-- Create AttendanceLogs table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='AttendanceLogs' AND xtype='U')
BEGIN
    CREATE TABLE [AttendanceLogs] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [ZkDeviceId] int NOT NULL,
        [EmployeeId] nvarchar(50) NOT NULL,
        [EmployeeName] nvarchar(200) NULL,
        [LogTime] datetime2 NOT NULL,
        [LogType] nvarchar(20) NOT NULL,
        [VerificationMode] nvarchar(20) NULL,
        [WorkCode] nvarchar(20) NULL,
        [Remarks] nvarchar(500) NULL,
        [IsProcessed] bit NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [CreatedBy] nvarchar(50) NULL,
        [UpdatedBy] nvarchar(50) NULL,
        CONSTRAINT [PK_AttendanceLogs] PRIMARY KEY ([Id])
    );

    -- Create foreign key constraint
    ALTER TABLE [AttendanceLogs] 
    ADD CONSTRAINT [FK_AttendanceLogs_ZkDevices_ZkDeviceId] 
    FOREIGN KEY ([ZkDeviceId]) REFERENCES [ZkDevices] ([Id]) ON DELETE CASCADE;

    -- Create indexes for better performance
    CREATE INDEX [IX_AttendanceLogs_ZkDeviceId_EmployeeId_LogTime] ON [AttendanceLogs] ([ZkDeviceId], [EmployeeId], [LogTime]);
    CREATE INDEX [IX_AttendanceLogs_LogTime] ON [AttendanceLogs] ([LogTime]);
    CREATE INDEX [IX_AttendanceLogs_EmployeeId] ON [AttendanceLogs] ([EmployeeId]);

    PRINT 'AttendanceLogs table created successfully!';
END
ELSE
BEGIN
    PRINT 'AttendanceLogs table already exists.';
END
GO

PRINT 'All ZK device tables created successfully!';
