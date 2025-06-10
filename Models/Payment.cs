namespace PoultryPOS.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
        public string? CustomerName { get; set; }
    }
}