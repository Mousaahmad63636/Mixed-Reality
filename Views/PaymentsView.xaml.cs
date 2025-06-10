using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Windows;
using System.Windows.Controls;

namespace PoultryPOS.Views
{
    public partial class PaymentsView : Page
    {
        private readonly CustomerService _customerService;
        private readonly PaymentService _paymentService;

        public PaymentsView()
        {
            InitializeComponent();
            _customerService = new CustomerService();
            _paymentService = new PaymentService();

            LoadData();
            UpdateStats();
            LoadAllPayments();
        }

        private void LoadData()
        {
            LoadCustomers();
            LoadCustomersWithBalance();
        }

        private void LoadCustomers()
        {
            cmbCustomer.ItemsSource = _customerService.GetAll().Where(c => c.Balance > 0).ToList();
            cmbCustomer.DisplayMemberPath = "Name";
            cmbCustomer.SelectedValuePath = "Id";
        }

        private void LoadCustomersWithBalance()
        {
            dgCustomersWithBalance.ItemsSource = _customerService.GetAll().Where(c => c.Balance > 0).ToList();
        }

        private void LoadAllPayments()
        {
            var payments = _paymentService.GetAll();
            dgPayments.ItemsSource = payments;

            var total = payments.Sum(p => p.Amount);
            lblFilteredTotal.Text = $"Total: {total:C}";
            lblFilteredCount.Text = $"Count: {payments.Count}";
        }

        private void UpdateStats()
        {
            var todayPayments = _paymentService.GetByDateRange(DateTime.Today, DateTime.Today);
            var todayTotal = todayPayments.Sum(p => p.Amount);

            var customers = _customerService.GetAll();
            var totalOutstanding = customers.Sum(c => c.Balance);

            lblTodayPayments.Text = $"Today's Payments: {todayTotal:C}";
            lblTodayCount.Text = $"Payment Count: {todayPayments.Count}";
            lblTotalOutstanding.Text = $"Total Outstanding: {totalOutstanding:C}";
        }

        private void CmbCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCustomer.SelectedValue != null)
            {
                var customerId = (int)cmbCustomer.SelectedValue;
                var customer = _customerService.GetById(customerId);
                if (customer != null)
                {
                    lblCurrentBalance.Text = $"Current Balance: {customer.Balance:C}";
                }
            }
        }

        private void BtnReceivePayment_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPaymentAmount.Text, out decimal paymentAmount) || paymentAmount <= 0)
            {
                MessageBox.Show("Please enter a valid payment amount.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var customerId = (int)cmbCustomer.SelectedValue;
            var customer = _customerService.GetById(customerId);

            if (customer == null)
            {
                MessageBox.Show("Customer not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (paymentAmount > customer.Balance)
            {
                var result = MessageBox.Show($"Payment amount ({paymentAmount:C}) is greater than customer balance ({customer.Balance:C}). Continue anyway?",
                                           "Payment Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                    return;
            }

            try
            {
                var payment = new Payment
                {
                    CustomerId = customerId,
                    Amount = paymentAmount,
                    PaymentDate = DateTime.Now,
                    Notes = string.IsNullOrWhiteSpace(txtPaymentNotes.Text) ? null : txtPaymentNotes.Text.Trim()
                };

                _paymentService.Add(payment);

                var newBalance = customer.Balance - paymentAmount;
                _customerService.UpdateBalance(customerId, newBalance);

                PrintPaymentReceipt(payment, customer, newBalance);
                LoadData();
                UpdateStats();
                LoadAllPayments();
                ClearForm();
                MessageBox.Show("Payment received successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintPaymentReceipt(Payment payment, Customer customer, decimal newBalance)
        {
            var receipt = $@"
=====================================
        PAYMENT RECEIPT
=====================================
Date: {payment.PaymentDate:yyyy-MM-dd HH:mm}
Receipt #: P{payment.PaymentDate:yyyyMMdd}-{payment.Id:D4}

Customer: {customer.Name}
Payment Amount: {payment.Amount:C}
Previous Balance: {customer.Balance:C}
New Balance: {newBalance:C}

{(string.IsNullOrEmpty(payment.Notes) ? "" : $"Notes: {payment.Notes}")}

Thank you for your payment!
=====================================";

            MessageBox.Show(receipt, "Payment Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FilterPayments(object sender, SelectionChangedEventArgs e)
        {
            if (dpFromDate.SelectedDate.HasValue && dpToDate.SelectedDate.HasValue)
            {
                var fromDate = dpFromDate.SelectedDate.Value;
                var toDate = dpToDate.SelectedDate.Value;

                if (fromDate <= toDate)
                {
                    var filteredPayments = _paymentService.GetByDateRange(fromDate, toDate);
                    dgPayments.ItemsSource = filteredPayments;

                    var total = filteredPayments.Sum(p => p.Amount);
                    lblFilteredTotal.Text = $"Filtered Total: {total:C}";
                    lblFilteredCount.Text = $"Count: {filteredPayments.Count}";
                }
            }
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            dpFromDate.SelectedDate = null;
            dpToDate.SelectedDate = null;
            LoadAllPayments();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            cmbCustomer.SelectedIndex = -1;
            txtPaymentAmount.Clear();
            txtPaymentNotes.Clear();
            lblCurrentBalance.Text = "";
        }
    }
}