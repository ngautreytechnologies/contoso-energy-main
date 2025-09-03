using System.Diagnostics.Metrics;
using ContosoEnergy.Core.Interfaces;

namespace ContosoEnergy.Infrastructure.Metrics
{
    /// <summary>
    /// Publishes domain-specific metrics for the JobService.
    /// </summary>
    /// <remarks>
    /// - Tracks jobs created and failed counters.
    /// - Enables observability dashboards (Prometheus → Grafana).
    /// - Related NFRs:
    ///   <nfr-observability>nfr-06</nfr-observability>
    ///   <nfr-scalability>nfr-04</nfr-scalability>
    /// </remarks>
    public class MetricsPublisher(
        Counter<int> jobsCreatedCounter,
        Counter<int> jobsFailedCounter) : IMetricsPublisher
    {
        private readonly Counter<int> _jobsCreatedCounter = jobsCreatedCounter ?? throw new ArgumentNullException(nameof(jobsCreatedCounter));
        private readonly Counter<int> _jobsFailedCounter = jobsFailedCounter ?? throw new ArgumentNullException(nameof(jobsFailedCounter));

        /// <summary>
        /// Increments the jobs.created counter.
        /// </summary>
        /// <param name="jobName">Optional: name of the job.</param>
        public void IncrementJobsCreated(string? jobName = null)
        {
            _jobsCreatedCounter.Add(1,
                new KeyValuePair<string, object?>("name", jobName ?? "unknown"),
                new KeyValuePair<string, object?>("status", "success"));
        }

        /// <summary>
        /// Increments the jobs.failed counter.
        /// </summary>
        /// <param name="jobName">Optional: name of the failed job.</param>
        public void IncrementJobsFailed(string? jobName = null)
        {
            _jobsFailedCounter.Add(1,
                new KeyValuePair<string, object?>("name", jobName ?? "unknown"),
                new KeyValuePair<string, object?>("status", "failed"));
        }
    }
}
