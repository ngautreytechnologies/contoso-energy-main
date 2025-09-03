using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ContosoEnergy.Infrastructure.Data
{
    /// <summary>
    /// Design-time factory for ContosoDbContext.
    /// EF Core uses this to create a DbContext instance when running migrations.
    /// This avoids requiring dependency injection or a running API project.
    /// </summary>
    public class ContosoDbContextFactory : IDesignTimeDbContextFactory<ContosoDbContext>
    {
        /// <summary>
        /// Called by EF Core Tools at design time to create a DbContext instance.
        /// </summary>
        /// <param name="args">Command-line arguments (unused here).</param>
        /// <returns>A configured ContosoDbContext instance.</returns>
        public ContosoDbContext CreateDbContext(string[] args)
        {
            // 1️⃣ Create a DbContextOptionsBuilder for ContosoDbContext
            var optionsBuilder = new DbContextOptionsBuilder<ContosoDbContext>();

            // 2️⃣ Configure the database provider to PostgreSQL
            // Replace with your actual host, database, username, and password
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=ContosoEnergyDb;Username=postgres;Password=YourPassword");

            // 3️⃣ Return a new ContosoDbContext instance with the configured options
            return new ContosoDbContext(optionsBuilder.Options);
        }
    }
}
