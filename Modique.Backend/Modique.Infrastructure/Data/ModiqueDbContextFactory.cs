using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Modique.Infrastructure.Data
{
    
    public class ModiqueDbContextFactory : IDesignTimeDbContextFactory<ModiqueDbContext>
    {
        public ModiqueDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ModiqueDbContext>();

            
            var connectionString =
                "Server=(localdb)\\MSSQLLocalDB;Database=ModiqueDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

            optionsBuilder.UseSqlServer(connectionString);
            return new ModiqueDbContext(optionsBuilder.Options);
        }
    }
}
