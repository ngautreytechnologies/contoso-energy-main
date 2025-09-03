using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;

namespace ContosoEnergy.Tests.Unit.Mocking.Builders
{
    /// <summary>
    /// Fluent builder for mocking IAmazonSQS client for unit tests.
    /// </summary>
    public class SqsClientMockBuilder
    {
        private readonly Mock<IAmazonSQS> _mockClient;
        private string _queueUrl;
        private Func<SendMessageRequest, CancellationToken, Task<SendMessageResponse>> _sendMessageFunc;

        public SqsClientMockBuilder()
        {
            _mockClient = new Mock<IAmazonSQS>();
            _queueUrl = "https://mock-queue-url";
            _sendMessageFunc = (req, ct) => Task.FromResult(new SendMessageResponse { MessageId = "mock-message-id" });
        }

        /// <summary>
        /// Sets the queue URL for the mock client.
        /// </summary>
        public SqsClientMockBuilder WithQueueUrl(string queueUrl)
        {
            _queueUrl = queueUrl;
            return this;
        }

        /// <summary>
        /// Configures custom behavior for SendMessageAsync.
        /// </summary>
        public SqsClientMockBuilder WithSendMessageAsync(Func<SendMessageRequest, CancellationToken, Task<SendMessageResponse>> sendMessageFunc)
        {
            _sendMessageFunc = sendMessageFunc;
            return this;
        }

        /// <summary>
        /// Builds the mocked IAmazonSQS instance.
        /// </summary>
        public IAmazonSQS Build()
        {
            _mockClient
                .Setup(c => c.SendMessageAsync(It.IsAny<SendMessageRequest>(), It.IsAny<CancellationToken>()))
                .Returns((SendMessageRequest req, CancellationToken ct) =>
                {
                    // Use provided queue URL if not null
                    req.QueueUrl ??= _queueUrl;
                    return _sendMessageFunc(req, ct);
                });

            return _mockClient.Object;
        }
    }
}
