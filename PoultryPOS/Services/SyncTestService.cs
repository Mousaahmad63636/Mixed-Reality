using PoultryPOS.Models;

namespace PoultryPOS.Services
{
    public class SyncTestService
    {
        private readonly FileOperationsService _fileService;
        private readonly SyncConfigurationService _configService;
        private readonly ChangeDetectionService _changeDetection;

        public SyncTestService()
        {
            _fileService = new FileOperationsService();
            _configService = new SyncConfigurationService();
            _changeDetection = new ChangeDetectionService();
        }

        public bool TestCloudAccess()
        {
            try
            {
                _fileService.EnsureFoldersExist();

                // Create a test change using the change detection service
                var testData = new Dictionary<string, object?>
                {
                    ["TestField"] = "Test Value",
                    ["Timestamp"] = DateTime.Now,
                    ["DeviceId"] = _configService.GetDeviceId()
                };

                _changeDetection.RecordChange("TestTable", 999, "TEST", testData);

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Cloud access test failed: {ex.Message}", "Sync Test",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }
        }
        public void DebugSyncFiles()
        {
            try
            {
                var fileService = new FileOperationsService();
                var dailyFiles = fileService.GetNewDailyFilesFromOtherDevices();

                foreach (var dailyFile in dailyFiles)
                {
                    System.Windows.MessageBox.Show($"Daily file from {dailyFile.DeviceId} has {dailyFile.Changes.Count} changes", "Debug");

                    foreach (var change in dailyFile.Changes)
                    {
                        var dataKeys = string.Join(", ", change.Data.Keys);
                        System.Windows.MessageBox.Show($"Change: {change.Table} {change.Operation} ID:{change.RecordId}\nData keys: {dataKeys}", "Debug");

                        if (change.Table.ToLower() == "sales")
                        {
                            var customerId = change.Data.ContainsKey("CustomerId") ? change.Data["CustomerId"]?.ToString() : "NULL";
                            var totalAmount = change.Data.ContainsKey("TotalAmount") ? change.Data["TotalAmount"]?.ToString() : "NULL";
                            System.Windows.MessageBox.Show($"Sales data - Customer: {customerId}, Amount: {totalAmount}", "Sales Debug");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Debug failed: {ex.Message}", "Error");
            }
        }
        public void TestDailySyncFile()
        {
            try
            {
                // Test creating a daily sync file with sample data
                var testChange = new SyncChange
                {
                    Table = "TestTable",
                    Operation = "TEST",
                    RecordId = 999,
                    SyncId = Guid.NewGuid().ToString(),
                    LastModified = DateTime.UtcNow,
                    Version = 1,
                    Data = new Dictionary<string, object?>
                    {
                        ["TestField"] = "Daily Test Value",
                        ["Timestamp"] = DateTime.Now,
                        ["DeviceId"] = _configService.GetDeviceId()
                    }
                };

                _fileService.AddChangeToTodaysFile(testChange);

                System.Windows.MessageBox.Show("Daily sync file test completed successfully!", "Test Success");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Daily sync file test failed: {ex.Message}", "Test Error");
            }
        }
    }
}