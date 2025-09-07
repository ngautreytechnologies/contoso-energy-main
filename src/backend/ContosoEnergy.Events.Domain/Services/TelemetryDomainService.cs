using ContosoEnergy.Core.Entities;
using ContosoEnergy.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace ContosoEnergy.Core.Services
{
    /// <summary>
    /// Application service for managing Jobs.
    /// Contains business logic and coordinates with the repository layer.
    /// </summary>
    /// <remarks>
    /// Injects repository and optionally an SQS client + queue URL for publishing events (MVP).
    /// <nfr-observability>nfr-06</nfr-observability>
    /// <nfr-performance>nfr-01</nfr-performance>
    /// <fr-ids>fr-01</fr-ids>
    /// </remarks>
    public class TelemetryDomainService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly ILogger<TelemetryDomainService> _logger;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMetricsPublisher _metricsPublisher;

        public TelemetryDomainService(
            IJobRepository jobRepository,
            ILogger<TelemetryDomainService> logger,
            IEventPublisher eventPublisher,
            IMetricsPublisher metricsPublisher)
        {
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _metricsPublisher = metricsPublisher ?? throw new ArgumentNullException(nameof(metricsPublisher));
        }

        /// <summary>
        /// Creates a new job and records success in metrics.
        /// </summary>
        /// <param name="name">Job name.</param>
        /// <remarks>
        /// - Captures successful job creation events.
        /// - Enables dashboard visibility (Prometheus → Grafana).
        /// - Related FR: <fr-01/>
        /// - Related NFR: <nfr-observability>nfr-06</nfr-observability>
        /// </remarks>
        /// <returns>Created job instance.</returns>
        public async Task<Job> CreateJobAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogError("Job name cannot be null or empty");
                throw new ArgumentException("Job name cannot be null or empty", nameof(name));
            }

            _logger.LogDebug("Creating Job with name: {JobName}", name);

            Job result;

            try
            {
                // Create the job entity
                result = await _jobRepository.AddAsync(new Job(name));

                ArgumentNullException.ThrowIfNull(result, nameof(result));

                _logger.LogInformation("Job created with Id: {JobId}", result.Id);

                // Increment metrics
                _metricsPublisher.IncrementJobsCreated(name);

                // Publish event
                await _eventPublisher.PublishJobCreatedAsync(result);

                _logger.LogInformation("Published JobCreated event for JobId: {JobId}", result.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Job with name: {JobName}", name);

                // Increment failure metric
                _metricsPublisher.IncrementJobsFailed(name);

                throw;
            }

            _logger.LogInformation("Job created successfully with Id: {JobId}", result.Id);

            return result;
        }

        /// <summary>
        /// Retrieves all jobs from the repository.
        /// </summary>
        /// <returns>A collection of Job entities.</returns>
        /// <remarks>
        /// <nfr-observability>nfr-06</nfr-observability>
        /// <nfr-performance>nfr-01;nfr-04</nfr-performance>
        /// <fr-ids>fr-03</fr-ids>
        /// </remarks>
        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            _logger.LogInformation("Fetching all Jobs from repository");
            return await _jobRepository.ListAsync(specification: null, CancellationToken.None);
        }

        /// <summary>
        /// Retrieves a Job by its primary key (Id).
        /// Returns null if no job is found.
        /// </summary>
        /// <param name="id">Primary key of the Job entity.</param>
        /// <returns>The Job entity or null.</returns>
        /// <remarks>
        /// <nfr-observability>nfr-06</nfr-observability>
        /// <nfr-performance>nfr-01;nfr-04</nfr-performance>
        /// <fr-ids>fr-02</fr-ids>
        /// </remarks>
        public async Task<Job?> GetJobByIdAsync(int id)
        {
            _logger.LogInformation("Fetching Job by Id {JobId}", id);
            return await _jobRepository.GetByIdAsync(id);
        }
    }
}
