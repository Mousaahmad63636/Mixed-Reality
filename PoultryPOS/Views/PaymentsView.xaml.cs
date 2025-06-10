using PoultryPOS.Models;
using PoultryPOS.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Printing;

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
                    "Payment Receipt");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing receipt: {ex.Message}", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            footer.Inlines.Add(new LineBreak());
            footer.Inlines.Add(new Run("Thank you for your payment!")
            {
                FontSize = 14,
                FontWeight = FontWeights.Normal,
                Foreground = Brushes.DarkGreen
            });

            flowDocument.Blocks.Add(footer);

            return flowDocument;
        }

        private TableCell CreateCellWithBorder(string text, FontWeight fontWeight = default, TextAlignment alignment = TextAlignment.Left, Brush foreground = null, Brush background = null)
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
                Background = background ?? Brushes.White
            };

            return cell;
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