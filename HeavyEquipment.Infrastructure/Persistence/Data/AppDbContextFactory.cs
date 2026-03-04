using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HeavyEquipment.Infrastructure.Persistence.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=.;Database=HeavyEquipmentDB;Trusted_Connection=True;TrustServerCertificate=True"
            );

            return new AppDbContext(optionsBuilder.Options, null!);
        }
    }
}
