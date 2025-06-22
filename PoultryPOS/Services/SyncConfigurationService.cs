using Microsoft.Data.SqlClient;
using PoultryPOS.Models;
using System.Data;
using System.IO;

namespace PoultryPOS.Services
{
    public class SyncConfigurationService
    {
        private readonly DatabaseService _dbService;
        private readonly string _deviceId = "PC1"; // Change this to "PC2" on second computer
        private readonly string _fixedCloudPath = @"G:\My Drive\PoultryPOS_Sync";

        public SyncConfigurationService()
        {
            _dbService = new DatabaseService();
        }

        public SyncConfiguration GetConfiguration()
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand("SELECT * FROM SyncConfiguration WHERE DeviceId = @DeviceId", connection);
            command.Parameters.AddWithValue("@DeviceId", _deviceId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var cloudPath = reader.IsDBNull("CloudFolderPath") ? null : reader.GetString("CloudFolderPath");

                // If path is empty or null, use the fixed path
                if (string.IsNullOrEmpty(cloudPath))
                {
                    cloudPath = _fixedCloudPath;
                    reader.Close();

                    // Update the database with the correct path
                    var updateCommand = new SqlCommand(@"
                        UPDATE SyncConfiguration 
                        SET CloudFolderPath = @CloudFolderPath 
                        WHERE DeviceId = @DeviceId", connection);
                    updateCommand.Parameters.AddWithValue("@CloudFolderPath", cloudPath);
                    updateCommand.Parameters.AddWithValue("@DeviceId", _deviceId);
                    updateCommand.ExecuteNonQuery();
                }
                else
                {
                    reader.Close();
                }

                var config = new SyncConfiguration
                {
                    Id = 1,
                    DeviceId = _deviceId,
                    LastSyncDate = new DateTime(1900, 1, 1),
                    CloudFolderPath = cloudPath,
                    IsEnabled = true,
                    CreatedDate = DateTime.Now
                };

                System.Windows.MessageBox.Show($"Config loaded - Path: '{config.CloudFolderPath}'", "Config Debug");
                return config;
            }

            return CreateDefaultConfiguration();
        }
        public void ResetLastSyncDate()
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
        UPDATE SyncConfiguration 
        SET LastSyncDate = '1900-01-01' 
        WHERE DeviceId = @DeviceId", connection);

            command.Parameters.AddWithValue("@DeviceId", _deviceId);
            command.ExecuteNonQuery();

            System.Windows.MessageBox.Show("LastSyncDate reset to 1900-01-01", "Reset Complete");
        }
        public void UpdateConfiguration(SyncConfiguration config)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var command = new SqlCommand(@"
                UPDATE SyncConfiguration 
                SET LastSyncDate = @LastSyncDate, CloudFolderPath = @CloudFolderPath, IsEnabled = @IsEnabled 
                WHERE DeviceId = @DeviceId", connection);

            command.Parameters.AddWithValue("@DeviceId", config.DeviceId);
            command.Parameters.AddWithValue("@LastSyncDate", config.LastSyncDate);
            command.Parameters.AddWithValue("@CloudFolderPath", config.CloudFolderPath);
            command.Parameters.AddWithValue("@IsEnabled", config.IsEnabled);

            command.ExecuteNonQuery();
        }

        private SyncConfiguration CreateDefaultConfiguration()
        {
            System.Windows.MessageBox.Show($"Creating default config with path: '{_fixedCloudPath}'", "Config Debug");

            var config = new SyncConfiguration
            {
                DeviceId = _deviceId,
                LastSyncDate = new DateTime(1900, 1, 1),
                CloudFolderPath = _fixedCloudPath,
                IsEnabled = true,
                CreatedDate = DateTime.Now
            };

            using var connection = _dbService.GetConnection();
            connection.Open();

            // Delete any existing config for this device
            var deleteCommand = new SqlCommand("DELETE FROM SyncConfiguration WHERE DeviceId = @DeviceId", connection);
            deleteCommand.Parameters.AddWithValue("@DeviceId", _deviceId);
            deleteCommand.ExecuteNonQuery();

            // Insert new config
            var command = new SqlCommand(@"
                INSERT INTO SyncConfiguration (DeviceId, LastSyncDate, CloudFolderPath, IsEnabled, CreatedDate) 
                VALUES (@DeviceId, @LastSyncDate, @CloudFolderPath, @IsEnabled, @CreatedDate)", connection);

            command.Parameters.AddWithValue("@DeviceId", config.DeviceId);
            command.Parameters.AddWithValue("@LastSyncDate", config.LastSyncDate);
            command.Parameters.AddWithValue("@CloudFolderPath", config.CloudFolderPath);
            command.Parameters.AddWithValue("@IsEnabled", config.IsEnabled);
            command.Parameters.AddWithValue("@CreatedDate", config.CreatedDate);

            command.ExecuteNonQuery();

            return config;
        }

        public string GetDeviceId() => _deviceId;

        public string GetMyChangesFolder()
        {
            var config = GetConfiguration();
            var basePath = config.CloudFolderPath ?? _fixedCloudPath;
            var fullPath = Path.Combine(basePath, $"{_deviceId}_Changes");

            System.Windows.MessageBox.Show($"My changes folder: '{fullPath}'", "Path Debug");

            return fullPath;
        }

        public string GetOtherDevicesFolder()
        {
            var config = GetConfiguration();
            return config.CloudFolderPath ?? _fixedCloudPath;
        }
    }
}