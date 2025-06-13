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
                    TruckId INT FOREIGN KEY REFERENCES Trucks(Id),
                    DriverId INT FOREIGN KEY REFERENCES Drivers(Id),
                    GrossWeight DECIMAL(10,2) NOT NULL,
                    NumberOfCages INT NOT NULL,
                    CageWeight DECIMAL(10,2) NOT NULL,
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
    }
}