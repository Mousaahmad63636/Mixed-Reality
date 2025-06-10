namespace PoultryPOS.Models
{
    public class Transaction
    {
        public DateTime Date { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string AmountDisplay { get; set; } = string.Empty;
        public string? TruckName { get; set; }
        public string? DriverName { get; set; }
        public string? Notes { get; set; }
        public int CustomerId { get; set; }
    }
}