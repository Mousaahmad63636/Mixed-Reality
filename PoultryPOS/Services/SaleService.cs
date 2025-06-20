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

        public int AddWithItems(Sale sale, List<SaleItem> saleItems)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var saleCommand = new SqlCommand(@"
                    INSERT INTO Sales (CustomerId, TruckId, DriverId, GrossWeight, NumberOfCages, 
                                      CageWeight, NetWeight, PricePerKg, TotalAmount, IsPaidNow, SaleDate) 
                    VALUES (@CustomerId, @TruckId, @DriverId, @GrossWeight, @NumberOfCages, 
                            @CageWeight, @NetWeight, @PricePerKg, @TotalAmount, @IsPaidNow, @SaleDate);
                    SELECT SCOPE_IDENTITY();", connection, transaction);

                saleCommand.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
                saleCommand.Parameters.AddWithValue("@TruckId", sale.TruckId ?? (object)DBNull.Value);
                saleCommand.Parameters.AddWithValue("@DriverId", sale.DriverId ?? (object)DBNull.Value);
                saleCommand.Parameters.AddWithValue("@GrossWeight", sale.GrossWeight);
                saleCommand.Parameters.AddWithValue("@NumberOfCages", sale.NumberOfCages);
                saleCommand.Parameters.AddWithValue("@CageWeight", sale.CageWeight);
                saleCommand.Parameters.AddWithValue("@NetWeight", sale.NetWeight);
                saleCommand.Parameters.AddWithValue("@PricePerKg", sale.PricePerKg);
                saleCommand.Parameters.AddWithValue("@TotalAmount", sale.TotalAmount);
                saleCommand.Parameters.AddWithValue("@IsPaidNow", sale.IsPaidNow);
                saleCommand.Parameters.AddWithValue("@SaleDate", sale.SaleDate);

                var saleId = Convert.ToInt32(saleCommand.ExecuteScalar());

                foreach (var item in saleItems)
                {
                    var itemCommand = new SqlCommand(@"
                        INSERT INTO SaleItems (SaleId, GrossWeight, NumberOfCages, SingleCageWeight, 
                                              TotalCageWeight, NetWeight, TotalAmount)
                        VALUES (@SaleId, @GrossWeight, @NumberOfCages, @SingleCageWeight, 
                                @TotalCageWeight, @NetWeight, @TotalAmount)", connection, transaction);

                    itemCommand.Parameters.AddWithValue("@SaleId", saleId);
                    itemCommand.Parameters.AddWithValue("@GrossWeight", item.GrossWeight);
                    itemCommand.Parameters.AddWithValue("@NumberOfCages", item.NumberOfCages);
                    itemCommand.Parameters.AddWithValue("@SingleCageWeight", item.SingleCageWeight);
                    itemCommand.Parameters.AddWithValue("@TotalCageWeight", item.TotalCageWeight);
                    itemCommand.Parameters.AddWithValue("@NetWeight", item.NetWeight);
                    itemCommand.Parameters.AddWithValue("@TotalAmount", item.TotalAmount);

                    itemCommand.ExecuteNonQuery();
                }

                transaction.Commit();
                return saleId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void UpdateWithItems(Sale sale, List<SaleItem> saleItems)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var updateSaleCommand = new SqlCommand(@"
                    UPDATE Sales SET CustomerId = @CustomerId, TruckId = @TruckId, DriverId = @DriverId,
                                    GrossWeight = @GrossWeight, NumberOfCages = @NumberOfCages,
                                    CageWeight = @CageWeight, NetWeight = @NetWeight, PricePerKg = @PricePerKg,
                                    TotalAmount = @TotalAmount, IsPaidNow = @IsPaidNow
                    WHERE Id = @Id", connection, transaction);

                updateSaleCommand.Parameters.AddWithValue("@Id", sale.Id);
                updateSaleCommand.Parameters.AddWithValue("@CustomerId", sale.CustomerId);
                updateSaleCommand.Parameters.AddWithValue("@TruckId", sale.TruckId ?? (object)DBNull.Value);
                updateSaleCommand.Parameters.AddWithValue("@DriverId", sale.DriverId ?? (object)DBNull.Value);
                updateSaleCommand.Parameters.AddWithValue("@GrossWeight", sale.GrossWeight);
                updateSaleCommand.Parameters.AddWithValue("@NumberOfCages", sale.NumberOfCages);
                updateSaleCommand.Parameters.AddWithValue("@CageWeight", sale.CageWeight);
                updateSaleCommand.Parameters.AddWithValue("@NetWeight", sale.NetWeight);
                updateSaleCommand.Parameters.AddWithValue("@PricePerKg", sale.PricePerKg);
                updateSaleCommand.Parameters.AddWithValue("@TotalAmount", sale.TotalAmount);
                updateSaleCommand.Parameters.AddWithValue("@IsPaidNow", sale.IsPaidNow);

                updateSaleCommand.ExecuteNonQuery();

                var deleteSaleItemsCommand = new SqlCommand("DELETE FROM SaleItems WHERE SaleId = @SaleId", connection, transaction);
                deleteSaleItemsCommand.Parameters.AddWithValue("@SaleId", sale.Id);
                deleteSaleItemsCommand.ExecuteNonQuery();

                foreach (var item in saleItems)
                {
                    var itemCommand = new SqlCommand(@"
                        INSERT INTO SaleItems (SaleId, GrossWeight, NumberOfCages, SingleCageWeight, 
                                              TotalCageWeight, NetWeight, TotalAmount)
                        VALUES (@SaleId, @GrossWeight, @NumberOfCages, @SingleCageWeight, 
                                @TotalCageWeight, @NetWeight, @TotalAmount)", connection, transaction);

                    itemCommand.Parameters.AddWithValue("@SaleId", sale.Id);
                    itemCommand.Parameters.AddWithValue("@GrossWeight", item.GrossWeight);
                    itemCommand.Parameters.AddWithValue("@NumberOfCages", item.NumberOfCages);
                    itemCommand.Parameters.AddWithValue("@SingleCageWeight", item.SingleCageWeight);
                    itemCommand.Parameters.AddWithValue("@TotalCageWeight", item.TotalCageWeight);
                    itemCommand.Parameters.AddWithValue("@NetWeight", item.NetWeight);
                    itemCommand.Parameters.AddWithValue("@TotalAmount", item.TotalAmount);

                    itemCommand.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public List<SaleItem> GetSaleItems(int saleId)
        {
            var items = new List<SaleItem>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("SELECT * FROM SaleItems WHERE SaleId = @SaleId", connection);
            command.Parameters.AddWithValue("@SaleId", saleId);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var item = new SaleItem
                {
                    GrossWeight = reader.GetDecimal("GrossWeight"),
                    NumberOfCages = reader.GetInt32("NumberOfCages"),
                    SingleCageWeight = reader.GetDecimal("SingleCageWeight")
                };
                items.Add(item);
            }

            return items;
        }

        public Sale GetSaleById(int saleId)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                SELECT s.*, c.Name as CustomerName, t.Name as TruckName, d.Name as DriverName
                FROM Sales s
                JOIN Customers c ON s.CustomerId = c.Id
                LEFT JOIN Trucks t ON s.TruckId = t.Id
                LEFT JOIN Drivers d ON s.DriverId = d.Id
                WHERE s.Id = @Id", connection);

            command.Parameters.AddWithValue("@Id", saleId);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Sale
                {
                    Id = reader.GetInt32("Id"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    TruckId = reader.IsDBNull("TruckId") ? null : reader.GetInt32("TruckId"),
                    DriverId = reader.IsDBNull("DriverId") ? null : reader.GetInt32("DriverId"),
                    GrossWeight = reader.GetDecimal("GrossWeight"),
                    NumberOfCages = reader.GetInt32("NumberOfCages"),
                    CageWeight = reader.GetDecimal("CageWeight"),
                    NetWeight = reader.GetDecimal("NetWeight"),
                    PricePerKg = reader.GetDecimal("PricePerKg"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    IsPaidNow = reader.GetBoolean("IsPaidNow"),
                    SaleDate = reader.GetDateTime("SaleDate"),
                    CustomerName = reader.GetString("CustomerName"),
                    TruckName = reader.IsDBNull("TruckName") ? null : reader.GetString("TruckName"),
                    DriverName = reader.IsDBNull("DriverName") ? null : reader.GetString("DriverName")
                };
            }

            return null;
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
            command.Parameters.AddWithValue("@TruckId", sale.TruckId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@DriverId", sale.DriverId ?? (object)DBNull.Value);
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
                LEFT JOIN Trucks t ON s.TruckId = t.Id
                LEFT JOIN Drivers d ON s.DriverId = d.Id
                ORDER BY s.SaleDate DESC", connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                sales.Add(new Sale
                {
                    Id = reader.GetInt32("Id"),
                    CustomerId = reader.GetInt32("CustomerId"),
                    TruckId = reader.IsDBNull("TruckId") ? null : reader.GetInt32("TruckId"),
                    DriverId = reader.IsDBNull("DriverId") ? null : reader.GetInt32("DriverId"),
                    GrossWeight = reader.GetDecimal("GrossWeight"),
                    NumberOfCages = reader.GetInt32("NumberOfCages"),
                    CageWeight = reader.GetDecimal("CageWeight"),
                    NetWeight = reader.GetDecimal("NetWeight"),
                    PricePerKg = reader.GetDecimal("PricePerKg"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    IsPaidNow = reader.GetBoolean("IsPaidNow"),
                    SaleDate = reader.GetDateTime("SaleDate"),
                    CustomerName = reader.GetString("CustomerName"),
                    TruckName = reader.IsDBNull("TruckName") ? null : reader.GetString("TruckName"),
                    DriverName = reader.IsDBNull("DriverName") ? null : reader.GetString("DriverName")
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
                LEFT JOIN Trucks t ON s.TruckId = t.Id
                LEFT JOIN Drivers d ON s.DriverId = d.Id
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
                    TruckId = reader.IsDBNull("TruckId") ? null : reader.GetInt32("TruckId"),
                    DriverId = reader.IsDBNull("DriverId") ? null : reader.GetInt32("DriverId"),
                    GrossWeight = reader.GetDecimal("GrossWeight"),
                    NumberOfCages = reader.GetInt32("NumberOfCages"),
                    CageWeight = reader.GetDecimal("CageWeight"),
                    NetWeight = reader.GetDecimal("NetWeight"),
                    PricePerKg = reader.GetDecimal("PricePerKg"),
                    TotalAmount = reader.GetDecimal("TotalAmount"),
                    IsPaidNow = reader.GetBoolean("IsPaidNow"),
                    SaleDate = reader.GetDateTime("SaleDate"),
                    CustomerName = reader.GetString("CustomerName"),
                    TruckName = reader.IsDBNull("TruckName") ? null : reader.GetString("TruckName"),
                    DriverName = reader.IsDBNull("DriverName") ? null : reader.GetString("DriverName")
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