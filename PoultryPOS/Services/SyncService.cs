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
                var dailyFiles = _fileService.GetNewDailyFilesFromOtherDevices();

                if (dailyFiles.Count == 0)
                {
                    return;
                }

                int totalChanges = 0;
                foreach (var dailyFile in dailyFiles)
                {
                    foreach (var change in dailyFile.Changes)
                    {
                        _syncApp.ApplyChangesToLocal(change);
                        totalChanges++;
                    }
                }

                if (totalChanges > 0)
                {
                    System.Windows.MessageBox.Show($"Sync completed! Applied {totalChanges} changes.", "Sync Complete");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Sync failed: {ex.Message}", "Sync Error");
            }
        }
    }
}