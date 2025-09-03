using ContosoEnergy.Core.Entities;

namespace ContosoEnergy.Core.Interfaces
{

    /// <summary>
    /// Contract for individual event sink (strategy) implementations.
    /// </summary>
    /// <remarks>
    /// - Defines a standard interface for sinks like SQS, Kafka, or in-memory queues.
    /// - Allows the core service to remain agnostic of the transport mechanism.
    /// - NFRs:
    ///   <list type="bullet">
    ///     <item><description><b>Scalability:</b> nfr-04, can add new sinks without modifying core service.</description></item>
    ///     <item><description><b>Maintainability:</b> nfr-05, single-responsibility principle for each sink.</description></item>
    ///   </list>
    /// </remarks>
    public interface IEventSink
    {
        /// <summary>
        /// Publishes a JobCreated event to the sink asynchronously.
        /// </summary>
        /// <param name="job">The Job entity to publish.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PublishJobCreatedAsync(Job job);
    }
}