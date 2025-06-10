using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Windows;
using System.Windows.Controls;

namespace PoultryPOS.Views
{
    public partial class CustomersView : Page
    {
        private readonly CustomerService _customerService;
        private readonly PaymentService _paymentService;
        private Customer _selectedCustomer;

        public CustomersView()
        {
            InitializeComponent();
            _customerService = new CustomerService();
            _paymentService = new PaymentService();
            LoadData();
            UpdateSummary();
        }

        private void LoadData()
        {
            LoadCustomers();
            LoadPaymentCustomers();
            LoadCustomersWithBalance();
        }

        private void LoadCustomers()
        {
            dgCustomers.ItemsSource = _customerService.GetAll();
        }

        private void LoadPaymentCustomers()
        {
            cmbPaymentCustomer.ItemsSource = _customerService.GetAll().Where(c => c.Balance > 0).ToList();
            cmbPaymentCustomer.DisplayMemberPath = "Name";
            cmbPaymentCustomer.SelectedValuePath = "Id";
        }

        private void LoadCustomersWithBalance()
        {
            dgCustomersWithBalance.ItemsSource = _customerService.GetAll().Where(c => c.Balance > 0).ToList();
        }

        private void UpdateSummary()
        {
            var customers = _customerService.GetAll();
            var customersWithBalance = customers.Where(c => c.Balance > 0).ToList();
            var totalBalance = customers.Sum(c => c.Balance);

            lblTotalCustomers.Text = $"Total Customers: {customers.Count}";
            lblCustomersWithBalance.Text = $"With Balance: {customersWithBalance.Count}";
            lblTotalBalance.Text = $"Total Balance: {totalBalance:C}";
        }

        private void DgCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer customer)
            {
                _selectedCustomer = customer;
                txtCustomerName.Text = customer.Name;
                txtCustomerPhone.Text = customer.Phone ?? "";
                txtInitialBalance.Text = customer.Balance.ToString("F2");
            }
        }

        private void CmbPaymentCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPaymentCustomer.SelectedValue != null)
            {
                var customerId = (int)cmbPaymentCustomer.SelectedValue;
                var customer = _customerService.GetById(customerId);
                if (customer != null)
                {
                    lblCustomerBalance.Text = $"Current Balance: {customer.Balance:C}";
                }
            }
        }

        private void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Please enter customer name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtInitialBalance.Text, out decimal initialBalance) || initialBalance < 0)
            {
                MessageBox.Show("Please enter a valid initial balance.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var customer = new Customer
                {
                    Name = txtCustomerName.Text.Trim(),
                    Phone = string.IsNullOrWhiteSpace(txtCustomerPhone.Text) ? null : txtCustomerPhone.Text.Trim(),
                    Balance = initialBalance
                };

                _customerService.Add(customer);
                LoadData();
                UpdateSummary();
                ClearCustomerForm();
                MessageBox.Show("Customer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer to update.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("Please enter customer name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtInitialBalance.Text, out decimal balance) || balance < 0)
            {
                MessageBox.Show("Please enter a valid balance.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _selectedCustomer.Name = txtCustomerName.Text.Trim();
                _selectedCustomer.Phone = string.IsNullOrWhiteSpace(txtCustomerPhone.Text) ? null : txtCustomerPhone.Text.Trim();
                _selectedCustomer.Balance = balance;

                _customerService.Update(_selectedCustomer);
                LoadData();
                UpdateSummary();
                ClearCustomerForm();
                MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete customer '{_selectedCustomer.Name}'?",
                                       "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _customerService.Delete(_selectedCustomer.Id);
                    LoadData();
                    UpdateSummary();
                    ClearCustomerForm();
                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClearCustomer_Click(object sender, RoutedEventArgs e)
        {
            ClearCustomerForm();
        }

        private void ClearCustomerForm()
        {
            txtCustomerName.Clear();
            txtCustomerPhone.Clear();
            txtInitialBalance.Text = "0";
            _selectedCustomer = null;
            dgCustomers.SelectedItem = null;
        }

        private void BtnReceivePayment_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPaymentCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPaymentAmount.Text, out decimal paymentAmount) || paymentAmount <= 0)
            {
                MessageBox.Show("Please enter a valid payment amount.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var customerId = (int)cmbPaymentCustomer.SelectedValue;
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
                UpdateSummary();
                ClearPaymentForm();
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
Receipt #: {payment.Id}

Customer: {customer.Name}
Payment Amount: {payment.Amount:C}
Previous Balance: {customer.Balance:C}
New Balance: {newBalance:C}

{(string.IsNullOrEmpty(payment.Notes) ? "" : $"Notes: {payment.Notes}")}

Thank you for your payment!
=====================================";

            MessageBox.Show(receipt, "Payment Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearPaymentForm()
        {
            cmbPaymentCustomer.SelectedIndex = -1;
            txtPaymentAmount.Clear();
            txtPaymentNotes.Clear();
            lblCustomerBalance.Text = "";
        }
    }
}