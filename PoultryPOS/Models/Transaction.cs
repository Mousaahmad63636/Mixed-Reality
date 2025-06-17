namespace PoultryPOS.Models
{
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