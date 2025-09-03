using ContosoEnergy.Core.Entities;
using ContosoEnergy.Core.Interfaces;
using System;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Threading.Tasks;

namespace ContosoEnergy.Infrastructure.Events
{
    /// <summary>
    /// SQS implementation of <see cref="IEventSink"/>.
    /// </summary>
    /// <remarks>
    /// - Publishes JobCreated events to an AWS SQS queue.
    /// - Enables horizontally scalable, reliable event distribution.
    /// - NFRs:
    ///   <list type="bullet">
    ///     <item><description><b>Scalability:</b> nfr-04</description></item>
    ///     <item><description><b>Observability:</b> nfr-06</description></item>
    ///     <item><description><b>Maintainability:</b> nfr-05</description></item>
    ///   </list>
    /// </remarks>
    public class SqsEventSink : IEventSink
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly string _queueUrl;

        /// <summary>
        /// Initializes a new instance of <see cref="SqsEventSink"/>.
        /// </summary>
        /// <param name="sqsClient">AWS SQS client.</param>
        /// <param name="queueUrl">Target queue URL.</param>
        public SqsEventSink(IAmazonSQS sqsClient, string queueUrl)
        {
            _sqsClient = sqsClient ?? throw new ArgumentNullException(nameof(sqsClient));
            _queueUrl = queueUrl ?? throw new ArgumentNullException(nameof(queueUrl));
        }

        /// <summary>
        /// Publishes a JobCreated event to the configured SQS queue.
        /// </summary>
        /// <param name="job">The Job entity to publish.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task PublishJobCreatedAsync(Job job)
        {
            var request = new Amazon.SQS.Model.SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = $"JobCreated:{job.Id}"
            };

            await _sqsClient.SendMessageAsync(request);
        }
    }
}
