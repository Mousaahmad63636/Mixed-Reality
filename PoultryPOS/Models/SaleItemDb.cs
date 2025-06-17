namespace PoultryPOS.Models
{
    public class SaleItemDb
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public decimal GrossWeight { get; set; }
        public int NumberOfCages { get; set; }
        public decimal SingleCageWeight { get; set; }
        public decimal TotalCageWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal TotalAmount { get; set; }
    }
}