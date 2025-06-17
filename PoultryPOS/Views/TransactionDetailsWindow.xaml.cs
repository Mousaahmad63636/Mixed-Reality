using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PoultryPOS.Views
{
    public partial class TransactionDetailsWindow : Window
    {
        private readonly SaleService _saleService;
        private readonly PaymentService _paymentService;
        private readonly CustomerService _customerService;
        private readonly TruckService _truckService;
        private readonly DriverService _driverService;

        private Sale _currentSale;
        private Payment _currentPayment;
        private ObservableCollection<SaleItem> _saleItems;
        private bool _isEditMode = false;
        private string _transactionType;
        private int _transactionId;

        public TransactionDetailsWindow(string transactionType, int transactionId)
        {
            InitializeComponent();

            _saleService = new SaleService();
            _paymentService = new PaymentService();
            _customerService = new CustomerService();
            _truckService = new TruckService();
            _driverService = new DriverService();

            _transactionType = transactionType;
            _transactionId = transactionId;
            _saleItems = new ObservableCollection<SaleItem>();

            LoadData();
            LoadTransaction();
        }

        private void LoadData()
        {
            cmbCustomer.ItemsSource = _customerService.GetAll();
            cmbCustomer.DisplayMemberPath = "Name";
            cmbCustomer.SelectedValuePath = "Id";

            cmbTruck.ItemsSource = _truckService.GetAll();
            cmbTruck.DisplayMemberPath = "Name";
            cmbTruck.SelectedValuePath = "Id";

            cmbDriver.ItemsSource = _driverService.GetAll();
            cmbDriver.DisplayMemberPath = "Name";
            cmbDriver.SelectedValuePath = "Id";
        }

        private void LoadTransaction()
        {
            if (_transactionType == "Sale")
            {
                LoadSaleTransaction();
            }
            else if (_transactionType == "Payment")
            {
                LoadPaymentTransaction();
            }
        }

        private void LoadSaleTransaction()
        {
            _currentSale = _saleService.GetSaleById(_transactionId);
            if (_currentSale == null)
            {
                MessageBox.Show("لم يتم العثور على المعاملة.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            lblTitle.Text = $"تفاصيل المبيعة - فاتورة رقم {_currentSale.Id}";

            cmbCustomer.SelectedValue = _currentSale.CustomerId;
            cmbTruck.SelectedValue = _currentSale.TruckId;
            cmbDriver.SelectedValue = _currentSale.DriverId;
            txtPricePerKg.Text = _currentSale.PricePerKg.ToString("F2");

            var saleItems = _saleService.GetSaleItems(_currentSale.Id);
            _saleItems.Clear();

            foreach (var item in saleItems)
            {
                item.UpdatePrice(_currentSale.PricePerKg);
                item.PropertyChanged += SaleItem_PropertyChanged;
                _saleItems.Add(item);
            }

            dgSaleItems.ItemsSource = _saleItems;
            UpdateTotals();

            lblPaymentStatus.Text = _currentSale.IsPaidNow ? "الحالة: مدفوع نقداً" : "الحالة: مضاف للحساب";
            lblPaymentStatus.Foreground = _currentSale.IsPaidNow ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;

            spSaleDetails.Visibility = Visibility.Visible;
            spPaymentDetails.Visibility = Visibility.Collapsed;
        }

        private void SaleItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SaleItem.TotalAmount))
            {
                UpdateTotals();
            }
        }

        private void LoadPaymentTransaction()
        {
            var payments = _paymentService.GetAll();
            _currentPayment = payments.FirstOrDefault(p => p.Id == _transactionId);

            if (_currentPayment == null)
            {
                MessageBox.Show("لم يتم العثور على المعاملة.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            lblTitle.Text = $"تفاصيل الدفعة - إيصال رقم {_currentPayment.Id}";

            cmbCustomer.SelectedValue = _currentPayment.CustomerId;
            txtPaymentAmount.Text = _currentPayment.Amount.ToString("C");
            txtPaymentNotes.Text = _currentPayment.Notes ?? "";

            spSaleDetails.Visibility = Visibility.Collapsed;
            spPaymentDetails.Visibility = Visibility.Visible;
            btnEditMode.Visibility = Visibility.Collapsed;
        }

        private void UpdateTotals()
        {
            if (_saleItems == null) return;

            var totalAmount = _saleItems.Sum(item => item.TotalAmount);
            var itemCount = _saleItems.Count;
            lblTotals.Text = $"إجمالي الفاتورة: {totalAmount:C} | عدد البنود: {itemCount}";
        }

        private void BtnEditMode_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = !_isEditMode;

            if (_isEditMode)
            {
                btnEditMode.Content = "إلغاء التعديل";
                btnEditMode.Background = System.Windows.Media.Brushes.Orange;
                btnAddItem.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Visible;
                dgSaleItems.IsReadOnly = false;

                cmbCustomer.IsEnabled = true;
                cmbTruck.IsEnabled = true;
                cmbDriver.IsEnabled = true;
                txtPricePerKg.IsReadOnly = false;

                foreach (var column in dgSaleItems.Columns)
                {
                    if (column.Header.ToString() == "الإجراء")
                    {
                        if (column is DataGridTemplateColumn templateColumn)
                        {
                            foreach (var item in dgSaleItems.Items)
                            {
                                var row = dgSaleItems.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                                if (row != null)
                                {
                                    var cell = dgSaleItems.Columns[dgSaleItems.Columns.Count - 1].GetCellContent(row);
                                    if (cell is Button button)
                                    {
                                        button.Visibility = Visibility.Visible;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                btnEditMode.Content = "تفعيل التعديل";
                btnEditMode.Background = System.Windows.Media.Brushes.Blue;
                btnAddItem.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Collapsed;
                dgSaleItems.IsReadOnly = true;

                cmbCustomer.IsEnabled = false;
                cmbTruck.IsEnabled = false;
                cmbDriver.IsEnabled = false;
                txtPricePerKg.IsReadOnly = true;

                LoadSaleTransaction();
            }
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (!_isEditMode) return;

            var newItem = new SaleItem();
            if (decimal.TryParse(txtPricePerKg.Text, out decimal price))
            {
                newItem.UpdatePrice(price);
            }

            newItem.PropertyChanged += SaleItem_PropertyChanged;
            _saleItems.Add(newItem);
            UpdateTotals();
        }

        private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (!_isEditMode) return;

            if (sender is Button button && button.DataContext is SaleItem saleItem)
            {
                saleItem.PropertyChanged -= SaleItem_PropertyChanged;
                _saleItems.Remove(saleItem);
                UpdateTotals();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!_isEditMode) return;

            if (!ValidateData()) return;

            try
            {
                if (decimal.TryParse(txtPricePerKg.Text, out decimal pricePerKg))
                {
                    foreach (var item in _saleItems)
                    {
                        item.UpdatePrice(pricePerKg);
                    }
                }

                _currentSale.CustomerId = (int)cmbCustomer.SelectedValue;
                _currentSale.TruckId = (int)cmbTruck.SelectedValue;
                _currentSale.DriverId = (int)cmbDriver.SelectedValue;
                _currentSale.PricePerKg = pricePerKg;

                _currentSale.GrossWeight = _saleItems.Sum(item => item.GrossWeight);
                _currentSale.NumberOfCages = _saleItems.Sum(item => item.NumberOfCages);
                _currentSale.CageWeight = _saleItems.Sum(item => item.TotalCageWeight);
                _currentSale.NetWeight = _saleItems.Sum(item => item.NetWeight);
                _currentSale.TotalAmount = _saleItems.Sum(item => item.TotalAmount);

                _saleService.UpdateWithItems(_currentSale, _saleItems.ToList());

                MessageBox.Show("تم حفظ التغييرات بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnEditMode_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ التغييرات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool ValidateData()
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار عميل.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbTruck.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار شاحنة.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cmbDriver.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار سائق.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPricePerKg.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("يرجى إدخال سعر صالح للكيلو.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (_saleItems.Count == 0)
            {
                MessageBox.Show("يرجى إضافة عنصر بيع واحد على الأقل.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

  
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                BtnEditMode_Click(sender, e);
            }
            else
            {
                Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}