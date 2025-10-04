USE HrHubDB;
GO

-- Insert the 5 ZK devices based on your ZKTeco software
INSERT INTO ZkDevices (
    DeviceName, 
    IpAddress, 
    Port, 
    SerialNumber, 
    ProductName, 
    MachineNumber, 
    UserCount, 
    AdminCount, 
    FpCount, 
    FcCount, 
    PasswordCount, 
    LogCount, 
    IsConnected, 
    Location, 
    IsActive, 
    CreatedAt, 
    UpdatedAt, 
    CreatedBy, 
    UpdatedBy
) VALUES 
-- Device 1: 192.168.1.201:4370 - 57 logs - Serial: CGT9214760112
('1', '192.168.1.201', 4370, 'CGT9214760112', 'F18/ID', '101', 1340, 1, 1432, 0, 0, 57, 0, 'Main Office', 1, GETUTCDATE(), GETUTCDATE(), 'System', 'System'),

-- Device 2: 192.168.1.202:4370 - 30 logs - Serial: AIOR210460094
('2', '192.168.1.202', 4370, 'AIOR210460094', 'F18/ID', '102', 1342, 1, 1419, 0, 0, 30, 0, 'Main Office', 1, GETUTCDATE(), GETUTCDATE(), 'System', 'System'),

-- Device 3: 192.168.1.203:4370 - 9 logs - Serial: BAY5234201212
('3', '192.168.1.203', 4370, 'BAY5234201212', 'F18', '103', 1342, 1, 1439, 0, 0, 9, 0, 'Main Office', 1, GETUTCDATE(), GETUTCDATE(), 'System', 'System'),

-- Device 4: 192.168.1.204:4370 - 60 logs - Serial: AIOR205160196
('4', '192.168.1.204', 4370, 'AIOR205160196', 'F18/ID', '104', 1342, 1, 1430, 0, 0, 60, 0, 'Main Office', 1, GETUTCDATE(), GETUTCDATE(), 'System', 'System'),

-- Device 5: 192.168.1.220:4370 - 49 logs - Serial: CQQC225261297
('5', '192.168.1.220', 4370, 'CQQC225261297', 'F18/ID', '105', 1343, 1, 1430, 0, 0, 49, 0, 'Main Office', 1, GETUTCDATE(), GETUTCDATE(), 'System', 'System');

PRINT 'Successfully added 5 ZK devices to the database!';
PRINT 'Device 1: 192.168.1.201:4370 (57 logs)';
PRINT 'Device 2: 192.168.1.202:4370 (30 logs)';
PRINT 'Device 3: 192.168.1.203:4370 (9 logs)';
PRINT 'Device 4: 192.168.1.204:4370 (60 logs)';
PRINT 'Device 5: 192.168.1.220:4370 (49 logs)';
