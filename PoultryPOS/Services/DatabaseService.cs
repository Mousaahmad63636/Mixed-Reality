using Microsoft.Data.SqlClient;
using System.Data;

namespace PoultryPOS.Services
{
    public class DatabaseService
    {
        private readonly string _masterConnectionString;
        private readonly string _connectionString;

        public DatabaseService()
        {
            _masterConnectionString = "Server=.\\posserver;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
            _connectionString = "Server=.\\posserver;Database=PoultryPOS;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public void InitializeDatabase()
        {
            CreateDatabaseIfNotExists();
            CreateTablesIfNotExist();
            AddNetWeightColumnIfNotExists();
            CreateSaleItemsTableIfNotExists();
            CreateTruckLoadingSessionsTableIfNotExists();
            MakeTruckDriverOptional();
            AddSyncColumnsIfNotExists();
            CreateSyncControlTablesIfNotExists();
            ExpandSyncLogStatusColumn();
        }

        private void CreateDatabaseIfNotExists()
        {
            using var connection = new SqlConnection(_masterConnectionString);
            connection.Open();

            var checkDbCommand = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name = 'PoultryPOS'", connection);
            var dbExists = (int)checkDbCommand.ExecuteScalar() > 0;

            if (!dbExists)
            {
                var createDbCommand = new SqlCommand("CREATE DATABASE PoultryPOS", connection);
                createDbCommand.ExecuteNonQuery();
            }
        }
        private void ExpandSyncLogStatusColumn()
        {
            using var connection = GetConnection();
            connection.Open();

            var expandStatusScript = @"
        -- Expand Status column to handle longer messages
        IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_NAME = 'SyncLog' AND COLUMN_NAME = 'Status' AND CHARACTER_MAXIMUM_LENGTH = 20)
        BEGIN
            ALTER TABLE SyncLog ALTER COLUMN Status NVARCHAR(50)
        END";

            using var command = new SqlCommand(expandStatusScript, connection);
            command.ExecuteNonQuery();
        }
        private void CreateTablesIfNotExist()
        {
            using var connection = GetConnection();
            connection.Open();

            var createTablesScript = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Trucks' AND xtype='U')
                CREATE TABLE Trucks (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    CurrentLoad INT DEFAULT 0,
                    NetWeight DECIMAL(10,2) DEFAULT 0,
                    PlateNumber NVARCHAR(50),
                    IsActive BIT DEFAULT 1
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Drivers' AND xtype='U')
                CREATE TABLE Drivers (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Phone NVARCHAR(20),
                    IsActive BIT DEFAULT 1
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customers' AND xtype='U')
                CREATE TABLE Customers (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Phone NVARCHAR(20),
                    Balance DECIMAL(10,2) DEFAULT 0,
                    IsActive BIT DEFAULT 1
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Sales' AND xtype='U')
                CREATE TABLE Sales (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    CustomerId INT FOREIGN KEY REFERENCES Customers(Id),
                    TruckId INT NULL,
                    DriverId INT NULL,
                    GrossWeight DECIMAL(10,2) NOT NULL,
                    NumberOfCages INT DEFAULT 0,
                    CageWeight DECIMAL(10,2) DEFAULT 0,
                    NetWeight DECIMAL(10,2) NOT NULL,
                    PricePerKg DECIMAL(10,2) NOT NULL,
                    TotalAmount DECIMAL(10,2) NOT NULL,
                    IsPaidNow BIT NOT NULL,
                    SaleDate DATETIME DEFAULT GETDATE()
                );

                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Payments' AND xtype='U')
                CREATE TABLE Payments (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    CustomerId INT FOREIGN KEY REFERENCES Customers(Id),
                    Amount DECIMAL(10,2) NOT NULL,
                    PaymentDate DATETIME DEFAULT GETDATE(),
                    Notes NVARCHAR(200)
                );";

            using var command = new SqlCommand(createTablesScript, connection);
            command.ExecuteNonQuery();
        }

        private void AddNetWeightColumnIfNotExists()
        {
            using var connection = GetConnection();
            connection.Open();

            var checkColumnScript = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                              WHERE TABLE_NAME = 'Trucks' AND COLUMN_NAME = 'NetWeight')
                BEGIN
                    ALTER TABLE Trucks ADD NetWeight DECIMAL(10,2) DEFAULT 0
                END";

            using var command = new SqlCommand(checkColumnScript, connection);
            command.ExecuteNonQuery();
        }

        private void AddSyncColumnsIfNotExists()
        {
            using var connection = GetConnection();
            connection.Open();

            var syncColumnsScript = @"
        -- Add sync columns to Customers table
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Customers' AND COLUMN_NAME = 'SyncId')
        BEGIN
            ALTER TABLE Customers ADD SyncId UNIQUEIDENTIFIER DEFAULT NEWID()
            ALTER TABLE Customers ADD LastModified DATETIME2 DEFAULT GETDATE()
            ALTER TABLE Customers ADD IsDeleted BIT DEFAULT 0
            ALTER TABLE Customers ADD SyncStatus NVARCHAR(20) DEFAULT 'Pending'
            ALTER TABLE Customers ADD DeviceId NVARCHAR(50) DEFAULT 'PC1'
            ALTER TABLE Customers ADD Version INT DEFAULT 1
        END

        -- Add sync columns to Sales table
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Sales' AND COLUMN_NAME = 'SyncId')
        BEGIN
            ALTER TABLE Sales ADD SyncId UNIQUEIDENTIFIER DEFAULT NEWID()
            ALTER TABLE Sales ADD LastModified DATETIME2 DEFAULT GETDATE()
            ALTER TABLE Sales ADD IsDeleted BIT DEFAULT 0
            ALTER TABLE Sales ADD SyncStatus NVARCHAR(20) DEFAULT 'Pending'
            ALTER TABLE Sales ADD DeviceId NVARCHAR(50) DEFAULT 'PC1'
            ALTER TABLE Sales ADD Version INT DEFAULT 1
        END

        -- Add sync columns to Payments table
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Payments' AND COLUMN_NAME = 'SyncId')
        BEGIN
            ALTER TABLE Payments ADD SyncId UNIQUEIDENTIFIER DEFAULT NEWID()
            ALTER TABLE Payments ADD LastModified DATETIME2 DEFAULT GETDATE()
            ALTER TABLE Payments ADD IsDeleted BIT DEFAULT 0
            ALTER TABLE Payments ADD SyncStatus NVARCHAR(20) DEFAULT 'Pending'
            ALTER TABLE Payments ADD DeviceId NVARCHAR(50) DEFAULT 'PC1'
            ALTER TABLE Payments ADD Version INT DEFAULT 1
        END

        -- Add sync columns to Trucks table
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Trucks' AND COLUMN_NAME = 'SyncId')
        BEGIN
            ALTER TABLE Trucks ADD SyncId UNIQUEIDENTIFIER DEFAULT NEWID()
            ALTER TABLE Trucks ADD LastModified DATETIME2 DEFAULT GETDATE()
            ALTER TABLE Trucks ADD IsDeleted BIT DEFAULT 0
            ALTER TABLE Trucks ADD SyncStatus NVARCHAR(20) DEFAULT 'Pending'
            ALTER TABLE Trucks ADD DeviceId NVARCHAR(50) DEFAULT 'PC1'
            ALTER TABLE Trucks ADD Version INT DEFAULT 1
        END

        -- Add sync columns to Drivers table
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Drivers' AND COLUMN_NAME = 'SyncId')
        BEGIN
            ALTER TABLE Drivers ADD SyncId UNIQUEIDENTIFIER DEFAULT NEWID()
            ALTER TABLE Drivers ADD LastModified DATETIME2 DEFAULT GETDATE()
            ALTER TABLE Drivers ADD IsDeleted BIT DEFAULT 0
            ALTER TABLE Drivers ADD SyncStatus NVARCHAR(20) DEFAULT 'Pending'
            ALTER TABLE Drivers ADD DeviceId NVARCHAR(50) DEFAULT 'PC1'
            ALTER TABLE Drivers ADD Version INT DEFAULT 1
        END

        -- Add sync columns to SaleItems table
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SaleItems' AND COLUMN_NAME = 'SyncId')
        BEGIN
            ALTER TABLE SaleItems ADD SyncId UNIQUEIDENTIFIER DEFAULT NEWID()
            ALTER TABLE SaleItems ADD LastModified DATETIME2 DEFAULT GETDATE()
            ALTER TABLE SaleItems ADD IsDeleted BIT DEFAULT 0
            ALTER TABLE SaleItems ADD SyncStatus NVARCHAR(20) DEFAULT 'Pending'
            ALTER TABLE SaleItems ADD DeviceId NVARCHAR(50) DEFAULT 'PC1'
            ALTER TABLE SaleItems ADD Version INT DEFAULT 1
        END

        -- Add sync columns to TruckLoadingSessions table
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TruckLoadingSessions' AND COLUMN_NAME = 'SyncId')
        BEGIN
            ALTER TABLE TruckLoadingSessions ADD SyncId UNIQUEIDENTIFIER DEFAULT NEWID()
            ALTER TABLE TruckLoadingSessions ADD LastModified DATETIME2 DEFAULT GETDATE()
            ALTER TABLE TruckLoadingSessions ADD IsDeleted BIT DEFAULT 0
            ALTER TABLE TruckLoadingSessions ADD SyncStatus NVARCHAR(20) DEFAULT 'Pending'
            ALTER TABLE TruckLoadingSessions ADD DeviceId NVARCHAR(50) DEFAULT 'PC1'
            ALTER TABLE TruckLoadingSessions ADD Version INT DEFAULT 1
        END";

            using var command = new SqlCommand(syncColumnsScript, connection);
            command.ExecuteNonQuery();
        }
        private void CreateSaleItemsTableIfNotExists()
        {
            using var connection = GetConnection();
            connection.Open();

            var createSaleItemsScript = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SaleItems' AND xtype='U')
                CREATE TABLE SaleItems (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    SaleId INT FOREIGN KEY REFERENCES Sales(Id),
                    GrossWeight DECIMAL(10,2) NOT NULL,
                    NumberOfCages INT DEFAULT 0,
                    SingleCageWeight DECIMAL(10,2) DEFAULT 0,
                    TotalCageWeight DECIMAL(10,2) DEFAULT 0,
                    NetWeight DECIMAL(10,2) NOT NULL,
                    TotalAmount DECIMAL(10,2) NOT NULL
                );";

            using var command = new SqlCommand(createSaleItemsScript, connection);
            command.ExecuteNonQuery();
        }
        private void CreateSyncControlTablesIfNotExists()
        {
            using var connection = GetConnection();
            connection.Open();

            var syncTablesScript = @"
        -- Create SyncConfiguration table
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SyncConfiguration' AND xtype='U')
        CREATE TABLE SyncConfiguration (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            DeviceId NVARCHAR(50) NOT NULL UNIQUE,
            LastSyncDate DATETIME2 DEFAULT '1900-01-01',
            CloudFolderPath NVARCHAR(500),
            IsEnabled BIT DEFAULT 1,
            CreatedDate DATETIME2 DEFAULT GETDATE()
        );

        -- Create SyncLog table
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SyncLog' AND xtype='U')
        CREATE TABLE SyncLog (
            LogId INT IDENTITY(1,1) PRIMARY KEY,
            SyncDate DATETIME2 DEFAULT GETDATE(),
            Operation NVARCHAR(50),
            TableName NVARCHAR(100),
            RecordId INT,
            Status NVARCHAR(20),
            ErrorMessage NVARCHAR(500),
            DeviceId NVARCHAR(50)
        );

        -- Create ConflictLog table
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ConflictLog' AND xtype='U')
        CREATE TABLE ConflictLog (
            ConflictId INT IDENTITY(1,1) PRIMARY KEY,
            TableName NVARCHAR(100) NOT NULL,
            RecordId INT NOT NULL,
            LocalData NVARCHAR(MAX),
            RemoteData NVARCHAR(MAX),
            ResolutionAction NVARCHAR(100),
            ResolvedDate DATETIME2 DEFAULT GETDATE(),
            DeviceId NVARCHAR(50)
        );

        -- Insert default configuration if not exists
        IF NOT EXISTS (SELECT * FROM SyncConfiguration WHERE DeviceId = 'PC1')
        INSERT INTO SyncConfiguration (DeviceId, CloudFolderPath, IsEnabled) 
        VALUES ('PC1', '', 1);";

            using var command = new SqlCommand(syncTablesScript, connection);
            command.ExecuteNonQuery();
        }
        private void CreateTruckLoadingSessionsTableIfNotExists()
        {
            using var connection = GetConnection();
            connection.Open();

            var createTruckLoadingSessionsScript = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TruckLoadingSessions' AND xtype='U')
                CREATE TABLE TruckLoadingSessions (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    TruckId INT FOREIGN KEY REFERENCES Trucks(Id),
                    LoadDate DATETIME NOT NULL,
                    InitialCages INT NOT NULL,
                    InitialWeight DECIMAL(10,2) NOT NULL,
                    CompletionDate DATETIME NULL,
                    FinalWeight DECIMAL(10,2) NULL,
                    WeightVariance DECIMAL(10,2) NULL,
                    IsCompleted BIT DEFAULT 0
                );";

            using var command = new SqlCommand(createTruckLoadingSessionsScript, connection);
            command.ExecuteNonQuery();
        }

        private void MakeTruckDriverOptional()
        {
            using var connection = GetConnection();
            connection.Open();

            var alterScript = @"
                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                          WHERE CONSTRAINT_TYPE = 'FOREIGN KEY' 
                          AND TABLE_NAME = 'Sales' 
                          AND CONSTRAINT_NAME LIKE '%TruckId%')
                BEGIN
                    DECLARE @ConstraintName NVARCHAR(255)
                    SELECT @ConstraintName = CONSTRAINT_NAME 
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_TYPE = 'FOREIGN KEY' 
                    AND TABLE_NAME = 'Sales' 
                    AND CONSTRAINT_NAME LIKE '%TruckId%'
                    
                    EXEC('ALTER TABLE Sales DROP CONSTRAINT ' + @ConstraintName)
                    
                    ALTER TABLE Sales ALTER COLUMN TruckId INT NULL
                END

                IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                          WHERE CONSTRAINT_TYPE = 'FOREIGN KEY' 
                          AND TABLE_NAME = 'Sales' 
                          AND CONSTRAINT_NAME LIKE '%DriverId%')
                BEGIN
                    DECLARE @ConstraintName2 NVARCHAR(255)
                    SELECT @ConstraintName2 = CONSTRAINT_NAME 
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_TYPE = 'FOREIGN KEY' 
                    AND TABLE_NAME = 'Sales' 
                    AND CONSTRAINT_NAME LIKE '%DriverId%'
                    
                    EXEC('ALTER TABLE Sales DROP CONSTRAINT ' + @ConstraintName2)
                    
                    ALTER TABLE Sales ALTER COLUMN DriverId INT NULL
                END

                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                              WHERE CONSTRAINT_TYPE = 'FOREIGN KEY' 
                              AND TABLE_NAME = 'Sales' 
                              AND CONSTRAINT_NAME = 'FK_Sales_Trucks')
                BEGIN
                    ALTER TABLE Sales ADD CONSTRAINT FK_Sales_Trucks FOREIGN KEY (TruckId) REFERENCES Trucks(Id)
                END

                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                              WHERE CONSTRAINT_TYPE = 'FOREIGN KEY' 
                              AND TABLE_NAME = 'Sales' 
                              AND CONSTRAINT_NAME = 'FK_Sales_Drivers')
                BEGIN
                    ALTER TABLE Sales ADD CONSTRAINT FK_Sales_Drivers FOREIGN KEY (DriverId) REFERENCES Drivers(Id)
                END";

            using var command = new SqlCommand(alterScript, connection);
            command.ExecuteNonQuery();
        }
    }
}