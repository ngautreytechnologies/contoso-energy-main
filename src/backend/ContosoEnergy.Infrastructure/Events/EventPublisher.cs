using Amazon.SQS;
using ContosoEnergy.Core.Entities;
using ContosoEnergy.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContosoEnergy.Infrastructure.Events
{
    /// <summary>
    /// Strategy-based event publisher for JobCreated events.
    /// </summary>
    /// <remarks>
    /// - Uses the <see cref="IEventSink"/> strategy pattern to support multiple backends.
    /// - Allows decoupling the core domain from infrastructure concerns (SQS, Kafka, in-memory, etc.).
    /// - Supports NFRs:
    ///   <list type="bullet">
    ///     <item><description><b>Observability:</b> nfr-06, logs each event publishing attempt.</description></item>
    ///     <item><description><b>Scalability:</b> nfr-04, can add more sinks or switch to high-throughput systems.</description></item>
    ///     <item><description><b>Maintainability:</b> nfr-05, adding new sinks does not require modifying core logic.</description></item>
    ///   </list>
    /// - Testability: easily mocked via <see cref="IEventSink"/> for unit tests.
    /// </remarks>
    public class EventPublisher : IEventPublisher
    {
        private readonly IEnumerable<IEventSink> _sinks;
        private readonly ILogger<EventPublisher> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="EventPublisher"/>.
        /// </summary>
        /// <param name="sinks">Collection of strategies/sinks to publish events to.</param>
        /// <param name="logger">Logger for structured logs and observability.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sinks"/> or <paramref name="logger"/> is null.</exception>
        public EventPublisher(IEnumerable<IEventSink> sinks, ILogger<EventPublisher> logger)
        {
            _sinks = sinks ?? throw new ArgumentNullException(nameof(sinks));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Publishes a JobCreated event to all configured sinks.
        /// </summary>
        /// <param name="job">The Job entity that was created.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <remarks>
        /// - Logs each publishing attempt and failure.
        /// - Continues publishing to remaining sinks even if one fails (resilience).
        /// - Related NFRs:
        ///   <list type="bullet">
        ///     <item><description><b>Observability:</b> nfr-06</description></item>
        ///     <item><description><b>Scalability:</b> nfr-04</description></item>
        ///   </list>
        /// - Encourages use of Dependency Injection to swap sinks for testing, staging, or production.
        /// </remarks>
        public async Task PublishJobCreatedAsync(Job job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));

            foreach (var sink in _sinks)
            {
                try
                {
                    await sink.PublishJobCreatedAsync(job);
                    _logger.LogInformation(
                        "Published JobCreated event to {SinkType} for JobId {JobId}",
                        sink.GetType().Name, job.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to publish JobCreated event to {SinkType} for JobId {JobId}",
                        sink.GetType().Name, job.Id);
                    // Resilience: continue to next sink to prevent single point of failure
                }
            }
        }
    }
}
