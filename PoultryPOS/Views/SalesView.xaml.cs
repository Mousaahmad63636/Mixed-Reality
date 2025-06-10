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
    public partial class SalesView : Page
    {
        private readonly TruckService _truckService;
        private readonly DriverService _driverService;
        private readonly CustomerService _customerService;
        private readonly SaleService _saleService;
        private ObservableCollection<SaleItem> _saleItems;

        public SalesView()
        {
            InitializeComponent();
            _truckService = new TruckService();
            _driverService = new DriverService();
            _customerService = new CustomerService();
            _saleService = new SaleService();
            _saleItems = new ObservableCollection<SaleItem>();

            LoadData();
            SetupDataGrid();
            _saleItems.CollectionChanged += SaleItems_CollectionChanged;
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

        private void SetupDataGrid()
        {
            dgSaleItems.ItemsSource = _saleItems;
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
            lblInvoiceTotal.Text = $"Invoice Total: {totalAmount:C} | Line Items: {lineCount}";
        }

        private void CmbCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbCustomer.SelectedValue != null)
            {
                var customerId = (int)cmbCustomer.SelectedValue;
                var customer = _customerService.GetById(customerId);
                if (customer != null && customer.Balance > 0)
                {
                    lblCurrentBalance.Text = $"Current Balance: {customer.Balance:C}";
                }
                else
                {
                    lblCurrentBalance.Text = "";
                }
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
                MessageBox.Show("Invoice processed successfully - Paid Now!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddToAccount_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInvoice()) return;

            try
            {
                ProcessInvoice(false);
                MessageBox.Show("Invoice added to customer account!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

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

            if (!decimal.TryParse(txtPricePerKg.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price per kg.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (_saleItems.Count == 0)
            {
                MessageBox.Show("Please add at least one sale item.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            foreach (var item in _saleItems)
            {
                if (item.GrossWeight <= 0)
                {
                    MessageBox.Show("Please enter valid gross weight for all items.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (item.NumberOfCages <= 0)
                {
                    MessageBox.Show("Please enter valid number of cages for all items.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (item.SingleCageWeight <= 0)
                {
                    MessageBox.Show("Please enter valid single cage weight for all items.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (item.NetWeight <= 0)
                {
                    MessageBox.Show("Net weight must be greater than zero for all items.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }

        private void ProcessInvoice(bool isPaidNow)
        {
            var customerId = (int)cmbCustomer.SelectedValue;
            var truckId = (int)cmbTruck.SelectedValue;
            var driverId = (int)cmbDriver.SelectedValue;
            var pricePerKg = decimal.Parse(txtPricePerKg.Text);

            var totalGrossWeight = _saleItems.Sum(item => item.GrossWeight);
            var totalNumberOfCages = _saleItems.Sum(item => item.NumberOfCages);
            var totalCageWeight = _saleItems.Sum(item => item.TotalCageWeight);
            var totalNetWeight = _saleItems.Sum(item => item.NetWeight);
            var invoiceTotal = _saleItems.Sum(item => item.TotalAmount);

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

            _saleService.Add(sale);

            _truckService.UpdateCurrentLoad(truckId, totalNumberOfCages);

            if (!isPaidNow)
            {
                var customer = _customerService.GetById(customerId);
                var newBalance = customer.Balance + invoiceTotal;
                _customerService.UpdateBalance(customerId, newBalance);
            }

            PrintInvoiceReceipt(isPaidNow, invoiceTotal, pricePerKg);
        }
        private async void PrintInvoiceReceipt(bool isPaidNow, decimal invoiceTotal, decimal pricePerKg)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true)
                    return;

                var customer = _customerService.GetById((int)cmbCustomer.SelectedValue);
                var truck = _truckService.GetAll().First(t => t.Id == (int)cmbTruck.SelectedValue);
                var driver = _driverService.GetAll().First(d => d.Id == (int)cmbDriver.SelectedValue);

                var invoiceId = DateTime.Now.ToString("yyyyMMddHHmmss");

                var flowDocument = CreateInvoiceDocument(
                    printDialog,
                    invoiceId,
                    customer,
                    truck,
                    driver,
                    isPaidNow,
                    invoiceTotal,
                    pricePerKg);

                printDialog.PrintDocument(
                    ((IDocumentPaginatorSource)flowDocument).DocumentPaginator,
                    "Poultry Sales Invoice");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
       decimal pricePerKg)
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
            metaInfoParagraph.Inlines.Add(new LineBreak());
            metaInfoParagraph.Inlines.Add(new Run($"الشاحنة: {truck.Name}")
            {
                FontWeight = FontWeights.Bold
            });
            metaInfoParagraph.Inlines.Add(new Run("  |  "));
            metaInfoParagraph.Inlines.Add(new Run($"السائق: {driver.Name}")
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

            var cagesRow = new TableRow();
            cagesRow.Cells.Add(CreateCellWithBorder("عدد الأقفاص", FontWeights.Normal, TextAlignment.Right));
            cagesRow.Cells.Add(CreateCellWithBorder($"{totalCages}", FontWeights.Normal, TextAlignment.Center));
            summaryTable.RowGroups[0].Rows.Add(cagesRow);

            var cageWeightRow = new TableRow();
            cageWeightRow.Cells.Add(CreateCellWithBorder("وزن الأقفاص", FontWeights.Normal, TextAlignment.Right));
            cageWeightRow.Cells.Add(CreateCellWithBorder($"{totalCageWeight:F2} كغ", FontWeights.Normal, TextAlignment.Center));
            summaryTable.RowGroups[0].Rows.Add(cageWeightRow);

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

                var previousBalance = customer.Balance;
                var newBalance = previousBalance + invoiceTotal;

                var balanceHeaderRow = new TableRow();
                balanceHeaderRow.Cells.Add(CreateCellWithBorder("حساب العميل", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkRed, 2));
                balanceTable.RowGroups[0].Rows.Add(balanceHeaderRow);

                var prevBalanceRow = new TableRow();
                prevBalanceRow.Cells.Add(CreateCellWithBorder("الرصيد السابق", FontWeights.Normal, TextAlignment.Right));
                prevBalanceRow.Cells.Add(CreateCellWithBorder($"{previousBalance:C}", FontWeights.Normal, TextAlignment.Center));
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

        private void AddMetaRowWithBorder(Table table, string label, string value)
        {
            if (table == null) return;

            var row = new TableRow();
            row.Cells.Add(CreateCellWithBorder(label, FontWeights.Bold, TextAlignment.Right, Brushes.DarkBlue, Brushes.LightGray));
            row.Cells.Add(CreateCellWithBorder(value ?? string.Empty, FontWeights.Normal, TextAlignment.Left));
            table.RowGroups[0].Rows.Add(row);
        }
        private TableCell CreateCell(string text, FontWeight fontWeight = default, TextAlignment alignment = TextAlignment.Left)
        {
            var paragraph = new Paragraph(new Run(text ?? string.Empty))
            {
                FontWeight = fontWeight == default ? FontWeights.Normal : fontWeight,
                TextAlignment = alignment,
                Margin = new Thickness(2)
            };
            return new TableCell(paragraph);
        }

        private void AddMetaRow(Table table, string label, string value)
        {
            if (table == null) return;

            var row = new TableRow();
            row.Cells.Add(CreateCell(label, FontWeights.Bold));
            row.Cells.Add(CreateCell(value ?? string.Empty, FontWeights.Normal));
            table.RowGroups[0].Rows.Add(row);
        }

        private void AddTotalRow(Table table, string label, string value)
        {
            if (table == null) return;

            var row = new TableRow();
            row.Cells.Add(CreateCell(label, FontWeights.Normal, TextAlignment.Left));
            row.Cells.Add(CreateCell(value, FontWeights.Normal, TextAlignment.Right));
            table.RowGroups[0].Rows.Add(row);
        }

        private BlockUIContainer CreateDivider()
        {
            return new BlockUIContainer(new Border
            {
                Height = 1,
                Background = Brushes.Black,
                Margin = new Thickness(0, 2, 0, 2)
            });
        }

        private void ClearAll()
        {
            _saleItems.Clear();
            cmbCustomer.SelectedIndex = -1;
            cmbTruck.SelectedIndex = -1;
            cmbDriver.SelectedIndex = -1;
            txtPricePerKg.Clear();
            lblCurrentBalance.Text = "";
        }
    }
}