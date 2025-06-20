using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Printing;

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

        private int _originalCustomerId;
        private decimal _originalTotalAmount;

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

            var trucks = new List<object> { new { Id = (int?)null, Name = "بدون شاحنة" } };
            trucks.AddRange(_truckService.GetAll().Select(t => new { Id = (int?)t.Id, Name = t.Name }));
            cmbTruck.ItemsSource = trucks;
            cmbTruck.DisplayMemberPath = "Name";
            cmbTruck.SelectedValuePath = "Id";

            var drivers = new List<object> { new { Id = (int?)null, Name = "بدون سائق" } };
            drivers.AddRange(_driverService.GetAll().Select(d => new { Id = (int?)d.Id, Name = d.Name }));
            cmbDriver.ItemsSource = drivers;
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

            _originalCustomerId = _currentSale.CustomerId;
            _originalTotalAmount = _currentSale.TotalAmount;

            lblTitle.Text = $"تفاصيل المبيعة - فاتورة رقم {_currentSale.Id}";

            cmbCustomer.SelectedValue = _currentSale.CustomerId;

            if (_currentSale.TruckId.HasValue && _currentSale.TruckId.Value > 0)
                cmbTruck.SelectedValue = _currentSale.TruckId.Value;
            else
                cmbTruck.SelectedIndex = 0;

            if (_currentSale.DriverId.HasValue && _currentSale.DriverId.Value > 0)
                cmbDriver.SelectedValue = _currentSale.DriverId.Value;
            else
                cmbDriver.SelectedIndex = 0;

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

        private void BtnReprint_Click(object sender, RoutedEventArgs e)
        {
            if (_transactionType == "Sale")
            {
                ReprintSaleTransaction();
            }
            else if (_transactionType == "Payment")
            {
                ReprintPaymentTransaction();
            }
        }

        private void ReprintSaleTransaction()
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true)
                    return;

                var customer = _customerService.GetById(_currentSale.CustomerId);

                Truck truck = null;
                if (_currentSale.TruckId.HasValue)
                    truck = _truckService.GetAll().FirstOrDefault(t => t.Id == _currentSale.TruckId.Value);

                Driver driver = null;
                if (_currentSale.DriverId.HasValue)
                    driver = _driverService.GetAll().FirstOrDefault(d => d.Id == _currentSale.DriverId.Value);

                var currentBalance = customer.Balance;
                var originalBalance = _currentSale.IsPaidNow ? currentBalance : currentBalance - _currentSale.TotalAmount;

                var invoiceId = _currentSale.SaleDate.ToString("yyyyMMddHHmmss");

                var flowDocument = CreateSaleReceiptDocument(
                    printDialog,
                    invoiceId,
                    customer,
                    truck,
                    driver,
                    _currentSale.IsPaidNow,
                    _currentSale.TotalAmount,
                    _currentSale.PricePerKg,
                    originalBalance,
                    currentBalance);

                printDialog.PrintDocument(
                    ((IDocumentPaginatorSource)flowDocument).DocumentPaginator,
                    "فاتورة مبيعات الدواجن - إعادة طباعة");

                MessageBox.Show("تم إعادة طباعة الفاتورة بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إعادة طباعة الفاتورة: {ex.Message}", "خطأ في الطباعة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReprintPaymentTransaction()
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true)
                    return;

                var customer = _customerService.GetById(_currentPayment.CustomerId);
                var receiptId = _currentPayment.PaymentDate.ToString("yyMMddHHmm");

                var currentBalance = customer.Balance;
                var originalBalance = currentBalance + _currentPayment.Amount;

                var flowDocument = CreatePaymentReceiptDocument(
                    printDialog,
                    receiptId,
                    _currentPayment,
                    customer,
                    originalBalance,
                    currentBalance);

                printDialog.PrintDocument(
                    ((IDocumentPaginatorSource)flowDocument).DocumentPaginator,
                    "إيصال استلام دفعة - إعادة طباعة");

                MessageBox.Show("تم إعادة طباعة الإيصال بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إعادة طباعة الإيصال: {ex.Message}", "خطأ في الطباعة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument CreateSaleReceiptDocument(
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

            header.Inlines.Add(new Run("فاتورة مبيعات الدواجن - إعادة طباعة")
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

            var shortInvoiceId = _currentSale.SaleDate.ToString("yyMMddHHmm");

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
            metaInfoParagraph.Inlines.Add(new Run($"التاريخ الأصلي: {_currentSale.SaleDate:yyyy/MM/dd hh:mm tt}")
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

            var grossWeightRow = new TableRow();
            grossWeightRow.Cells.Add(CreateCellWithBorder("الوزن الاساسي", FontWeights.Normal, TextAlignment.Right));
            grossWeightRow.Cells.Add(CreateCellWithBorder($"{_currentSale.GrossWeight:F2} كغ", FontWeights.Normal, TextAlignment.Center));
            summaryTable.RowGroups[0].Rows.Add(grossWeightRow);

            var cagesRow = new TableRow();
            cagesRow.Cells.Add(CreateCellWithBorder("عدد الأقفاص", FontWeights.Normal, TextAlignment.Right));
            cagesRow.Cells.Add(CreateCellWithBorder($"{_currentSale.NumberOfCages}", FontWeights.Normal, TextAlignment.Center));
            summaryTable.RowGroups[0].Rows.Add(cagesRow);

            var cageWeightRow = new TableRow();
            cageWeightRow.Cells.Add(CreateCellWithBorder("وزن الأقفاص", FontWeights.Normal, TextAlignment.Right));
            cageWeightRow.Cells.Add(CreateCellWithBorder($"{_currentSale.CageWeight:F2} كغ", FontWeights.Normal, TextAlignment.Center));
            summaryTable.RowGroups[0].Rows.Add(cageWeightRow);

            var netWeightRow = new TableRow();
            netWeightRow.Cells.Add(CreateCellWithBorder("الوزن الصافي", FontWeights.Normal, TextAlignment.Right));
            netWeightRow.Cells.Add(CreateCellWithBorder($"{_currentSale.NetWeight:F2} كغ", FontWeights.Normal, TextAlignment.Center));
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
                balanceHeaderRow.Cells.Add(CreateCellWithBorder("حساب العميل وقت المعاملة", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkRed, 2));
                balanceTable.RowGroups[0].Rows.Add(balanceHeaderRow);

                var prevBalanceRow = new TableRow();
                prevBalanceRow.Cells.Add(CreateCellWithBorder("الرصيد السابق", FontWeights.Normal, TextAlignment.Right));
                prevBalanceRow.Cells.Add(CreateCellWithBorder($"{originalBalance:C}", FontWeights.Normal, TextAlignment.Center));
                balanceTable.RowGroups[0].Rows.Add(prevBalanceRow);

                var newBalanceRow = new TableRow();
                newBalanceRow.Cells.Add(CreateCellWithBorder("الرصيد بعد المعاملة", FontWeights.Bold, TextAlignment.Right, Brushes.DarkRed));
                newBalanceRow.Cells.Add(CreateCellWithBorder($"{originalBalance + _currentSale.TotalAmount:C}", FontWeights.Bold, TextAlignment.Center, Brushes.DarkRed));
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

            footer.Inlines.Add(new Run("إعادة طباعة - شكراً لتعاملكم معنا")
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue
            });

            flowDocument.Blocks.Add(footer);

            return flowDocument;
        }

        private FlowDocument CreatePaymentReceiptDocument(
            PrintDialog printDialog,
            string receiptId,
            Payment payment,
            Customer customer,
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
                Background = Brushes.LightGreen
            };

            header.Inlines.Add(new Run("إيصال استلام دفعة - إعادة طباعة")
            {
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkGreen
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

            customerParagraph.Inlines.Add(new Run("العميل:")
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold
            });
            customerParagraph.Inlines.Add(new Run(" "));
            customerParagraph.Inlines.Add(new Run(customer.Name)
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue
            });

            flowDocument.Blocks.Add(customerParagraph);

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

            metaInfoParagraph.Inlines.Add(new Run($"رقم الإيصال: {receiptId}")
            {
                FontWeight = FontWeights.Bold
            });
            metaInfoParagraph.Inlines.Add(new Run("  |  "));
            metaInfoParagraph.Inlines.Add(new Run($"التاريخ الأصلي: {payment.PaymentDate:yyyy/MM/dd hh:mm tt}")
            {
                FontWeight = FontWeights.Bold
            });

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
            summaryHeaderRow.Cells.Add(CreateCellWithBorder("البيان", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkGreen));
            summaryHeaderRow.Cells.Add(CreateCellWithBorder("القيمة", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkGreen));
            summaryTable.RowGroups[0].Rows.Add(summaryHeaderRow);

            var previousBalanceRow = new TableRow();
            previousBalanceRow.Cells.Add(CreateCellWithBorder("الرصيد قبل الدفعة", FontWeights.Normal, TextAlignment.Right));
            previousBalanceRow.Cells.Add(CreateCellWithBorder($"{originalBalance:C}", FontWeights.Normal, TextAlignment.Center));
            summaryTable.RowGroups[0].Rows.Add(previousBalanceRow);

            var paymentAmountRow = new TableRow();
            paymentAmountRow.Cells.Add(CreateCellWithBorder("مبلغ الدفعة", FontWeights.Normal, TextAlignment.Right));
            paymentAmountRow.Cells.Add(CreateCellWithBorder($"{payment.Amount:C}", FontWeights.Normal, TextAlignment.Center, Brushes.DarkGreen));
            summaryTable.RowGroups[0].Rows.Add(paymentAmountRow);

            var newBalanceRow = new TableRow { Background = Brushes.LightYellow };
            newBalanceRow.Cells.Add(CreateCellWithBorder("الرصيد بعد الدفعة", FontWeights.Bold, TextAlignment.Right, Brushes.DarkRed));
            newBalanceRow.Cells.Add(CreateCellWithBorder($"{originalBalance - payment.Amount:C}", FontWeights.Bold, TextAlignment.Center, Brushes.DarkRed));
            summaryTable.RowGroups[0].Rows.Add(newBalanceRow);

            flowDocument.Blocks.Add(summaryTable);

            if (!string.IsNullOrEmpty(payment.Notes))
            {
                var notesParagraph = new Paragraph
                {
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 15, 0, 10),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(10),
                    Background = Brushes.LightCyan
                };

                notesParagraph.Inlines.Add(new Run("ملاحظات:")
                {
                    FontSize = 14,
                    FontWeight = FontWeights.Bold
                });
                notesParagraph.Inlines.Add(new LineBreak());
                notesParagraph.Inlines.Add(new Run(payment.Notes)
                {
                    FontSize = 12,
                    FontStyle = FontStyles.Italic
                });

                flowDocument.Blocks.Add(notesParagraph);
            }

            var footer = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 5),
                BorderBrush = Brushes.DarkGreen,
                BorderThickness = new Thickness(2),
                Padding = new Thickness(10),
                Background = Brushes.LightGreen
            };

            footer.Inlines.Add(new Run("إعادة طباعة - شكراً لسداد دفعتكم")
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkGreen
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

                var newCustomerId = (int)cmbCustomer.SelectedValue;
                var newTotalAmount = _saleItems.Sum(item => item.TotalAmount);

                int? newTruckId = null;
                if (cmbTruck.SelectedValue is int selectedTruckId)
                {
                    newTruckId = selectedTruckId;
                }

                int? newDriverId = null;
                if (cmbDriver.SelectedValue is int selectedDriverId)
                {
                    newDriverId = selectedDriverId;
                }

                var originalTruckId = _currentSale.TruckId;
                var totalNumberOfCages = _saleItems.Sum(item => item.NumberOfCages);
                var totalNetWeight = _saleItems.Sum(item => item.NetWeight);

                if (!_currentSale.IsPaidNow)
                {
                    if (_originalCustomerId != newCustomerId || _originalTotalAmount != newTotalAmount)
                    {
                        var originalCustomer = _customerService.GetById(_originalCustomerId);
                        if (originalCustomer != null)
                        {
                            var updatedOriginalBalance = originalCustomer.Balance - _originalTotalAmount;
                            _customerService.UpdateBalance(_originalCustomerId, updatedOriginalBalance);
                        }

                        var newCustomer = _customerService.GetById(newCustomerId);
                        if (newCustomer != null)
                        {
                            var updatedNewBalance = newCustomer.Balance + newTotalAmount;
                            _customerService.UpdateBalance(newCustomerId, updatedNewBalance);
                        }
                    }
                }

                _currentSale.CustomerId = newCustomerId;
                _currentSale.TruckId = newTruckId;
                _currentSale.DriverId = newDriverId;
                _currentSale.PricePerKg = pricePerKg;

                _currentSale.GrossWeight = _saleItems.Sum(item => item.GrossWeight);
                _currentSale.NumberOfCages = _saleItems.Sum(item => item.NumberOfCages);
                _currentSale.CageWeight = _saleItems.Sum(item => item.TotalCageWeight);
                _currentSale.NetWeight = _saleItems.Sum(item => item.NetWeight);
                _currentSale.TotalAmount = newTotalAmount;

                _saleService.UpdateWithItems(_currentSale, _saleItems.ToList());

                if (originalTruckId != newTruckId)
                {
                    if (originalTruckId.HasValue)
                    {
                        var originalTruck = _truckService.GetById(originalTruckId.Value);
                        if (originalTruck != null)
                        {
                            var restoredCages = originalTruck.CurrentLoad + totalNumberOfCages;
                            var restoredWeight = originalTruck.NetWeight + totalNetWeight;

                            var updatedOriginalTruck = new Truck
                            {
                                Id = originalTruck.Id,
                                Name = originalTruck.Name,
                                CurrentLoad = restoredCages,
                                NetWeight = restoredWeight,
                                PlateNumber = originalTruck.PlateNumber,
                                IsActive = originalTruck.IsActive
                            };
                            _truckService.Update(updatedOriginalTruck);
                        }
                    }

                    if (newTruckId.HasValue)
                    {
                        var newTruck = _truckService.GetById(newTruckId.Value);
                        if (newTruck != null)
                        {
                            if (totalNetWeight > newTruck.NetWeight && newTruck.NetWeight > 0)
                            {
                                var overage = totalNetWeight - newTruck.NetWeight;
                                var result = MessageBox.Show(
                                    $"تحذير: المبيعة تتجاوز الوزن المتاح في الشاحنة بمقدار {overage:F2} كغ\n" +
                                    $"متاح: {newTruck.NetWeight:F2} كغ | مطلوب: {totalNetWeight:F2} كغ\n\n" +
                                    "هل تريد المتابعة؟ سيتم تسجيل انحراف سالب.",
                                    "تجاوز في الوزن", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                                if (result == MessageBoxResult.No)
                                    return;
                            }

                            if (totalNumberOfCages > newTruck.CurrentLoad && newTruck.CurrentLoad > 0)
                            {
                                var overage = totalNumberOfCages - newTruck.CurrentLoad;
                                var result = MessageBox.Show(
                                    $"تحذير: المبيعة تتجاوز الأقفاص المتاحة بمقدار {overage} قفص\n" +
                                    $"متاح: {newTruck.CurrentLoad} أقفاص | مطلوب: {totalNumberOfCages} أقفاص\n\n" +
                                    "هل تريد المتابعة؟",
                                    "تجاوز في الأقفاص", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                                if (result == MessageBoxResult.No)
                                    return;
                            }

                            _truckService.UpdateTruckFromSale(newTruckId.Value, totalNumberOfCages, totalNetWeight);
                        }
                    }
                }

                _originalCustomerId = newCustomerId;
                _originalTotalAmount = newTotalAmount;

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