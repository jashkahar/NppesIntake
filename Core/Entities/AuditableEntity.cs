using System.ComponentModel.DataAnnotations;

namespace NppesIntake.Core.Entities;

public abstract class AuditableEntity
{
    [Key]
    public int Id { get; set; }
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}