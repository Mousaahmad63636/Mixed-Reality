using PoultryPOS.Services;
using System.Windows;

namespace PoultryPOS
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                var dbService = new DatabaseService();
                dbService.InitializeDatabase();

                var syncTest = new SyncTestService();
                var accessWorking = syncTest.TestCloudAccess();

                if (accessWorking)
                {
                    System.Windows.MessageBox.Show("Cloud sync setup successful!", "Sync Ready");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Application initialization failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}