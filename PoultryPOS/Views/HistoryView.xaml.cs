using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Windows;
using System.Windows.Controls;

namespace PoultryPOS.Views
{
    public partial class HistoryView : Page
    {
        private readonly SaleService _saleService;
        private readonly PaymentService _paymentService;
        private readonly CustomerService _customerService;
        private List<Transaction> _allTransactions;

        public HistoryView()
        {
            InitializeComponent();
            _saleService = new SaleService();
            _paymentService = new PaymentService();
            _customerService = new CustomerService();

            LoadData();
            UpdateSummary();
        }

        private void LoadData()
        {
            LoadAllTransactions();
            LoadCustomerFilter();
            FilterTransactions(null, null);
        }

        private void LoadAllTransactions()
        {
            _allTransactions = new List<Transaction>();

            var sales = _saleService.GetAll();
            foreach (var sale in sales)
            {
                _allTransactions.Add(new Transaction
                {
                    Date = sale.SaleDate,
                    CustomerName = sale.CustomerName ?? "",
                    Type = "Sale",
                    Amount = sale.TotalAmount,
                    AmountDisplay = $"+{sale.TotalAmount:C}",
                    TruckName = sale.TruckName,
                    DriverName = sale.DriverName,
                    Notes = $"{sale.NetWeight:F2}kg @ {sale.PricePerKg:C}/kg",
                    CustomerId = sale.CustomerId
                });
            }

            var payments = _paymentService.GetAll();
            foreach (var payment in payments)
            {
                _allTransactions.Add(new Transaction
                {
                    Date = payment.PaymentDate,
                    CustomerName = payment.CustomerName ?? "",
                    Type = "Payment",
                    Amount = payment.Amount,
                    AmountDisplay = $"-{payment.Amount:C}",
                    TruckName = "-",
                    DriverName = "-",
                    Notes = payment.Notes,
                    CustomerId = payment.CustomerId
                });
            }

            _allTransactions = _allTransactions.OrderByDescending(t => t.Date).ToList();
        }

        private void LoadCustomerFilter()
        {
            var customers = _customerService.GetAll().OrderBy(c => c.Name).ToList();

            cmbCustomerFilter.Items.Clear();
            cmbCustomerFilter.Items.Add(new ComboBoxItem { Content = "All Customers", Tag = -1 });

            foreach (var customer in customers)
            {
                cmbCustomerFilter.Items.Add(new ComboBoxItem { Content = customer.Name, Tag = customer.Id });
            }

            cmbCustomerFilter.SelectedIndex = 0;
        }

        private void UpdateSummary()
        {
            var totalSales = _saleService.GetTotalSales();
            lblTotalSales.Text = totalSales.ToString("C");

            var customers = _customerService.GetAll();
            var outstandingBalance = customers.Sum(c => c.Balance);
            lblOutstandingBalance.Text = outstandingBalance.ToString("C");
        }

        private void FilterTransactions(object sender, SelectionChangedEventArgs e)
        {
            if (_allTransactions == null) return;

            var filteredTransactions = _allTransactions.AsEnumerable();

            if (dpFromDate.SelectedDate.HasValue)
            {
                filteredTransactions = filteredTransactions.Where(t => t.Date.Date >= dpFromDate.SelectedDate.Value.Date);
            }

            if (dpToDate.SelectedDate.HasValue)
            {
                filteredTransactions = filteredTransactions.Where(t => t.Date.Date <= dpToDate.SelectedDate.Value.Date);
            }

            if (cmbCustomerFilter.SelectedItem is ComboBoxItem customerItem && (int)customerItem.Tag != -1)
            {
                var customerId = (int)customerItem.Tag;
                filteredTransactions = filteredTransactions.Where(t => t.CustomerId == customerId);
            }

            if (cmbTypeFilter.SelectedItem is ComboBoxItem typeItem)
            {
                var typeFilter = typeItem.Content.ToString();
                if (typeFilter == "Sales Only")
                {
                    filteredTransactions = filteredTransactions.Where(t => t.Type == "Sale");
                }
                else if (typeFilter == "Payments Only")
                {
                    filteredTransactions = filteredTransactions.Where(t => t.Type == "Payment");
                }
            }

            var results = filteredTransactions.ToList();
            dgTransactions.ItemsSource = results;

            var periodSales = results.Where(t => t.Type == "Sale").Sum(t => t.Amount);
            lblPeriodSales.Text = periodSales.ToString("C");
            lblPeriodTransactions.Text = results.Count.ToString();
        }

        private void BtnToday_Click(object sender, RoutedEventArgs e)
        {
            dpFromDate.SelectedDate = DateTime.Today;
            dpToDate.SelectedDate = DateTime.Today;
            FilterTransactions(null, null);
        }

        private void BtnThisWeek_Click(object sender, RoutedEventArgs e)
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);

            dpFromDate.SelectedDate = startOfWeek;
            dpToDate.SelectedDate = today;
            FilterTransactions(null, null);
        }

        private void BtnThisMonth_Click(object sender, RoutedEventArgs e)
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            dpFromDate.SelectedDate = startOfMonth;
            dpToDate.SelectedDate = today;
            FilterTransactions(null, null);
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            dpFromDate.SelectedDate = null;
            dpToDate.SelectedDate = null;
            cmbCustomerFilter.SelectedIndex = 0;
            cmbTypeFilter.SelectedIndex = 0;
            FilterTransactions(null, null);
        }
    }
}