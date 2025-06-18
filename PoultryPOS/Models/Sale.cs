namespace PoultryPOS.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int? TruckId { get; set; }
        public int? DriverId { get; set; }
        public decimal GrossWeight { get; set; }
        public int NumberOfCages { get; set; }
        public decimal CageWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal PricePerKg { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaidNow { get; set; }
        public DateTime SaleDate { get; set; }

        public string? CustomerName { get; set; }
        public string? TruckName { get; set; }
        public string? DriverName { get; set; }
    }
}