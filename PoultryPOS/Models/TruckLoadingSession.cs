namespace PoultryPOS.Models
{
    public class TruckLoadingSession
    {
        public int Id { get; set; }
        public int TruckId { get; set; }
        public DateTime LoadDate { get; set; }
        public int InitialCages { get; set; }
        public decimal InitialWeight { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? FinalWeight { get; set; }
        public decimal? WeightVariance { get; set; }
        public bool IsCompleted { get; set; }

        public string? TruckName { get; set; }
        public string? LoadDateDisplay => LoadDate.ToString("yyyy-MM-dd HH:mm");
        public string? CompletionDateDisplay => CompletionDate?.ToString("yyyy-MM-dd HH:mm");
        public string VarianceDisplay => WeightVariance?.ToString("F2") ?? "0.00";
        public string StatusDisplay => IsCompleted ? "مكتمل" : "جاري";
        public decimal VariancePercentage => InitialWeight > 0 && WeightVariance.HasValue ?
            (WeightVariance.Value / InitialWeight) * 100 : 0;
    }
}