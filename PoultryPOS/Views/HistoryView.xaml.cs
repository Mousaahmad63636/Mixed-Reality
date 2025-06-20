﻿using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PoultryPOS.Views
{
    public partial class HistoryView : Page
    {
        private readonly SaleService _saleService;
        private readonly PaymentService _paymentService;
        private readonly CustomerService _customerService;
        private List<Transaction> _allTransactions;
        private List<Transaction> _filteredTransactions;

        private int _currentPage = 1;
        private int _itemsPerPage = 25;
        private int _totalPages = 1;

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
                    TypeArabic = "مبيعة",
                    Amount = sale.TotalAmount,
                    AmountDisplay = $"+{sale.TotalAmount:C}",
                    TruckName = sale.TruckName,
                    DriverName = sale.DriverName,
                    Notes = $"{sale.NetWeight:F2}كغ @ {sale.PricePerKg:C}/كغ",
                    CustomerId = sale.CustomerId,
                    TransactionId = sale.Id
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
                    TypeArabic = "دفعة",
                    Amount = payment.Amount,
                    AmountDisplay = $"-{payment.Amount:C}",
                    TruckName = "-",
                    DriverName = "-",
                    Notes = payment.Notes,
                    CustomerId = payment.CustomerId,
                    TransactionId = payment.Id
                });
            }

            _allTransactions = _allTransactions.OrderByDescending(t => t.Date).ToList();
        }

        private void BtnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Transaction transaction)
            {
                var detailsWindow = new TransactionDetailsWindow(transaction.Type, transaction.TransactionId);
                detailsWindow.ShowDialog();

                LoadData();
                UpdateSummary();
            }
        }

        private void LoadCustomerFilter()
        {
            var customers = _customerService.GetAll().OrderBy(c => c.Name).ToList();

            cmbCustomerFilter.Items.Clear();
            cmbCustomerFilter.Items.Add(new ComboBoxItem { Content = "جميع العملاء", Tag = -1 });

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

            int? selectedCustomerId = null;
            if (cmbCustomerFilter.SelectedItem is ComboBoxItem customerItem && (int)customerItem.Tag != -1)
            {
                selectedCustomerId = (int)customerItem.Tag;
                filteredTransactions = filteredTransactions.Where(t => t.CustomerId == selectedCustomerId);
            }

            if (cmbTypeFilter.SelectedItem is ComboBoxItem typeItem)
            {
                var typeFilter = typeItem.Content.ToString();
                if (typeFilter == "المبيعات فقط")
                {
                    filteredTransactions = filteredTransactions.Where(t => t.Type == "Sale");
                }
                else if (typeFilter == "المدفوعات فقط")
                {
                    filteredTransactions = filteredTransactions.Where(t => t.Type == "Payment");
                }
            }

            _filteredTransactions = filteredTransactions.ToList();
            _currentPage = 1;
            UpdatePaginationAndDisplay();
            UpdateOutstandingBalanceForSelectedCustomer(selectedCustomerId);
        }

        private void UpdateOutstandingBalanceForSelectedCustomer(int? customerId)
        {
            if (customerId.HasValue)
            {
                var customer = _customerService.GetById(customerId.Value);
                if (customer != null)
                {
                    lblOutstandingBalance.Text = customer.Balance.ToString("C");
                }
                else
                {
                    lblOutstandingBalance.Text = "$0.00";
                }
            }
            else
            {
                var customers = _customerService.GetAll();
                var totalOutstanding = customers.Sum(c => c.Balance);
                lblOutstandingBalance.Text = totalOutstanding.ToString("C");
            }
        }
        private void UpdatePaginationAndDisplay()
        {
            if (_filteredTransactions == null)
            {
                dgTransactions.ItemsSource = null;
                UpdatePaginationInfo(0, 0, 0);
                UpdatePaginationButtons();
                return;
            }

            if (!_filteredTransactions.Any())
            {
                dgTransactions.ItemsSource = null;
                UpdatePaginationInfo(0, 0, 0);
                UpdatePaginationButtons();
                return;
            }

            _totalPages = (int)Math.Ceiling((double)_filteredTransactions.Count / _itemsPerPage);

            if (_currentPage > _totalPages)
                _currentPage = _totalPages;

            var startIndex = (_currentPage - 1) * _itemsPerPage;
            var pageData = _filteredTransactions.Skip(startIndex).Take(_itemsPerPage).ToList();

            dgTransactions.ItemsSource = pageData;

            var periodSales = _filteredTransactions.Where(t => t.Type == "Sale").Sum(t => t.Amount);
            lblPeriodSales.Text = periodSales.ToString("C");
            lblPeriodTransactions.Text = _filteredTransactions.Count.ToString();

            var displayStart = startIndex + 1;
            var displayEnd = Math.Min(startIndex + _itemsPerPage, _filteredTransactions.Count);
            UpdatePaginationInfo(displayStart, displayEnd, _filteredTransactions.Count);
            UpdatePaginationButtons();
            UpdatePageNumbers();
        }

        private void UpdatePaginationInfo(int start, int end, int total)
        {
            if (total == 0)
            {
                lblPaginationInfo.Text = "لا توجد سجلات";
            }
            else
            {
                lblPaginationInfo.Text = $"عرض {start}-{end} من {total} سجل (الصفحة {_currentPage} من {_totalPages})";
            }
        }

        private void UpdatePaginationButtons()
        {
            if (_totalPages <= 0)
            {
                btnFirstPage.IsEnabled = false;
                btnPreviousPage.IsEnabled = false;
                btnNextPage.IsEnabled = false;
                btnLastPage.IsEnabled = false;
                return;
            }

            btnFirstPage.IsEnabled = _currentPage > 1;
            btnPreviousPage.IsEnabled = _currentPage > 1;
            btnNextPage.IsEnabled = _currentPage < _totalPages;
            btnLastPage.IsEnabled = _currentPage < _totalPages;
        }

        private void UpdatePageNumbers()
        {
            spPageNumbers.Children.Clear();

            if (_totalPages <= 0) return;

            int startPage = Math.Max(1, _currentPage - 2);
            int endPage = Math.Min(_totalPages, _currentPage + 2);

            for (int i = startPage; i <= endPage; i++)
            {
                var pageButton = new Button
                {
                    Content = i.ToString(),
                    Tag = i,
                    Style = i == _currentPage ?
                        (Style)Resources["ActivePageButton"] :
                        (Style)Resources["PaginationButton"]
                };

                pageButton.Click += PageNumber_Click;
                spPageNumbers.Children.Add(pageButton);
            }
        }

        private void PageNumber_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int page))
            {
                _currentPage = page;
                UpdatePaginationAndDisplay();
            }
        }

        private void BtnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            _currentPage = 1;
            UpdatePaginationAndDisplay();
        }

        private void BtnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePaginationAndDisplay();
            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                UpdatePaginationAndDisplay();
            }
        }

        private void BtnLastPage_Click(object sender, RoutedEventArgs e)
        {
            _currentPage = _totalPages;
            UpdatePaginationAndDisplay();
        }

        private void BtnJumpToPage_Click(object sender, RoutedEventArgs e)
        {
            if (_totalPages <= 0)
            {
                MessageBox.Show("لا توجد بيانات متاحة للترقيم", "لا توجد بيانات", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (int.TryParse(txtJumpToPage.Text, out int page))
            {
                if (page >= 1 && page <= _totalPages)
                {
                    _currentPage = page;
                    UpdatePaginationAndDisplay();
                    txtJumpToPage.Clear();
                }
                else
                {
                    MessageBox.Show($"يرجى إدخال رقم صفحة بين 1 و {_totalPages}", "صفحة غير صالحة", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void TxtJumpToPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnJumpToPage_Click(sender, e);
            }
        }

        private void CmbItemsPerPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbItemsPerPage.SelectedItem is ComboBoxItem item && _filteredTransactions != null)
            {
                _itemsPerPage = int.Parse(item.Content.ToString());
                _currentPage = 1;
                UpdatePaginationAndDisplay();
            }
        }

        private void BtnToday_Click(object sender, RoutedEventArgs e)
        {
            dpFromDate.SelectedDate = DateTime.Today;
            dpToDate.SelectedDate = DateTime.Today;
        }

        private void BtnThisWeek_Click(object sender, RoutedEventArgs e)
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            dpFromDate.SelectedDate = startOfWeek;
            dpToDate.SelectedDate = today;
        }

        private void BtnThisMonth_Click(object sender, RoutedEventArgs e)
        {
            var today = DateTime.Today;
            dpFromDate.SelectedDate = new DateTime(today.Year, today.Month, 1);
            dpToDate.SelectedDate = today;
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            dpFromDate.SelectedDate = null;
            dpToDate.SelectedDate = null;
            cmbCustomerFilter.SelectedIndex = 0;
            cmbTypeFilter.SelectedIndex = 0;
        }
    }

    public class Transaction
    {
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string Type { get; set; }
        public string TypeArabic { get; set; }
        public decimal Amount { get; set; }
        public string AmountDisplay { get; set; }
        public string TruckName { get; set; }
        public string DriverName { get; set; }
        public string Notes { get; set; }
        public int CustomerId { get; set; }
        public int TransactionId { get; set; }
    }
}