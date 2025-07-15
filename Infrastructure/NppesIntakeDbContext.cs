using Microsoft.EntityFrameworkCore;
using NppesIntake.Core.Entities;

namespace NppesIntake.Infrastructure;

public class NppesIntakeDbContext : DbContext
{
    public NppesIntakeDbContext(DbContextOptions<NppesIntakeDbContext> options) : base(options)
    {
    }

    public DbSet<Member> Members { get; set; }
    public DbSet<MemberContactDetails> MemberContactDetails { get; set; }
    public DbSet<BusinessUnit> BusinessUnits { get; set; }
    public DbSet<BusinessUnitMember> BusinessUnitMembers { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusinessUnitMember>()
            .HasKey(bm => new { bm.MemberId, bm.BusinessUnitId });

        modelBuilder.Entity<BusinessUnitMember>()
            .HasOne(bm => bm.Member)
            .WithMany(m => m.BusinessUnitMembers)
            .HasForeignKey(bm => bm.MemberId);

        modelBuilder.Entity<BusinessUnitMember>()
            .HasOne(bm => bm.BusinessUnit)
            .WithMany(b => b.BusinessUnitMembers)
            .HasForeignKey(bm => bm.BusinessUnitId);

        modelBuilder.Entity<Member>()
        .HasIndex(m => m.Npi)
        .IsUnique();

        modelBuilder.Entity<BusinessUnit>()
            .HasIndex(b => b.OrganizationName);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableEntity && (
                    e.State == EntityState.Added || e.State == EntityState.Modified));

        var auditEntries = new List<AuditLog>();
        foreach (var entry in entries)
        {
            var auditableEntity = (AuditableEntity)entry.Entity;
            auditableEntity.UpdatedAtUtc = DateTime.UtcNow;

            var audit = new AuditLog
            {
                TableName = entry.Metadata.GetTableName()!,
                TimestampUtc = DateTime.UtcNow,
            };

            if (entry.State == EntityState.Added)
            {
                auditableEntity.CreatedAtUtc = DateTime.UtcNow;
                audit.Action = "CREATE";
                audit.Changes = System.Text.Json.JsonSerializer.Serialize(entry.CurrentValues.ToObject());
            }
            else // EntityState.Modified
            {
                audit.Action = "UPDATE";
                var changedProperties = entry.Properties.Where(p => p.IsModified).ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                audit.Changes = System.Text.Json.JsonSerializer.Serialize(changedProperties);
                // Set RecordId after the entity has been saved and has an ID
                audit.RecordId = auditableEntity.Id;
            }
            auditEntries.Add(audit);
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        // Save Audit Logs after the main transaction
        if (auditEntries.Any())
        {
            foreach (var audit in auditEntries)
            {
                // Ensure RecordId is set for newly created entities
                if (audit.RecordId == 0)
                {
                    var auditableEntity = entries.Select(e => e.Entity as AuditableEntity).FirstOrDefault(e => e.CreatedAtUtc == audit.TimestampUtc);
                    if (auditableEntity != null) audit.RecordId = auditableEntity.Id;
                }
            }
            AuditLogs.AddRange(auditEntries);
            await base.SaveChangesAsync(cancellationToken);
        }

        return result;
    }

}