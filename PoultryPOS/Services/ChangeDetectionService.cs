using Microsoft.Data.SqlClient;
using PoultryPOS.Models;
using System.Text.Json;
using System.IO;

namespace PoultryPOS.Services
{
    public class ChangeDetectionService
    {
        private readonly DatabaseService _dbService;
        private readonly SyncConfigurationService _configService;
        private readonly FileOperationsService _fileService;

        public ChangeDetectionService()
        {
            _dbService = new DatabaseService();
            _configService = new SyncConfigurationService();
            _fileService = new FileOperationsService();
        }

        public void RecordChange(string tableName, int recordId, string operation, Dictionary<string, object?> data)
        {
            try
            {
                var deviceId = _configService.GetDeviceId();
                var syncId = Guid.NewGuid().ToString();

                UpdateSyncMetadata(tableName, recordId, syncId, deviceId);

                var change = new SyncChange
                {
                    Table = tableName,
                    Operation = operation,
                    RecordId = recordId,
                    SyncId = syncId,
                    LastModified = DateTime.UtcNow,
                    Version = 1,
                    Data = data
                };

                _fileService.AddChangeToTodaysFile(change);
                LogChange(tableName, recordId, operation, "SUCCESS", null);
            }
            catch (Exception ex)
            {
                LogChange(tableName, recordId, operation, "ERROR", ex.Message);
            }
        }

        private void UpdateSyncMetadata(string tableName, int recordId, string syncId, string deviceId)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand($@"
                UPDATE {tableName} 
                SET SyncId = @SyncId, LastModified = @LastModified, SyncStatus = 'Pending', DeviceId = @DeviceId, Version = Version + 1
                WHERE Id = @RecordId", connection);

            command.Parameters.AddWithValue("@SyncId", syncId);
            command.Parameters.AddWithValue("@LastModified", DateTime.UtcNow);
            command.Parameters.AddWithValue("@DeviceId", deviceId);
            command.Parameters.AddWithValue("@RecordId", recordId);

            command.ExecuteNonQuery();
        }

        private void LogChange(string tableName, int recordId, string operation, string status, string? errorMessage)
        {
            try
            {
                using var connection = _dbService.GetConnection();
                connection.Open();

                var command = new SqlCommand(@"
                    INSERT INTO SyncLog (SyncDate, Operation, TableName, RecordId, Status, ErrorMessage, DeviceId) 
                    VALUES (@SyncDate, @Operation, @TableName, @RecordId, @Status, @ErrorMessage, @DeviceId)", connection);

                command.Parameters.AddWithValue("@SyncDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@Operation", operation);
                command.Parameters.AddWithValue("@TableName", tableName);
                command.Parameters.AddWithValue("@RecordId", recordId);
                command.Parameters.AddWithValue("@Status", status);
                command.Parameters.AddWithValue("@ErrorMessage", errorMessage ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DeviceId", _configService.GetDeviceId());

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PoultryPOS", "sync_errors.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.AppendAllText(logPath, $"{DateTime.Now}: Failed to log to database: {ex.Message}\n");
            }
        }
    }
}