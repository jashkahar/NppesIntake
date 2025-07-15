using System.ComponentModel.DataAnnotations;

namespace NppesIntake.Core.Entities;

public class AuditLog
{
    [Key]
    public long Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public int RecordId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Changes { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}