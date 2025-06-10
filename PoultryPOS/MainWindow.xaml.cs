using PoultryPOS.Views;
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
    }
}