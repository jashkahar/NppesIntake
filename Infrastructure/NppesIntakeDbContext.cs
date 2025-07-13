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
    }
}