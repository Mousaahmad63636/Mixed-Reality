namespace PoultryPOS.Models
{
    public class SyncConfiguration
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public DateTime LastSyncDate { get; set; }
        public string? CloudFolderPath { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class SyncLog
    {
        public int LogId { get; set; }
        public DateTime SyncDate { get; set; }
        public string? Operation { get; set; }
        public string? TableName { get; set; }
        public int? RecordId { get; set; }
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
        public string? DeviceId { get; set; }
    }

    public class ConflictLog
    {
        public int ConflictId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public string? LocalData { get; set; }
        public string? RemoteData { get; set; }
        public string? ResolutionAction { get; set; }
        public DateTime ResolvedDate { get; set; }
        public string? DeviceId { get; set; }
    }

    public class DailySyncFile
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Date { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public List<SyncChange> Changes { get; set; } = new List<SyncChange>();
    }

    public class SyncChange
    {
        public string Table { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public int RecordId { get; set; }
        public string SyncId { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }
        public int Version { get; set; }
        public Dictionary<string, object?> Data { get; set; } = new Dictionary<string, object?>();
    }
}