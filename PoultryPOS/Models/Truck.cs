namespace PoultryPOS.Models
{
    public class Truck
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CurrentLoad { get; set; }
        public decimal NetWeight { get; set; }
        public string? PlateNumber { get; set; }
        public bool IsActive { get; set; } = true;
    }
}