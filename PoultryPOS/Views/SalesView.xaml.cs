using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Windows;
using System.Windows.Controls;

namespace PoultryPOS.Views
{
    public partial class SalesView : Page
    {
        private readonly TruckService _truckService;
        private readonly DriverService _driverService;
        private readonly CustomerService _customerService;
        private readonly SaleService _saleService;

        public SalesView()
        {
            InitializeComponent();
            _truckService = new TruckService();
            _driverService = new DriverService();
            _customerService = new CustomerService();
            _saleService = new SaleService();

            LoadData();
            LoadTodaySales();
        }

        private void LoadData()
        {
            cmbTruck.ItemsSource = _truckService.GetAll();
            cmbTruck.DisplayMemberPath = "Name";
            cmbTruck.SelectedValuePath = "Id";

            cmbDriver.ItemsSource = _driverService.GetAll();
            cmbDriver.DisplayMemberPath = "Name";
            cmbDriver.SelectedValuePath = "Id";

            cmbCustomer.ItemsSource = _customerService.GetAll();
            cmbCustomer.DisplayMemberPath = "Name";
            cmbCustomer.SelectedValuePath = "Id";
        }

        private void LoadTodaySales()
        {
            var todaySales = _saleService.GetByDateRange(DateTime.Today, DateTime.Today);
            dgTodaySales.ItemsSource = todaySales;

            var todayTotal = todaySales.Sum(s => s.TotalAmount);
            lblTodayTotal.Text = $"Today's Total: {todayTotal:C}";
            lblTodayCount.Text = $"Transactions: {todaySales.Count}";
        }

        private void CmbCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCustomer.SelectedValue != null)
            {
                var customerId = (int)cmbCustomer.SelectedValue;
                var customer = _customerService.GetById(customerId);
                if (customer != null)
                {
                    if (customer.Balance > 0)
                    {
                        lblCurrentBalance.Text = $"Current Balance: {customer.Balance:C}";
                    }
                    else
                    {
                        lblCurrentBalance.Text = "";
                    }
                }
            }
        }

        private void CalculateWeights(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtGrossWeight.Text, out decimal grossWeight) &&
                decimal.TryParse(txtCageWeight.Text, out decimal cageWeight))
            {
                var netWeight = grossWeight - cageWeight;
                txtNetWeight.Text = netWeight.ToString("F2");
                CalculateTotal(null, null);
            }
        }

        private void CalculateTotal(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtNetWeight.Text, out decimal netWeight) &&
                decimal.TryParse(txtPricePerKg.Text, out decimal pricePerKg))
            {
                var totalAmount = netWeight * pricePerKg;
                txtTotalAmount.Text = totalAmount.ToString("F2");
            }
        }

        private bool ValidateForm()
        {
            if (cmbTruck.SelectedValue == null)
            {
                MessageBox.Show("Please select a truck.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbDriver.SelectedValue == null)
            {
                MessageBox.Show("Please select a driver.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtGrossWeight.Text, out _) || decimal.Parse(txtGrossWeight.Text) <= 0)
            {
                MessageBox.Show("Please enter a valid gross weight.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(txtNumberOfCages.Text, out _) || int.Parse(txtNumberOfCages.Text) <= 0)
            {
                MessageBox.Show("Please enter a valid number of cages.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtCageWeight.Text, out _) || decimal.Parse(txtCageWeight.Text) < 0)
            {
                MessageBox.Show("Please enter a valid cage weight.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPricePerKg.Text, out _) || decimal.Parse(txtPricePerKg.Text) <= 0)
            {
                MessageBox.Show("Please enter a valid price per kg.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (decimal.Parse(txtNetWeight.Text) <= 0)
            {
                MessageBox.Show("Net weight must be greater than zero.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private Sale CreateSaleFromForm(bool isPaidNow)
        {
            return new Sale
            {
                CustomerId = (int)cmbCustomer.SelectedValue,
                TruckId = (int)cmbTruck.SelectedValue,
                DriverId = (int)cmbDriver.SelectedValue,
                GrossWeight = decimal.Parse(txtGrossWeight.Text),
                NumberOfCages = int.Parse(txtNumberOfCages.Text),
                CageWeight = decimal.Parse(txtCageWeight.Text),
                NetWeight = decimal.Parse(txtNetWeight.Text),
                PricePerKg = decimal.Parse(txtPricePerKg.Text),
                TotalAmount = decimal.Parse(txtTotalAmount.Text),
                IsPaidNow = isPaidNow,
                SaleDate = DateTime.Now
            };
        }

        private void BtnPayNow_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                var sale = CreateSaleFromForm(true);
                _saleService.Add(sale);

                MessageBox.Show("Sale completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                PrintReceipt(sale, false);
                ClearForm();
                LoadTodaySales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddToAccount_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                var sale = CreateSaleFromForm(false);
                _saleService.Add(sale);

                var customerId = (int)cmbCustomer.SelectedValue;
                var customer = _customerService.GetById(customerId);
                var newBalance = customer.Balance + sale.TotalAmount;
                _customerService.UpdateBalance(customerId, newBalance);

                MessageBox.Show("Sale added to customer account!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                PrintReceipt(sale, true);
                ClearForm();
                LoadTodaySales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintReceipt(Sale sale, bool addedToAccount)
        {
            var customer = _customerService.GetById(sale.CustomerId);
            var truck = _truckService.GetAll().First(t => t.Id == sale.TruckId);
            var driver = _driverService.GetAll().First(d => d.Id == sale.DriverId);

            var receipt = $@"
=====================================
         POULTRY SALES RECEIPT
=====================================
Date: {sale.SaleDate:yyyy-MM-dd HH:mm}
Receipt #: {sale.Id}

Customer: {customer.Name}
Truck: {truck.Name}
Driver: {driver.Name}

-------------------------------------
Gross Weight:     {sale.GrossWeight:F2} KG
Cages:            {sale.NumberOfCages}
Cage Weight:      {sale.CageWeight:F2} KG
Net Weight:       {sale.NetWeight:F2} KG
Price per KG:     {sale.PricePerKg:C}
-------------------------------------
TOTAL AMOUNT:     {sale.TotalAmount:C}

Payment: {(addedToAccount ? "Added to Account" : "Paid Now")}
{(addedToAccount ? $"New Balance: {customer.Balance + sale.TotalAmount:C}" : "")}

Thank you for your business!
=====================================";

            MessageBox.Show(receipt, "Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            cmbTruck.SelectedIndex = -1;
            cmbDriver.SelectedIndex = -1;
            cmbCustomer.SelectedIndex = -1;
            txtGrossWeight.Clear();
            txtNumberOfCages.Clear();
            txtCageWeight.Clear();
            txtNetWeight.Clear();
            txtPricePerKg.Clear();
            txtTotalAmount.Clear();
            lblCurrentBalance.Text = "";
        }
    }
}