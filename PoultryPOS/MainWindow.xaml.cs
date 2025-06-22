using PoultryPOS.Views;
using PoultryPOS.Services;
using PoultryPOS.Models;
using System.Windows;

namespace PoultryPOS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new SalesView());
        }

        private void BtnSales_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SalesView());
        }

        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CustomersView());
        }

        private void BtnTruckDriver_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TruckDriverView());
        }

        private void BtnPayments_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PaymentsView());
        }

        private void BtnHistory_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new HistoryView());
        }

        private void BtnVarianceReport_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new VarianceReportView());
        }

        private void BtnTestSync_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // First debug what's in the files
                var syncTest = new SyncTestService();
                syncTest.DebugSyncFiles();

                // Then try to sync
                var syncService = new SyncService();
                syncService.PerformSync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sync test failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}