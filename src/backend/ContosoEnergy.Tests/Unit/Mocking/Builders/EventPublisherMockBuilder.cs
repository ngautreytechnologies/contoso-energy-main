using ContosoEnergy.Core.Entities;
using ContosoEnergy.Core.Interfaces;
using Moq;

namespace ContosoEnergy.Tests.Unit.Mocking.Builders
{
    public class EventPublisherMockBuilder
    {
        private readonly Mock<IEventPublisher> _mock = new();

        public EventPublisherMockBuilder WithPublishJobCreatedAsync()
        {
            _mock.Setup(e => e.PublishJobCreatedAsync(It.IsAny<Job>()))
                 .Returns(Task.CompletedTask);
            return this;
        }

        public IEventPublisher Build() => _mock.Object;
    }
}