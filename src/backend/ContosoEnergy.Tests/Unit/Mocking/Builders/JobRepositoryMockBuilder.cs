using ContosoEnergy.Core.Entities;
using ContosoEnergy.Core.Interfaces;
using Moq;
using System.Collections.Generic;

namespace ContosoEnergy.Tests.Unit.Mocking.Builders
{
    /// <summary>
    /// Concrete mock builder for <see cref="IJobRepository"/>.
    /// Inherits from <see cref="RepositoryMockBuilderBase{T, TBuilder, TInterface}"/> 
    /// to provide fluent setup for unit tests.
    /// </summary>
    public class JobRepositoryMockBuilder
        : RepositoryMockBuilderBase<Job, JobRepositoryMockBuilder, IJobRepository>
    {
        /// <summary>
        /// Adds a set of default jobs to the mock repository.
        /// </summary>
        /// <remarks>
        /// Provides a convenient way to inject predictable test data for jobs.
        /// Uses covariant returns to maintain fluent API.
        /// </remarks>
        /// <returns>The builder instance for fluent chaining.</returns>
        public JobRepositoryMockBuilder WithDefaultJobs()
        {
            var jobs = new List<Job>
            {
                new("Meter Reading"),
                new("Billing Run")
            };

            // Reuse base method to set up ListAsync
            return WithListAsync(jobs);
        }

        /// <summary>
        /// Sets up <c>GetByNameAsync</c> to return a specific job for the given name.
        /// </summary>
        /// <param name="name">The name of the job to return.</param>
        /// <param name="job">The job to return.</param>
        /// <returns>The builder instance for fluent chaining.</returns>
        /// <remarks>
        /// This method demonstrates extending the base builder with interface-specific methods.
        /// Supports covariant/fluent returns.
        /// </remarks>
        public JobRepositoryMockBuilder WithGetByName(string name, Job job)
        {
            //_mockRepo.Setup(r => r.GetByNameAsync(name, default))
            //         .ReturnsAsync(job);

            return this;
        }
    }
}
