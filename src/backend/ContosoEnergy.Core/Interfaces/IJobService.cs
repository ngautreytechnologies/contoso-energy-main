using ContosoEnergy.Core.Entities;

namespace ContosoEnergy.Core.Interfaces
{
    /// <summary>
    /// Application service interface for managing Job entities.
    /// 
    /// This interface defines the contract for use cases related to Jobs,
    /// such as creating, retrieving, or listing jobs.
    /// It belongs to the Application layer and orchestrates domain entities
    /// without knowing about EF Core or persistence details.
    /// </summary>
    public interface IJobService
    {
        /// <summary>
        /// Creates a new Job entity with the specified name.
        /// </summary>
        /// <param name="name">The name of the job to create.</param>
        /// <returns>The newly created Job entity.</returns>
        Task<Job> CreateJobAsync(string name);

        /// <summary>
        /// Retrieves all Job entities.
        /// Useful for listing or reporting scenarios.
        /// </summary>
        /// <returns>An enumerable of all Job entities.</returns>
        Task<IEnumerable<Job>> GetAllJobsAsync();

        /// <summary>
        /// Retrieves a Job entity by its unique identifier.
        /// Returns null if the job does not exist.
        /// </summary>
        /// <param name="id">The ID of the job to retrieve.</param>
        /// <returns>The Job entity if found; otherwise, null.</returns>
        Task<Job?> GetJobByIdAsync(int id);
    }
}
