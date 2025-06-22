using Microsoft.Data.SqlClient;
using PoultryPOS.Models;
using System.Text.Json;

namespace PoultryPOS.Services
{
    public class SyncApplicationService
    {
        private readonly DatabaseService _dbService;
        private readonly SyncConfigurationService _configService;

        public SyncApplicationService()
        {
            _dbService = new DatabaseService();
            _configService = new SyncConfigurationService();
        }

        public void ApplyChangesToLocal(SyncChange change)
        {
            try
            {
                System.Windows.MessageBox.Show($"Applying: {change.Operation} on {change.Table} ID {change.RecordId}", "Apply Debug");

                switch (change.Operation.ToUpper())
                {
                    case "INSERT":
                        InsertRecord(change);
                        break;
                    case "UPDATE":
                    case "UPDATE_BALANCE":
                        UpdateRecord(change);
                        break;
                    case "DELETE":
                        DeleteRecord(change);
                        break;
                }

                LogSyncOperation(change, "SUCCESS", null);
                System.Windows.MessageBox.Show($"Successfully applied {change.Operation} on {change.Table} ID {change.RecordId}", "Success");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to apply {change.Operation} on {change.Table} ID {change.RecordId}: {ex.Message}", "Error");
                LogSyncOperation(change, "ERROR", ex.Message);
            }
        }

        private void InsertRecord(SyncChange change)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            // Check if record already exists
            var checkCommand = new SqlCommand($"SELECT COUNT(*) FROM {change.Table} WHERE Id = @Id", connection);
            checkCommand.Parameters.AddWithValue("@Id", change.RecordId);
            var exists = (int)checkCommand.ExecuteScalar() > 0;

            if (exists)
            {
                System.Windows.MessageBox.Show($"Record {change.RecordId} already exists in {change.Table}, skipping", "Info");
                return;
            }

            // Insert new record based on table
            switch (change.Table.ToLower())
            {
                case "customers":
                    InsertCustomer(change, connection);
                    System.Windows.MessageBox.Show($"Customer {change.RecordId} inserted successfully", "Success");
                    break;
                case "sales":
                    InsertSale(change, connection);
                    System.Windows.MessageBox.Show($"Sale {change.RecordId} inserted successfully", "Success");
                    break;
                case "payments":
                    InsertPayment(change, connection);
                    System.Windows.MessageBox.Show($"Payment {change.RecordId} inserted successfully", "Success");
                    break;
                case "saleitem":
                case "saleitems":
                    // Skip SaleItems for now, they're handled differently
                    System.Windows.MessageBox.Show($"SaleItems sync not yet implemented", "Info");
                    break;
                default:
                    System.Windows.MessageBox.Show($"Insert not implemented for table: {change.Table}", "Warning");
                    break;
            }
        }

        private void UpdateRecord(SyncChange change)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            switch (change.Table.ToLower())
            {
                case "customers":
                    UpdateCustomer(change, connection);
                    break;
                default:
                    System.Windows.MessageBox.Show($"Update not implemented for table: {change.Table}", "Warning");
                    break;
            }
        }

        private void DeleteRecord(SyncChange change)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand($"UPDATE {change.Table} SET IsActive = 0 WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", change.RecordId);
            command.ExecuteNonQuery();
        }

        private void InsertCustomer(SyncChange change, SqlConnection connection)
        {
            var command = new SqlCommand(@"
                SET IDENTITY_INSERT Customers ON;
                INSERT INTO Customers (Id, Name, Phone, Balance, IsActive, SyncId, LastModified, SyncStatus, DeviceId, Version, IsDeleted) 
                VALUES (@Id, @Name, @Phone, @Balance, @IsActive, @SyncId, @LastModified, 'Synced', @DeviceId, @Version, 0);
                SET IDENTITY_INSERT Customers OFF;", connection);

            command.Parameters.AddWithValue("@Id", change.RecordId);
            command.Parameters.AddWithValue("@Name", GetStringValue(change.Data, "Name"));
            command.Parameters.AddWithValue("@Phone", GetStringValueOrNull(change.Data, "Phone"));
            command.Parameters.AddWithValue("@Balance", GetDecimalValue(change.Data, "Balance"));
            command.Parameters.AddWithValue("@IsActive", GetBoolValue(change.Data, "IsActive"));
            command.Parameters.AddWithValue("@SyncId", change.SyncId);
            command.Parameters.AddWithValue("@LastModified", change.LastModified);
            command.Parameters.AddWithValue("@DeviceId", "Remote");
            command.Parameters.AddWithValue("@Version", change.Version);

            command.ExecuteNonQuery();
        }

        private void UpdateCustomer(SyncChange change, SqlConnection connection)
        {
            if (change.Operation == "UPDATE_BALANCE")
            {
                var command = new SqlCommand(@"
                    UPDATE Customers 
                    SET Balance = @Balance, LastModified = @LastModified, SyncStatus = 'Synced', Version = @Version
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", change.RecordId);
                command.Parameters.AddWithValue("@Balance", GetDecimalValue(change.Data, "Balance"));
                command.Parameters.AddWithValue("@LastModified", change.LastModified);
                command.Parameters.AddWithValue("@Version", change.Version);
                command.ExecuteNonQuery();
            }
            else
            {
                var command = new SqlCommand(@"
                    UPDATE Customers 
                    SET Name = @Name, Phone = @Phone, Balance = @Balance, IsActive = @IsActive, 
                        LastModified = @LastModified, SyncStatus = 'Synced', Version = @Version
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", change.RecordId);
                command.Parameters.AddWithValue("@Name", GetStringValue(change.Data, "Name"));
                command.Parameters.AddWithValue("@Phone", GetStringValueOrNull(change.Data, "Phone"));
                command.Parameters.AddWithValue("@Balance", GetDecimalValue(change.Data, "Balance"));
                command.Parameters.AddWithValue("@IsActive", GetBoolValue(change.Data, "IsActive"));
                command.Parameters.AddWithValue("@LastModified", change.LastModified);
                command.Parameters.AddWithValue("@Version", change.Version);
                command.ExecuteNonQuery();
            }
        }

        private void InsertSale(SyncChange change, SqlConnection connection)
        {
            try
            {
                System.Windows.MessageBox.Show($"Inserting Sale ID: {change.RecordId}", "Sale Debug");

                // First check if the customer exists
                var customerCheck = new SqlCommand("SELECT COUNT(*) FROM Customers WHERE Id = @CustomerId", connection);
                customerCheck.Parameters.AddWithValue("@CustomerId", GetIntValue(change.Data, "CustomerId"));
                var customerExists = (int)customerCheck.ExecuteScalar() > 0;

                if (!customerExists)
                {
                    System.Windows.MessageBox.Show($"Customer ID {GetIntValue(change.Data, "CustomerId")} does not exist, cannot insert sale", "Error");
                    return;
                }

                var command = new SqlCommand(@"
            SET IDENTITY_INSERT Sales ON;
            INSERT INTO Sales (Id, CustomerId, TruckId, DriverId, GrossWeight, NumberOfCages, CageWeight, 
                              NetWeight, PricePerKg, TotalAmount, IsPaidNow, SaleDate, 
                              SyncId, LastModified, SyncStatus, DeviceId, Version, IsDeleted) 
            VALUES (@Id, @CustomerId, @TruckId, @DriverId, @GrossWeight, @NumberOfCages, @CageWeight,
                    @NetWeight, @PricePerKg, @TotalAmount, @IsPaidNow, @SaleDate,
                    @SyncId, @LastModified, 'Synced', @DeviceId, @Version, 0);
            SET IDENTITY_INSERT Sales OFF;", connection);

                command.Parameters.AddWithValue("@Id", change.RecordId);
                command.Parameters.AddWithValue("@CustomerId", GetIntValue(change.Data, "CustomerId"));
                command.Parameters.AddWithValue("@TruckId", GetIntValueOrNull(change.Data, "TruckId"));
                command.Parameters.AddWithValue("@DriverId", GetIntValueOrNull(change.Data, "DriverId"));
                command.Parameters.AddWithValue("@GrossWeight", GetDecimalValue(change.Data, "GrossWeight"));
                command.Parameters.AddWithValue("@NumberOfCages", GetIntValue(change.Data, "NumberOfCages"));
                command.Parameters.AddWithValue("@CageWeight", GetDecimalValue(change.Data, "CageWeight"));
                command.Parameters.AddWithValue("@NetWeight", GetDecimalValue(change.Data, "NetWeight"));
                command.Parameters.AddWithValue("@PricePerKg", GetDecimalValue(change.Data, "PricePerKg"));
                command.Parameters.AddWithValue("@TotalAmount", GetDecimalValue(change.Data, "TotalAmount"));
                command.Parameters.AddWithValue("@IsPaidNow", GetBoolValue(change.Data, "IsPaidNow"));
                command.Parameters.AddWithValue("@SaleDate", GetDateValue(change.Data, "SaleDate"));
                command.Parameters.AddWithValue("@SyncId", change.SyncId);
                command.Parameters.AddWithValue("@LastModified", change.LastModified);
                command.Parameters.AddWithValue("@DeviceId", "Remote");
                command.Parameters.AddWithValue("@Version", change.Version);

                var rowsAffected = command.ExecuteNonQuery();
                System.Windows.MessageBox.Show($"Sale insert completed. Rows affected: {rowsAffected}", "Sale Debug");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error inserting sale: {ex.Message}", "Sale Error");
                throw;
            }
        }
        private void InsertPayment(SyncChange change, SqlConnection connection)
        {
            var command = new SqlCommand(@"
                SET IDENTITY_INSERT Payments ON;
                INSERT INTO Payments (Id, CustomerId, Amount, PaymentDate, Notes,
                                     SyncId, LastModified, SyncStatus, DeviceId, Version, IsDeleted) 
                VALUES (@Id, @CustomerId, @Amount, @PaymentDate, @Notes,
                        @SyncId, @LastModified, 'Synced', @DeviceId, @Version, 0);
                SET IDENTITY_INSERT Payments OFF;", connection);

            command.Parameters.AddWithValue("@Id", change.RecordId);
            command.Parameters.AddWithValue("@CustomerId", GetIntValue(change.Data, "CustomerId"));
            command.Parameters.AddWithValue("@Amount", GetDecimalValue(change.Data, "Amount"));
            command.Parameters.AddWithValue("@PaymentDate", GetDateValue(change.Data, "PaymentDate"));
            command.Parameters.AddWithValue("@Notes", GetStringValueOrNull(change.Data, "Notes"));
            command.Parameters.AddWithValue("@SyncId", change.SyncId);
            command.Parameters.AddWithValue("@LastModified", change.LastModified);
            command.Parameters.AddWithValue("@DeviceId", "Remote");
            command.Parameters.AddWithValue("@Version", change.Version);

            command.ExecuteNonQuery();
        }

        // Helper methods for safe data conversion
        private string GetStringValue(Dictionary<string, object?> data, string key)
        {
            if (data.ContainsKey(key) && data[key] != null)
            {
                if (data[key] is JsonElement element)
                    return element.GetString() ?? "";
                return data[key]?.ToString() ?? "";
            }
            return "";
        }

        private object GetStringValueOrNull(Dictionary<string, object?> data, string key)
        {
            if (data.ContainsKey(key) && data[key] != null)
            {
                if (data[key] is JsonElement element)
                {
                    var value = element.GetString();
                    return string.IsNullOrEmpty(value) ? DBNull.Value : value;
                }
                var strValue = data[key]?.ToString();
                return string.IsNullOrEmpty(strValue) ? DBNull.Value : strValue;
            }
            return DBNull.Value;
        }

        private decimal GetDecimalValue(Dictionary<string, object?> data, string key)
        {
            if (data.ContainsKey(key) && data[key] != null)
            {
                if (data[key] is JsonElement element)
                    return element.GetDecimal();
                if (decimal.TryParse(data[key]?.ToString(), out decimal result))
                    return result;
            }
            return 0;
        }

        private int GetIntValue(Dictionary<string, object?> data, string key)
        {
            if (data.ContainsKey(key) && data[key] != null)
            {
                if (data[key] is JsonElement element)
                    return element.GetInt32();
                if (int.TryParse(data[key]?.ToString(), out int result))
                    return result;
            }
            return 0;
        }

        private object GetIntValueOrNull(Dictionary<string, object?> data, string key)
        {
            if (data.ContainsKey(key) && data[key] != null)
            {
                if (data[key] is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Null)
                        return DBNull.Value;
                    return element.GetInt32();
                }
                if (int.TryParse(data[key]?.ToString(), out int result))
                    return result;
            }
            return DBNull.Value;
        }

        private bool GetBoolValue(Dictionary<string, object?> data, string key)
        {
            if (data.ContainsKey(key) && data[key] != null)
            {
                if (data[key] is JsonElement element)
                    return element.GetBoolean();
                if (bool.TryParse(data[key]?.ToString(), out bool result))
                    return result;
            }
            return true;
        }

        private DateTime GetDateValue(Dictionary<string, object?> data, string key)
        {
            if (data.ContainsKey(key) && data[key] != null)
            {
                if (data[key] is JsonElement element)
                    return element.GetDateTime();
                if (DateTime.TryParse(data[key]?.ToString(), out DateTime result))
                    return result;
            }
            return DateTime.Now;
        }

        private void LogSyncOperation(SyncChange change, string status, string? errorMessage)
        {
            try
            {
                using var connection = _dbService.GetConnection();
                connection.Open();

                var command = new SqlCommand(@"
                    INSERT INTO SyncLog (SyncDate, Operation, TableName, RecordId, Status, ErrorMessage, DeviceId) 
                    VALUES (@SyncDate, @Operation, @TableName, @RecordId, @Status, @ErrorMessage, @DeviceId)", connection);

                command.Parameters.AddWithValue("@SyncDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@Operation", $"APPLY_{change.Operation}");
                command.Parameters.AddWithValue("@TableName", change.Table);
                command.Parameters.AddWithValue("@RecordId", change.RecordId);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@ErrorMessage", errorMessage ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DeviceId", _configService.GetDeviceId());

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to log sync operation: {ex.Message}", "Log Error");
            }
        }
    }
}