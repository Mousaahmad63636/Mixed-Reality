using Microsoft.Data.SqlClient;
using PoultryPOS.Models;
using System.Data;

namespace PoultryPOS.Services
{
    public class SaleService
    {
        private readonly DatabaseService _dbService;

        public SaleService()
        {
            _dbService = new DatabaseService();
        }

        public void Add(Sale sale)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                INSERT INTO Sales (CustomerId, TruckId, DriverId, GrossWeight, NumberOfCages, 
                                  CageWeight, NetWeight, PricePerKg, TotalAmount, IsPaidNow, SaleDate) 
                VALUES (@CustomerId, @TruckId, @DriverId, @GrossWeight, @NumberOfCages, 
                        @CageWeight, @NetWeight, @PricePerKg, @TotalAmount, @IsPaidNow, @SaleDate)", connection);

            command.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
            command.Parameters.AddWithValue("@TruckId", sale.TruckId);
            command.Parameters.AddWithValue("@DriverId", sale.DriverId);
            command.Parameters.AddWithValue("@GrossWeight", sale.GrossWeight);
            command.Parameters.AddWithValue("@NumberOfCages", sale.NumberOfCages);
            command.Parameters.AddWithValue("@CageWeight", sale.CageWeight);
            command.Parameters.AddWithValue("@NetWeight", sale.NetWeight);
            command.Parameters.AddWithValue("@PricePerKg", sale.PricePerKg);
            command.Parameters.AddWithValue("@TotalAmount", sale.TotalAmount);
            command.Parameters.AddWithValue("@IsPaidNow", sale.IsPaidNow);
            command.Parameters.AddWithValue("@SaleDate", sale.SaleDate);

            command.ExecuteNonQuery();
        }

        public List<Sale> GetAll()
        {
            var sales = new List<Sale>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT s.*, c.Name as CustomerName, t.Name as TruckName, d.Name as DriverName
                FROM Sales s
                JOIN Customers c ON s.CustomerId = c.Id
                JOIN Trucks t ON s.TruckId = t.Id
                JOIN Drivers d ON s.DriverId = d.Id
                ORDER BY s.SaleDate DESC", connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                sales.Add(new Sale
                {
                    Id = reader.GetInt32("Id"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    TruckId = reader.GetInt32("TruckId"),
                    DriverId = reader.GetInt32("DriverId"),
                    GrossWeight = reader.GetDecimal("GrossWeight"),
                    NumberOfCages = reader.GetInt32("NumberOfCages"),
                    CageWeight = reader.GetDecimal("CageWeight"),
                    NetWeight = reader.GetDecimal("NetWeight"),
                    PricePerKg = reader.GetDecimal("PricePerKg"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    IsPaidNow = reader.GetBoolean("IsPaidNow"),
                    SaleDate = reader.GetDateTime("SaleDate"),
                    CustomerName = reader.GetString("CustomerName"),
                    TruckName = reader.GetString("TruckName"),
                    DriverName = reader.GetString("DriverName")
                });
            }

            return sales;
        }

        public List<Sale> GetByDateRange(DateTime fromDate, DateTime toDate)
        {
            var sales = new List<Sale>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT s.*, c.Name as CustomerName, t.Name as TruckName, d.Name as DriverName
                FROM Sales s
                JOIN Customers c ON s.CustomerId = c.Id
                JOIN Trucks t ON s.TruckId = t.Id
                JOIN Drivers d ON s.DriverId = d.Id
                WHERE s.SaleDate >= @FromDate AND s.SaleDate <= @ToDate
                ORDER BY s.SaleDate DESC", connection);

            command.Parameters.AddWithValue("@FromDate", fromDate.Date);
            command.Parameters.AddWithValue("@ToDate", toDate.Date.AddDays(1).AddSeconds(-1));

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                sales.Add(new Sale
                {
                    Id = reader.GetInt32("Id"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    TruckId = reader.GetInt32("TruckId"),
                    DriverId = reader.GetInt32("DriverId"),
                    GrossWeight = reader.GetDecimal("GrossWeight"),
                    NumberOfCages = reader.GetInt32("NumberOfCages"),
                    CageWeight = reader.GetDecimal("CageWeight"),
                    NetWeight = reader.GetDecimal("NetWeight"),
                    PricePerKg = reader.GetDecimal("PricePerKg"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    IsPaidNow = reader.GetBoolean("IsPaidNow"),
                    SaleDate = reader.GetDateTime("SaleDate"),
                    CustomerName = reader.GetString("CustomerName"),
                    TruckName = reader.GetString("TruckName"),
                    DriverName = reader.GetString("DriverName")
                });
            }

            return sales;
        }

        public decimal GetTotalSales()
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("SELECT ISNULL(SUM(TotalAmount), 0) FROM Sales", connection);
            return (decimal)command.ExecuteScalar();
        }

        public decimal GetTotalSalesByDateRange(DateTime fromDate, DateTime toDate)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("SELECT ISNULL(SUM(TotalAmount), 0) FROM Sales WHERE SaleDate >= @FromDate AND SaleDate <= @ToDate", connection);
            command.Parameters.AddWithValue("@FromDate", fromDate.Date);
            command.Parameters.AddWithValue("@ToDate", toDate.Date.AddDays(1).AddSeconds(-1));

            return (decimal)command.ExecuteScalar();
        }
    }
}