using PoultryPOS.Models;

namespace PoultryPOS.Services
{
    public class SyncService
    {
        private readonly FileOperationsService _fileService;
        private readonly SyncApplicationService _syncApp;
        private readonly SyncConfigurationService _configService;

        public SyncService()
        {
            _fileService = new FileOperationsService();
            _syncApp = new SyncApplicationService();
            _configService = new SyncConfigurationService();
        }

        public void PerformSync()
        {
            try
            {
                System.Windows.MessageBox.Show("Starting daily sync process...", "Daily Sync");

                var dailyFiles = _fileService.GetNewDailyFilesFromOtherDevices();

                System.Windows.MessageBox.Show($"Found {dailyFiles.Count} daily files to process", "Daily Sync");

                if (dailyFiles.Count == 0)
                {
                    System.Windows.MessageBox.Show("No daily files found from other devices.", "Daily Sync");
                    return;
                }

                int totalChanges = 0;
                foreach (var dailyFile in dailyFiles)
                {
                    System.Windows.MessageBox.Show($"Processing daily file from {dailyFile.DeviceId} for {dailyFile.Date} with {dailyFile.Changes.Count} changes", "Daily Sync");

                    foreach (var change in dailyFile.Changes)
                    {
                        _syncApp.ApplyChangesToLocal(change);
                        totalChanges++;
                    }
                }

                System.Windows.MessageBox.Show($"Daily sync completed! Applied {totalChanges} total changes.", "Daily Sync Complete");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Daily sync failed: {ex.Message}", "Sync Error");
            }
        }
    }
}