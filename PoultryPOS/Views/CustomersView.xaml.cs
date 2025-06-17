using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Printing;

namespace PoultryPOS.Views
{
    public partial class CustomersView : Page
    {
        private readonly CustomerService _customerService;
        private readonly PaymentService _paymentService;
        private readonly SaleService _saleService;
        private Customer _selectedCustomer;
        private List<Customer> _allCustomers;
        public CustomersView()
        {
            InitializeComponent();
            _customerService = new CustomerService();
            _paymentService = new PaymentService();
            _saleService = new SaleService();
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
            _allCustomers = _customerService.GetAll();
            dgCustomers.ItemsSource = _allCustomers;
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

            lblTotalCustomers.Text = $"إجمالي العملاء: {customers.Count}";
            lblCustomersWithBalance.Text = $"لديهم رصيد: {customersWithBalance.Count}";
            lblTotalBalance.Text = $"إجمالي الرصيد: {totalBalance:C}";
        }
        private void TxtSearchCustomers_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterCustomers();
        }

        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearchCustomers.Clear();
            FilterCustomers();
        }

        private void FilterCustomers()
        {
            if (_allCustomers == null) return;

            var searchText = txtSearchCustomers.Text?.Trim().ToLower() ?? "";

            if (string.IsNullOrEmpty(searchText))
            {
                dgCustomers.ItemsSource = _allCustomers;
            }
            else
            {
                var filteredCustomers = _allCustomers.Where(c =>
                    c.Name.ToLower().Contains(searchText) ||
                    (c.Phone != null && c.Phone.Contains(searchText)) ||
                    c.Id.ToString().Contains(searchText)).ToList();

                dgCustomers.ItemsSource = filteredCustomers;
            }
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
                    lblCustomerBalance.Text = $"الرصيد الحالي: {customer.Balance:C}";
                }
            }
        }

        private void BtnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم العميل.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtInitialBalance.Text, out decimal initialBalance) || initialBalance < 0)
            {
                MessageBox.Show("يرجى إدخال رصيد أولي صالح.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("تم إضافة العميل بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إضافة العميل: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnUpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer == null)
            {
                MessageBox.Show("يرجى اختيار عميل للتحديث.", "خطأ في التحديد", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم العميل.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtInitialBalance.Text, out decimal balance) || balance < 0)
            {
                MessageBox.Show("يرجى إدخال رصيد صالح.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("تم تحديث العميل بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحديث العميل: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer == null)
            {
                MessageBox.Show("يرجى اختيار عميل للحذف.", "خطأ في التحديد", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"هل أنت متأكد من حذف العميل '{_selectedCustomer.Name}'؟",
                                       "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _customerService.Delete(_selectedCustomer.Id);
                    LoadData();
                    UpdateSummary();
                    ClearCustomerForm();
                    MessageBox.Show("تم حذف العميل بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ في حذف العميل: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void BtnPrintStatement_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer == null)
            {
                MessageBox.Show("يرجى اختيار عميل لطباعة كشف الحساب.", "مطلوب تحديد",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PrintCustomerStatement(_selectedCustomer);
        }

        private async void PrintCustomerStatement(Customer customer)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true)
                    return;

                var statementId = DateTime.Now.ToString("yyMMddHHmm");

                var flowDocument = CreateCustomerStatementDocument(
                    printDialog,
                    statementId,
                    customer);

                printDialog.PrintDocument(
                    ((IDocumentPaginatorSource)flowDocument).DocumentPaginator,
                    "كشف حساب العميل");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في طباعة الكشف: {ex.Message}", "خطأ في الطباعة",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument CreateCustomerStatementDocument(
            PrintDialog printDialog,
            string statementId,
            Customer customer)
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

            header.Inlines.Add(new Run("كشف حساب العميل")
            {
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue
            });
            header.Inlines.Add(new LineBreak());
            header.Inlines.Add(new Run("CUSTOMER ACCOUNT STATEMENT")
            {
                FontSize = 18,
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
                Foreground = Brushes.DarkGreen
            });

            if (!string.IsNullOrEmpty(customer.Phone))
            {
                customerParagraph.Inlines.Add(new Run("  |  "));
                customerParagraph.Inlines.Add(new Run($"هاتف: {customer.Phone}")
                {
                    FontSize = 14,
                    FontWeight = FontWeights.Normal
                });
            }

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

            metaInfoParagraph.Inlines.Add(new Run($"رقم الكشف: {statementId}")
            {
                FontWeight = FontWeights.Bold
            });
            metaInfoParagraph.Inlines.Add(new Run("  |  "));
            metaInfoParagraph.Inlines.Add(new Run($"تاريخ الطباعة: {DateTime.Now:yyyy/MM/dd hh:mm tt}")
            {
                FontWeight = FontWeights.Bold
            });

            flowDocument.Blocks.Add(metaInfoParagraph);
            flowDocument.Blocks.Add(new Paragraph { Margin = new Thickness(0, 10, 0, 10) });

            var sales = _saleService.GetAll().Where(s => s.CustomerId == customer.Id).ToList();
            var payments = _paymentService.GetByCustomer(customer.Id);

            var allTransactions = new List<CustomerTransaction>();

            foreach (var sale in sales)
            {
                allTransactions.Add(new CustomerTransaction
                {
                    Date = sale.SaleDate,
                    Type = "Sale",
                    Description = $"مبيعات - وزن صافي: {sale.NetWeight:F2} كغ",
                    Debit = sale.TotalAmount,
                    Credit = 0,
                    Reference = $"INV-{sale.Id}"
                });
            }

            foreach (var payment in payments)
            {
                allTransactions.Add(new CustomerTransaction
                {
                    Date = payment.PaymentDate,
                    Type = "Payment",
                    Description = $"دفعة{(!string.IsNullOrEmpty(payment.Notes) ? $" - {payment.Notes}" : "")}",
                    Debit = 0,
                    Credit = payment.Amount,
                    Reference = $"PAY-{payment.Id}"
                });
            }

            allTransactions = allTransactions.OrderBy(t => t.Date).ToList();

            if (allTransactions.Any())
            {
                var transactionsTable = new Table
                {
                    FontSize = 9,
                    CellSpacing = 0,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1)
                };

                transactionsTable.Columns.Add(new TableColumn { Width = new GridLength(60) });
                transactionsTable.Columns.Add(new TableColumn { Width = new GridLength(50) });
                transactionsTable.Columns.Add(new TableColumn { Width = new GridLength(2, GridUnitType.Star) });
                transactionsTable.Columns.Add(new TableColumn { Width = new GridLength(60) });
                transactionsTable.Columns.Add(new TableColumn { Width = new GridLength(60) });
                transactionsTable.Columns.Add(new TableColumn { Width = new GridLength(60) });
                transactionsTable.RowGroups.Add(new TableRowGroup());

                var headerRow = new TableRow();
                headerRow.Cells.Add(CreateCellWithBorder("التاريخ", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkBlue));
                headerRow.Cells.Add(CreateCellWithBorder("المرجع", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkBlue));
                headerRow.Cells.Add(CreateCellWithBorder("الوصف", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkBlue));
                headerRow.Cells.Add(CreateCellWithBorder("مدين", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkBlue));
                headerRow.Cells.Add(CreateCellWithBorder("دائن", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkBlue));
                headerRow.Cells.Add(CreateCellWithBorder("الرصيد", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkBlue));
                transactionsTable.RowGroups[0].Rows.Add(headerRow);

                decimal runningBalance = 0;

                foreach (var transaction in allTransactions)
                {
                    runningBalance += transaction.Debit - transaction.Credit;

                    var row = new TableRow();
                    row.Cells.Add(CreateCellWithBorder(transaction.Date.ToString("MM/dd"), FontWeights.Normal, TextAlignment.Center));
                    row.Cells.Add(CreateCellWithBorder(transaction.Reference, FontWeights.Normal, TextAlignment.Center));
                    row.Cells.Add(CreateCellWithBorder(transaction.Description, FontWeights.Normal, TextAlignment.Right));

                    var debitColor = transaction.Debit > 0 ? Brushes.DarkRed : Brushes.Black;
                    var creditColor = transaction.Credit > 0 ? Brushes.DarkGreen : Brushes.Black;

                    row.Cells.Add(CreateCellWithBorder(transaction.Debit > 0 ? $"{transaction.Debit:C}" : "-",
                                                     FontWeights.Normal, TextAlignment.Center, debitColor));
                    row.Cells.Add(CreateCellWithBorder(transaction.Credit > 0 ? $"{transaction.Credit:C}" : "-",
                                                     FontWeights.Normal, TextAlignment.Center, creditColor));
                    row.Cells.Add(CreateCellWithBorder($"{runningBalance:C}", FontWeights.Bold, TextAlignment.Center,
                                                     runningBalance >= 0 ? Brushes.DarkRed : Brushes.DarkGreen));

                    transactionsTable.RowGroups[0].Rows.Add(row);
                }

                flowDocument.Blocks.Add(transactionsTable);
            }
            else
            {
                var noDataParagraph = new Paragraph
                {
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 20),
                    FontSize = 16,
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.Gray
                };
                noDataParagraph.Inlines.Add(new Run("لا توجد معاملات لهذا العميل"));
                noDataParagraph.Inlines.Add(new LineBreak());
                noDataParagraph.Inlines.Add(new Run("No transactions found for this customer"));
                flowDocument.Blocks.Add(noDataParagraph);
            }

            flowDocument.Blocks.Add(new Paragraph { Margin = new Thickness(0, 15, 0, 10) });

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
            summaryHeaderRow.Cells.Add(CreateCellWithBorder("ملخص الحساب", FontWeights.Bold, TextAlignment.Center, Brushes.White, Brushes.DarkGreen, 2));
            summaryTable.RowGroups[0].Rows.Add(summaryHeaderRow);

            var totalSales = sales.Sum(s => s.TotalAmount);
            var totalPayments = payments.Sum(p => p.Amount);
            var currentBalance = customer.Balance;

            var salesRow = new TableRow();
            salesRow.Cells.Add(CreateCellWithBorder("إجمالي المبيعات", FontWeights.Normal, TextAlignment.Right));
            salesRow.Cells.Add(CreateCellWithBorder($"{totalSales:C}", FontWeights.Normal, TextAlignment.Center, Brushes.DarkRed));
            summaryTable.RowGroups[0].Rows.Add(salesRow);

            var paymentsRow = new TableRow();
            paymentsRow.Cells.Add(CreateCellWithBorder("إجمالي المدفوعات", FontWeights.Normal, TextAlignment.Right));
            paymentsRow.Cells.Add(CreateCellWithBorder($"{totalPayments:C}", FontWeights.Normal, TextAlignment.Center, Brushes.DarkGreen));
            summaryTable.RowGroups[0].Rows.Add(paymentsRow);

            var balanceRow = new TableRow { Background = Brushes.LightYellow };
            balanceRow.Cells.Add(CreateCellWithBorder("الرصيد الحالي", FontWeights.Bold, TextAlignment.Right));
            balanceRow.Cells.Add(CreateCellWithBorder($"{currentBalance:C}", FontWeights.Bold, TextAlignment.Center,
                                                    currentBalance >= 0 ? Brushes.DarkRed : Brushes.DarkGreen));
            summaryTable.RowGroups[0].Rows.Add(balanceRow);

            flowDocument.Blocks.Add(summaryTable);

            var footer = new Paragraph
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 5),
                BorderBrush = Brushes.DarkBlue,
                BorderThickness = new Thickness(2),
                Padding = new Thickness(10),
                Background = Brushes.LightCyan
            };

            footer.Inlines.Add(new Run("كشف حساب معتمد")
            {
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkBlue
            });
            footer.Inlines.Add(new LineBreak());
            footer.Inlines.Add(new Run("Official Account Statement")
            {
                FontSize = 14,
                FontWeight = FontWeights.Normal,
                Foreground = Brushes.DarkBlue
            });

            flowDocument.Blocks.Add(footer);

            return flowDocument;
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
                MessageBox.Show("يرجى اختيار عميل.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtPaymentAmount.Text, out decimal paymentAmount) || paymentAmount <= 0)
            {
                MessageBox.Show("يرجى إدخال مبلغ دفعة صالح.", "خطأ في التحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var customerId = (int)cmbPaymentCustomer.SelectedValue;
            var customer = _customerService.GetById(customerId);

            if (customer == null)
            {
                MessageBox.Show("العميل غير موجود.", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (paymentAmount > customer.Balance)
            {
                var result = MessageBox.Show($"مبلغ الدفعة ({paymentAmount:C}) أكبر من رصيد العميل ({customer.Balance:C}). هل تريد المتابعة؟",
                                           "تحذير الدفعة", MessageBoxButton.YesNo, MessageBoxImage.Warning);
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
                MessageBox.Show("تم استلام الدفعة بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في معالجة الدفعة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void PrintPaymentReceipt(Payment payment, Customer customer, decimal newBalance)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true)
                    return;

                var receiptId = DateTime.Now.ToString("yyMMddHHmm");

                var flowDocument = CreatePaymentReceiptDocument(
                    printDialog,
                    receiptId,
                    payment,
                    customer,
                    newBalance);

                printDialog.PrintDocument(
                    ((IDocumentPaginatorSource)flowDocument).DocumentPaginator,
                    "إيصال استلام دفعة");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في طباعة الإيصال: {ex.Message}", "خطأ في الطباعة", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument CreatePaymentReceiptDocument(
            PrintDialog printDialog,
            string receiptId,
            Payment payment,
            Customer customer,
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

            header.Inlines.Add(new Run("إيصال استلام دفعة")
            {
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkGreen
            });
            header.Inlines.Add(new LineBreak());
            header.Inlines.Add(new Run("PAYMENT RECEIPT")
            {
                FontSize = 18,
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
            metaInfoParagraph.Inlines.Add(new Run($"التاريخ: {payment.PaymentDate:yyyy/MM/dd hh:mm tt}")
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
            previousBalanceRow.Cells.Add(CreateCellWithBorder("الرصيد السابق", FontWeights.Normal, TextAlignment.Right));
            previousBalanceRow.Cells.Add(CreateCellWithBorder($"{customer.Balance:C}", FontWeights.Normal, TextAlignment.Center));
            summaryTable.RowGroups[0].Rows.Add(previousBalanceRow);

            var paymentAmountRow = new TableRow();
            paymentAmountRow.Cells.Add(CreateCellWithBorder("مبلغ الدفعة", FontWeights.Normal, TextAlignment.Right));
            paymentAmountRow.Cells.Add(CreateCellWithBorder($"{payment.Amount:C}", FontWeights.Normal, TextAlignment.Center, Brushes.DarkGreen));
            summaryTable.RowGroups[0].Rows.Add(paymentAmountRow);

            var newBalanceRow = new TableRow { Background = Brushes.LightYellow };
            newBalanceRow.Cells.Add(CreateCellWithBorder("الرصيد الجديد", FontWeights.Bold, TextAlignment.Right, Brushes.DarkRed));
            newBalanceRow.Cells.Add(CreateCellWithBorder($"{newBalance:C}", FontWeights.Bold, TextAlignment.Center, Brushes.DarkRed));
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

            footer.Inlines.Add(new Run("شكراً لسداد دفعتكم")
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
                Margin = new Thickness(0),
                Padding = new Thickness(0),
                Foreground = foreground ?? Brushes.Black
            };

            var cell = new TableCell(paragraph)
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0.5),
                Padding = new Thickness(2),
                Background = background ?? Brushes.White,
                ColumnSpan = columnSpan
            };

            return cell;
        }

        private void ClearPaymentForm()
        {
            cmbPaymentCustomer.SelectedIndex = -1;
            txtPaymentAmount.Clear();
            txtPaymentNotes.Clear();
            lblCustomerBalance.Text = "";
        }
    }

    public class CustomerTransaction
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Reference { get; set; }
    }
}