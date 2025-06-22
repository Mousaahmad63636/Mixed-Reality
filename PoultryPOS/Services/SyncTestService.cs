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
                    foreach (var change in dailyFiles.SelectMany(df => df.Changes))
                    {
                        if (change.Table.ToLower() == "sales")
                        {
                            // Process sales changes silently
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Silent failure for debug operations
            }
        }

        public void TestDailySyncFile()
        {
            try
            {
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
            }
            catch (Exception)
            {
                // Silent failure for test operations
            }
        }
    }
}