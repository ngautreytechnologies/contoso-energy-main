using ContosoEnergy.Core.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoEnergy.Tests.Unit.Mocking.Builders
{
    public class MetricsPublisherMockBuilder
    {
        private readonly Mock<IMetricsPublisher> _mock = new();

        public MetricsPublisherMockBuilder WithIncrementJobsCreated()
        {
            _mock.Setup(m => m.IncrementJobsCreated(It.IsAny<string>()));
            return this;
        }

        public IMetricsPublisher Build() => _mock.Object;
    }
}
