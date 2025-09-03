namespace ContosoEnergy.Core.Interfaces
{
    public interface IMetricsPublisher
    {

        /// <summary>
        /// Increments the jobs.created counter.
        /// </summary>
        /// <param name="jobName">Optional: name of the job.</param>
        public void IncrementJobsCreated(string? jobName = null);

        /// <summary>
        /// Increments the jobs.failed counter.
        /// </summary>
        /// <param name="jobName">Optional: name of the failed job.</param>
        public void IncrementJobsFailed(string? jobName = null);
    }
}
