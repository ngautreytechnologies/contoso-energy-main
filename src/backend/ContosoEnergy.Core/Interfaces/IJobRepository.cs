using Ardalis.Specification;
using ContosoEnergy.Core.Entities;

namespace ContosoEnergy.Core.Interfaces
{
    /// <summary>
    /// Repository interface for the <see cref="Job"/> entity.
    /// 
    /// This defines the contract for data access operations related to Jobs
    /// without tying the domain to a specific persistence implementation.
    /// 
    /// Inherits from <see cref="IRepositoryBase{T}"/> from Ardalis.Specification,
    /// giving flexible querying capabilities (filters, projections, sorting, pagination).
    /// </summary>
    public interface IJobRepository : IRepositoryBase<Job>
    {
        /// <summary>
        /// Retrieves a Job entity by its name.
        /// Returns null if no matching job exists.
        /// </summary>
        /// <param name="name">The name of the job to retrieve.</param>
        /// <param name="cancellationToken">Optional cancellation token for async operations.</param>
        /// <returns>A task that resolves to the Job, or null if not found.</returns>
        Task<Job?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        // TODO: Add any additional domain-specific query methods here,
        // e.g., GetJobsByStatusAsync, GetRecentJobsAsync, etc.
    }
}
