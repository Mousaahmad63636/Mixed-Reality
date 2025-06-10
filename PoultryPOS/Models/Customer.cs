namespace PoultryPOS.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; } = true;
    }
}