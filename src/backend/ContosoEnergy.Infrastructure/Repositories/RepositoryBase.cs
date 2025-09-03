using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using ContosoEnergy.Core.Interfaces;
using ContosoEnergy.Infrastructure.Data;

namespace ContosoEnergy.Infrastructure.Repositories
{
    /// <summary>
    /// Generic EF Core repository base class for all entities.
    /// 
    /// Inherits from Ardalis.Specification's RepositoryBase<T>, which provides:
    /// - Generic CRUD operations
    /// - Specification support for flexible queries (filters, sorting, paging)
    /// 
    /// This class lives in the Infrastructure layer because it depends on EF Core.
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <remarks>
    /// Constructor accepts the application's EF Core DbContext.
    /// </remarks>
    /// <param name="dbContext">ContosoDbContext instance</param>
    public class EfRepositoryBase<T>(ContosoDbContext dbContext) : RepositoryBase<T>(dbContext), IRepositoryBase<T> where T : class
    {

        /// <summary>
        /// Additional generic or helper methods for all repositories could be added here.
        /// For example, common query extensions, soft-delete handling, etc.
        /// </summary>
    }
}
