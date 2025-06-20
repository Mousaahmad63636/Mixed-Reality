using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Printing;
using System.Windows.Input;

namespace PoultryPOS.Views
{
    public partial class SalesView : Page
    {
        private readonly TruckService _truckService;
        private readonly DriverService _driverService;
        private readonly CustomerService _customerService;
        private readonly SaleService _saleService;
        private ObservableCollection<SaleItem> _saleItems;
        private List<Customer> _allCustomers;

        public SalesView()
        {
            InitializeComponent();
            _truckService = new TruckService();
            _driverService = new DriverService();
            _customerService = new CustomerService();
            _saleService = new SaleService();
            _saleItems = new ObservableCollection<SaleItem>();
            SetupCustomerSearch();
            LoadData();
            SetupDataGrid();
            _saleItems.CollectionChanged += SaleItems_CollectionChanged;
        }

        private void LoadData()
        {
            cmbCustomer.ItemsSource = _customerService.GetAll();
            cmbCustomer.DisplayMemberPath = "Name";
            cmbCustomer.SelectedValuePath = "Id";
            _allCustomers = _customerService.GetAll();

            var trucks = new List<object> { new { Id = (int?)null, Name = "بدون شاحنة" } };
            trucks.AddRange(_truckService.GetAll().Select(t => new { Id = (int?)t.Id, Name = t.Name }));
            cmbTruck.ItemsSource = trucks;
            cmbTruck.DisplayMemberPath = "Name";
            cmbTruck.SelectedValuePath = "Id";
            cmbTruck.SelectedIndex = 0;

            var drivers = new List<object> { new { Id = (int?)null, Name = "بدون سائق" } };
            drivers.AddRange(_driverService.GetAll().Select(d => new { Id = (int?)d.Id, Name = d.Name }));
            cmbDriver.ItemsSource = drivers;
            cmbDriver.DisplayMemberPath = "Name";
            cmbDriver.SelectedValuePath = "Id";
            cmbDriver.SelectedIndex = 0;
        }

        private void CmbTruck_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTruck.SelectedValue != null && cmbTruck.SelectedValue is int truckId)
            {
                var truck = _truckService.GetById(truckId);
                if (truck != null && truck.CurrentLoad == 0)
                {
                    MessageBox.Show($"الشاحنة '{truck.Name}' لا تحتوي على أقفاص متاحة (الحمولة الحالية: 0)",
                                  "تحذير", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void SetupDataGrid()
        {
            dgSaleItems.ItemsSource = _saleItems;
        }

        private void DgSaleItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is DataGrid dataGrid)
            {
                var currentCell = dataGrid.CurrentCell;
                var currentRowIndex = dataGrid.Items.IndexOf(currentCell.Item);
                var currentColumnIndex = currentCell.Column.DisplayIndex;

                bool isLastEditableColumn = currentColumnIndex == 2;
                bool isLastRow = currentRowIndex == dataGrid.Items.Count - 1;

                if (isLastEditableColumn && isLastRow && currentCell.Item is SaleItem)
                {
                    var newItem = new SaleItem();
                    if (decimal.TryParse(txtPricePerKg.Text, out decimal price))
                    {
                        newItem.UpdatePrice(price);
                    }

                    if (!string.IsNullOrWhiteSpace(txtSingleCageWeight.Text) &&
                        decimal.TryParse(txtSingleCageWeight.Text, out decimal cageWeight))
                    {
                        newItem.SingleCageWeight = cageWeight;
                    }

                    _saleItems.Add(newItem);

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        dataGrid.SelectedIndex = dataGrid.Items.Count - 1;
                        dataGrid.ScrollIntoView(dataGrid.SelectedItem);

                        var newRow = dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.Items.Count - 1) as DataGridRow;
                        if (newRow != null)
                        {
                            dataGrid.CurrentCell = new DataGridCellInfo(newRow.Item, dataGrid.Columns[0]);
                            dataGrid.BeginEdit();
                        }
                    }), System.Windows.Threading.DispatcherPriority.Background);

                    e.Handled = true;
                }
            }
        }

        private void SaleItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SaleItem item in e.NewItems)
                {
                    item.PropertyChanged += SaleItem_PropertyChanged;
                    if (decimal.TryParse(txtPricePerKg.Text, out decimal price))
                    {
                        item.UpdatePrice(price);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (SaleItem item in e.OldItems)
                {
                    item.PropertyChanged -= SaleItem_PropertyChanged;
                }
            }

            UpdateTotals();
        }

        private void BtnApplyCageWeight_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSingleCageWeight.Text))
            {
                MessageBox.Show("يرجى إدخال وزن القفص الواحد.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtSingleCageWeight.Text, out decimal cageWeight) || cageWeight <= 0)
            {
                MessageBox.Show("يرجى إدخال وزن صالح للقفص الواحد.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_saleItems.Count == 0)
            {
                MessageBox.Show("لا توجد عناصر في الفاتورة لتطبيق وزن القفص عليها.", "لا توجد عناصر", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"هل تريد تطبيق وزن القفص ({cageWeight:F2} كغ) على جميع العناصر الحالية؟",
                                        "تأكيد التطبيق", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                foreach (var item in _saleItems)
                {
                    item.SingleCageWeight = cageWeight;
                }

                MessageBox.Show($"تم تطبيق وزن القفص ({cageWeight:F2} كغ) على جميع العناصر بنجاح!",
                               "تم التطبيق", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaleItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SaleItem.TotalAmount))
            {
                UpdateTotals();
            }
        }

        private void UpdateTotals()
        {
            var totalAmount = _saleItems.Sum(item => item.TotalAmount);
            var lineCount = _saleItems.Count;
            lblInvoiceTotal.Text = $"إجمالي الفاتورة: {totalAmount:C} | عدد البنود: {lineCount}";
        }

        private void CmbCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCustomer.SelectedValue != null)
            {
                var customerId = (int)cmbCustomer.SelectedValue;
                var customer = _customerService.GetById(customerId);
                if (customer != null && customer.Balance > 0)
                {
                    lblCurrentBalance.Text = $"الرصيد الحالي: {customer.Balance:C}";
                }
                else
                {
                    lblCurrentBalance.Text = "";
                }

                txtCustomerSearch.Text = "بحث عن العميل...";
                txtCustomerSearch.Foreground = System.Windows.Media.Brushes.Gray;
                txtCustomerSearch.FontStyle = FontStyles.Italic;
            }
        }

        private void TxtPricePerKg_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(txtPricePerKg.Text, out decimal price))
            {
                foreach (var item in _saleItems)
                {
                    item.UpdatePrice(price);
                }
            }
        }

        private void DgSaleItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (e.Row.Item is SaleItem item)
                {
                    if (decimal.TryParse(txtPricePerKg.Text, out decimal price))
                    {
                        item.UpdatePrice(price);
                    }

                    var currentRowIndex = dgSaleItems.Items.IndexOf(e.Row.Item);
                    var isLastRow = currentRowIndex == dgSaleItems.Items.Count - 1;
                    var isLastEditableColumn = e.Column.DisplayIndex == 2;

                    if (isLastRow && isLastEditableColumn)
                    {
                        var newItem = new SaleItem();
                        if (decimal.TryParse(txtPricePerKg.Text, out decimal priceForNew))
                        {
                            newItem.UpdatePrice(priceForNew);
                        }

                        if (!string.IsNullOrWhiteSpace(txtSingleCageWeight.Text) &&
                            decimal.TryParse(txtSingleCageWeight.Text, out decimal cageWeight))
                        {
                            newItem.SingleCageWeight = cageWeight;
                        }

                        _saleItems.Add(newItem);

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            dgSaleItems.SelectedIndex = dgSaleItems.Items.Count - 1;
                            dgSaleItems.ScrollIntoView(dgSaleItems.SelectedItem);
                            dgSaleItems.CurrentCell = new DataGridCellInfo(dgSaleItems.SelectedItem, dgSaleItems.Columns[0]);
                            dgSaleItems.BeginEdit();
                        }), System.Windows.Threading.DispatcherPriority.Background);
                    }
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void BtnAddRow_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new SaleItem();
            if (decimal.TryParse(txtPricePerKg.Text, out decimal price))
            {
                newItem.UpdatePrice(price);
            }
            _saleItems.Add(newItem);
        }

        private void BtnRemoveRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is SaleItem saleItem)
            {
                _saleItems.Remove(saleItem);
            }
        }

        private void BtnPayNow_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInvoice()) return;

            try
            {
                ProcessInvoice(true);
                MessageBox.Show("تم معالجة الفاتورة بنجاح - مدفوع نقداً!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في معالجة الفاتورة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddToAccount_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInvoice()) return;

            try
            {
                ProcessInvoice(false);
                MessageBox.Show("تم إضافة الفاتورة إلى حساب العميل!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في معالجة الفاتورة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClearAll_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private bool ValidateInvoice()
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار عميل.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            foreach (var item in _saleItems)
            {
                if (item.GrossWeight <= 0)
                {
                    MessageBox.Show("يرجى إدخال وزن إجمالي صالح لجميع العناصر.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (item.NetWeight <= 0)
                {
                    MessageBox.Show("يجب أن يكون الوزن الصافي أكبر من الصفر لجميع العناصر.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private void ProcessInvoice(bool isPaidNow)
        {
            var customerId = (int)cmbCustomer.SelectedValue;

            int? truckId = null;
            if (cmbTruck.SelectedValue is int selectedTruckId)
            {
                truckId = selectedTruckId;
            }

            int? driverId = null;
            if (cmbDriver.SelectedValue is int selectedDriverId)
            {
                driverId = selectedDriverId;
            }

            var pricePerKg = decimal.Parse(txtPricePerKg.Text);

            var totalGrossWeight = _saleItems.Sum(item => item.GrossWeight);
            var totalNumberOfCages = _saleItems.Sum(item => item.NumberOfCages);
            var totalCageWeight = _saleItems.Sum(item => item.TotalCageWeight);
            var totalNetWeight = _saleItems.Sum(item => item.NetWeight);
            var invoiceTotal = _saleItems.Sum(item => item.TotalAmount);

            if (truckId.HasValue)
            {
                var truck = _truckService.GetById(truckId.Value);
                if (truck != null)
                {
                    if (totalNetWeight > truck.NetWeight && truck.NetWeight > 0)
                    {
                        var overage = totalNetWeight - truck.NetWeight;
                        var result = MessageBox.Show(
                            $"تحذير: المبيعة تتجاوز الوزن المتاح في الشاحنة بمقدار {overage:F2} كغ\n" +
                            $"متاح: {truck.NetWeight:F2} كغ | مطلوب: {totalNetWeight:F2} كغ\n\n" +
                            "هل تريد المتابعة؟ سيتم تسجيل انحراف سالب.",
                            "تجاوز في الوزن", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.No)
                            return;
                    }

                    if (totalNumberOfCages > truck.CurrentLoad && truck.CurrentLoad > 0)
                    {
                        var overage = totalNumberOfCages - truck.CurrentLoad;
                        var result = MessageBox.Show(
                            $"تحذير: المبيعة تتجاوز الأقفاص المتاحة بمقدار {overage} قفص\n" +
                            $"متاح: {truck.CurrentLoad} أقفاص | مطلوب: {totalNumberOfCages} أقفاص\n\n" +
                            "هل تريد المتابعة؟",
                            "تجاوز في الأقفاص", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.No)
                            return;
                    }
                }
            }

            var customer = _customerService.GetById(customerId);
            var originalBalance = customer.Balance;
            var newBalance = originalBalance;

            var sale = new Sale
            {
                CustomerId = customerId,
                TruckId = truckId,
                DriverId = driverId,
                GrossWeight = totalGrossWeight,
                NumberOfCages = totalNumberOfCages,
                CageWeight = totalCageWeight,
                NetWeight = totalNetWeight,
                PricePerKg = pricePerKg,
                TotalAmount = invoiceTotal,
                IsPaidNow = isPaidNow,
                SaleDate = DateTime.Now
            };

            _saleService.AddWithItems(sale, _saleItems.ToList());

            if (truckId.HasValue)
            {
                _truckService.UpdateTruckFromSale(truckId.Value, totalNumberOfCages, totalNetWeight);
            }

            if (!isPaidNow)
            {
                newBalance = originalBalance + invoiceTotal;
                _customerService.UpdateBalance(customerId, newBalance);
            }

            PrintInvoiceReceipt(isPaidNow, invoiceTotal, pricePerKg, truckId, driverId, customer, originalBalance, newBalance);
        }

        private async void PrintInvoiceReceipt(bool isPaidNow, decimal invoiceTotal, decimal pricePerKg, int? truckId, int? driverId, Customer customer, decimal originalBalance, decimal newBalance)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true)
                    return;

                Truck truck = null;
                Driver driver = null;

                if (truckId.HasValue)
                    truck = _truckService.GetAll().FirstOrDefault(t => t.Id == truckId.Value);

                if (driverId.HasValue)
                    driver = _driverService.GetAll().FirstOrDefault(d => d.Id == driverId.Value);

                var invoiceId = DateTime.Now.ToString("yyyyMMddHHmmss");

                var flowDocument = CreateInvoiceDocument(
                    printDialog,
                    invoiceId,
                    customer,
                    truck,
                    driver,
                    isPaidNow,
                    invoiceTotal,
                    pricePerKg,
                    originalBalance,
                    newBalance);

                printDialog.PrintDocument(
                    ((IDocumentPaginatorSource)flowDocument).DocumentPaginator,
                    "فاتورة مبيعات الدواجن");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في طباعة الإيصال: {ex.Message}", "خطأ في الطباعة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument CreateInvoiceDocument(
            PrintDialog printDialog,
            string invoiceId,
            Customer customer,
            Truck truck,
            Driver driver,
            bool isPaidNow,
            decimal invoiceTotal,
            decimal pricePerKg,
            decimal originalBalance,
            decimal newBalance)
        {
            var flowDocument = new FlowDocument
            {
                PageWidth = printDialog.PrintableAreaWidth,
                ColumnWidth = printDialog.PrintableAreaWidth,
                FontFamily = new FontFamily("Segoe UI, Arial"),
                FontWeight = FontWeights.Normal,
                PagePadding = new Thickness(15, 10, 15, 10),
                TextAlignment = TextAlignment.Center,
                PageHeight = printDialog.PrintableAreaHeight
            };

            var header = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 5, 0, 10),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2),
                Padding = new Thickness(10),
                Background = Brushes.LightBlue
            };

            header.Inlines.Add(new Run("فاتورة مبيعات الدواجن")
            {
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue
            });

            flowDocument.Blocks.Add(header);

            var customerParagraph = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(8),
                Background = Brushes.LightYellow
            };

            customerParagraph.Inlines.Add(new Run("السادة")
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold
            });
            customerParagraph.Inlines.Add(new Run(" "));
            customerParagraph.Inlines.Add(new Run(customer.Name)
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkGreen
            });

            flowDocument.Blocks.Add(customerParagraph);

            var shortInvoiceId = DateTime.Now.ToString("yyMMddHHmm");

            var metaInfoParagraph = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(8),
                Background = Brushes.AliceBlue,
                FontSize = 12
            };

            metaInfoParagraph.Inlines.Add(new Run($"رقم الفاتورة: {shortInvoiceId}")
            {
                FontWeight = FontWeights.Bold
            });
            metaInfoParagraph.Inlines.Add(new Run("  |  "));
            metaInfoParagraph.Inlines.Add(new Run($"التاريخ: {DateTime.Now:yyyy/MM/dd hh:mm tt}")
            {
                FontWeight = FontWeights.Bold
            });

            if (truck != null || driver != null)
            {
                metaInfoParagraph.Inlines.Add(new LineBreak());
                if (truck != null)
                {
                    metaInfoParagraph.Inlines.Add(new Run($"الشاحنة: {truck.Name}")
                    {
                        FontWeight = FontWeights.Bold
                    });
                }
                if (truck != null && driver != null)
                {
                    metaInfoParagraph.Inlines.Add(new Run("  |  "));
                }
                if (driver != null)
                {
                    metaInfoParagraph.Inlines.Add(new Run($"السائق: {driver.Name}")
                    {
                        FontWeight = FontWeights.Bold
                    });
                }
            }
            else
            {
                metaInfoParagraph.Inlines.Add(new LineBreak());
                metaInfoParagraph.Inlines.Add(new Run("مبيعة مباشرة من المحل")
                {
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.DarkGreen
                });
            }

            flowDocument.Blocks.Add(metaInfoParagraph);
            flowDocument.Blocks.Add(new Paragraph { Margin = new Thickness(0, 10, 0, 10) });

            var summaryTable = new Table
            {
                FontSize = 13,
                CellSpacing = 0,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2)
            };
            summaryTable.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
            summaryTable.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
            summaryTable.RowGroups.Add(new TableRowGroup());

            var summaryHeaderRow = new TableRow();
            summaryHeaderRow.Cells.Add(CreateCellWithBorder("البيان", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkBlue));
            summaryHeaderRow.Cells.Add(CreateCellWithBorder("القيمة", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkBlue));
            summaryTable.RowGroups[0].Rows.Add(summaryHeaderRow);

            var totalGrossWeight = _saleItems.Sum(item => item.GrossWeight);
            var totalCages = _saleItems.Sum(item => item.NumberOfCages);
            var totalNetWeight = _saleItems.Sum(item => item.NetWeight);
            var totalCageWeight = _saleItems.Sum(item => item.TotalCageWeight);

            var grossWeightRow = new TableRow();
            grossWeightRow.Cells.Add(CreateCellWithBorder("الوزن الاساسي", FontWeights.Normal, TextAlignment.Right));
            grossWeightRow.Cells.Add(CreateCellWithBorder($"{totalGrossWeight:F2} كغ", FontWeights.Normal, TextAlignment.Center));
            summaryTable.RowGroups[0].Rows.Add(grossWeightRow);

            if (totalCages > 0)
            {
                var cagesRow = new TableRow();
                cagesRow.Cells.Add(CreateCellWithBorder("عدد الأقفاص", FontWeights.Normal, TextAlignment.Right));
                cagesRow.Cells.Add(CreateCellWithBorder($"{totalCages}", FontWeights.Normal, TextAlignment.Center));
                summaryTable.RowGroups[0].Rows.Add(cagesRow);

                var cageWeightRow = new TableRow();
                cageWeightRow.Cells.Add(CreateCellWithBorder("وزن الأقفاص", FontWeights.Normal, TextAlignment.Right));
                cageWeightRow.Cells.Add(CreateCellWithBorder($"{totalCageWeight:F2} كغ", FontWeights.Normal, TextAlignment.Center));
                summaryTable.RowGroups[0].Rows.Add(cageWeightRow);
            }

            var netWeightRow = new TableRow();
            netWeightRow.Cells.Add(CreateCellWithBorder("الوزن الصافي", FontWeights.Normal, TextAlignment.Right));
            netWeightRow.Cells.Add(CreateCellWithBorder($"{totalNetWeight:F2} كغ", FontWeights.Normal, TextAlignment.Center));
            summaryTable.RowGroups[0].Rows.Add(netWeightRow);

            var priceRow = new TableRow();
            priceRow.Cells.Add(CreateCellWithBorder("سعر الكيلو", FontWeights.Normal, TextAlignment.Right));
            priceRow.Cells.Add(CreateCellWithBorder($"{pricePerKg:C}", FontWeights.Normal, TextAlignment.Center));
            summaryTable.RowGroups[0].Rows.Add(priceRow);

            var totalRow = new TableRow();
            totalRow.Cells.Add(CreateCellWithBorder("إجمالي المبلغ", FontWeights.Bold, TextAlignment.Right, Brushes.DarkGreen, Brushes.LightYellow));
            totalRow.Cells.Add(CreateCellWithBorder($"{invoiceTotal:C}", FontWeights.Bold, TextAlignment.Center, Brushes.DarkGreen, Brushes.LightYellow));
            summaryTable.RowGroups[0].Rows.Add(totalRow);

            flowDocument.Blocks.Add(summaryTable);
            flowDocument.Blocks.Add(new Paragraph { Margin = new Thickness(0, 15, 0, 10) });

            var paymentStatusParagraph = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10),
                Background = isPaidNow ? Brushes.LightGreen : Brushes.LightCoral
            };

            paymentStatusParagraph.Inlines.Add(new Run("حالة الدفع:")
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold
            });
            paymentStatusParagraph.Inlines.Add(new Run(" "));
            paymentStatusParagraph.Inlines.Add(new Run(isPaidNow ? "مدفوع نقداً" : "مضاف للحساب")
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = isPaidNow ? Brushes.DarkGreen : Brushes.DarkRed
            });

            flowDocument.Blocks.Add(paymentStatusParagraph);

            if (!isPaidNow)
            {
                var balanceTable = new Table
                {
                    FontSize = 13,
                    CellSpacing = 0,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(2),
                    Margin = new Thickness(0, 10, 0, 0)
                };
                balanceTable.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
                balanceTable.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
                balanceTable.RowGroups.Add(new TableRowGroup());

                var balanceHeaderRow = new TableRow();
                balanceHeaderRow.Cells.Add(CreateCellWithBorder("حساب العميل", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkRed, 2));
                balanceTable.RowGroups[0].Rows.Add(balanceHeaderRow);

                var prevBalanceRow = new TableRow();
                prevBalanceRow.Cells.Add(CreateCellWithBorder("الرصيد السابق", FontWeights.Normal, TextAlignment.Right));
                prevBalanceRow.Cells.Add(CreateCellWithBorder($"{originalBalance:C}", FontWeights.Normal, TextAlignment.Center));
                balanceTable.RowGroups[0].Rows.Add(prevBalanceRow);

                var newBalanceRow = new TableRow();
                newBalanceRow.Cells.Add(CreateCellWithBorder("الرصيد الجديد", FontWeights.Bold, TextAlignment.Right, Brushes.DarkRed));
                newBalanceRow.Cells.Add(CreateCellWithBorder($"{newBalance:C}", FontWeights.Bold, TextAlignment.Center, Brushes.DarkRed));
                balanceTable.RowGroups[0].Rows.Add(newBalanceRow);

                flowDocument.Blocks.Add(balanceTable);
            }

            var footer = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 5),
                BorderBrush = Brushes.DarkBlue,
                BorderThickness = new Thickness(2),
                Padding = new Thickness(10),
                Background = Brushes.LightCyan
            };

            footer.Inlines.Add(new Run("شكراً لتعاملكم معنا")
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue
            });
            footer.Inlines.Add(new LineBreak());
            footer.Inlines.Add(new Run("Thank you for your business!")
            {
                FontSize = 14,
                FontWeight = FontWeights.Normal,
                Foreground = Brushes.DarkBlue
            });

            flowDocument.Blocks.Add(footer);

            return flowDocument;
        }

        private TableCell CreateCellWithBorder(string text, FontWeight fontWeight = default, TextAlignment alignment = TextAlignment.Left, Brush foreground = null, Brush background = null, int columnSpan = 1)
        {
            var paragraph = new Paragraph(new Run(text ?? string.Empty))
            {
                FontWeight = fontWeight == default ? FontWeights.Normal : fontWeight,
                TextAlignment = alignment,
                Margin = new Thickness(5),
                Foreground = foreground ?? Brushes.Black
            };

            var cell = new TableCell(paragraph)
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(5),
                Background = background ?? Brushes.White,
                ColumnSpan = columnSpan
            };

            return cell;
        }

        private void ClearAll()
        {
            _saleItems.Clear();
            cmbCustomer.SelectedIndex = -1;
            cmbTruck.SelectedIndex = 0;
            cmbDriver.SelectedIndex = 0;
            txtPricePerKg.Clear();
            lblCurrentBalance.Text = "";

            txtCustomerSearch.Text = "بحث عن العميل...";
            txtCustomerSearch.Foreground = System.Windows.Media.Brushes.Gray;
            txtCustomerSearch.FontStyle = FontStyles.Italic;
            cmbCustomer.ItemsSource = _allCustomers;
        }

        private void SetupCustomerSearch()
        {
            txtCustomerSearch.Text = "بحث عن العميل...";
            txtCustomerSearch.Foreground = System.Windows.Media.Brushes.Gray;
            txtCustomerSearch.FontStyle = FontStyles.Italic;
        }

        private void TxtCustomerSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_allCustomers == null || txtCustomerSearch.Foreground == System.Windows.Media.Brushes.Gray)
                return;

            var searchText = txtCustomerSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                cmbCustomer.ItemsSource = _allCustomers;
            }
            else
            {
                var filteredCustomers = _allCustomers.Where(c =>
                    c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(c.Phone) && c.Phone.Contains(searchText))
                ).ToList();

                cmbCustomer.ItemsSource = filteredCustomers;
            }

            if (cmbCustomer.Items.Count > 0)
            {
                cmbCustomer.IsDropDownOpen = true;
            }
        }

        private void TxtCustomerSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtCustomerSearch.Text == "بحث عن العميل...")
            {
                txtCustomerSearch.Text = "";
                txtCustomerSearch.Foreground = System.Windows.Media.Brushes.Black;
                txtCustomerSearch.FontStyle = FontStyles.Normal;
            }
        }

        private void TxtCustomerSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerSearch.Text))
            {
                txtCustomerSearch.Text = "بحث عن العميل...";
                txtCustomerSearch.Foreground = System.Windows.Media.Brushes.Gray;
                txtCustomerSearch.FontStyle = FontStyles.Italic;
                if (_allCustomers != null)
                {
                    cmbCustomer.ItemsSource = _allCustomers;
                }
            }
        }

        private void CmbCustomer_DropDownOpened(object sender, EventArgs e)
        {
            if (_allCustomers != null &&
                (string.IsNullOrWhiteSpace(txtCustomerSearch.Text) || txtCustomerSearch.Text == "بحث عن العميل..."))
            {
                cmbCustomer.ItemsSource = _allCustomers;
            }
        }
    }
}