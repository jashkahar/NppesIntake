using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NppesIntake.Infrastructure;

public class NppesIntakeDbContextFactory : IDesignTimeDbContextFactory<NppesIntakeDbContext>
{
    public NppesIntakeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NppesIntakeDbContext>();

        optionsBuilder.UseSqlServer("Server=.;Database=DesignTimeDb;Trusted_Connection=True;TrustServerCertificate=true");

        return new NppesIntakeDbContext(optionsBuilder.Options);
    }
}