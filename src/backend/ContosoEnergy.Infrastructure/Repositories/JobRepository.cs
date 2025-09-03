using ContosoEnergy.Core.Entities;
using ContosoEnergy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using ContosoEnergy.Core.Interfaces;

namespace ContosoEnergy.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Job entity.
    /// Inherits from Ardalis.Specification's EfRepositoryBase to provide
    /// standard CRUD operations and specification support.
    /// </summary>
    public class JobRepository(ContosoDbContext dbContext)
        : EfRepositoryBase<Job>(dbContext), IJobRepository
    {
        // Keep a reference to the DbContext for direct queries when needed
        private readonly ContosoDbContext _dbContext = dbContext;

        /// <summary>
        /// Retrieves a Job entity by its Name.
        /// This is a simple LINQ query; for more complex or reusable queries,
        /// you could create an Ardalis Specification instead.
        /// </summary>
        /// <param name="name">The name of the job to search for.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The Job entity if found; otherwise, null.</returns>
        public async Task<Job?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            // Direct EF Core query using DbSet
            return await _dbContext.Set<Job>()
                .FirstOrDefaultAsync(j => j.Name == name, cancellationToken);
        }
    }
}
