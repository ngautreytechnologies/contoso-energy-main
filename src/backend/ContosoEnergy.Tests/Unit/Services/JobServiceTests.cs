using ContosoEnergy.Core.Entities;
using ContosoEnergy.Core.Interfaces;
using ContosoEnergy.Core.Services;
using ContosoEnergy.Tests.Unit.Mocking.Builders;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ContosoEnergy.Tests.Unit.Services
{
    public class JobServiceTests
    {
        private readonly ILogger<JobService> _loggerMock;

        public JobServiceTests()
        {
            _loggerMock = NullLogger<JobService>.Instance;
        }

        [Fact]
        public async Task CreateJobAsync_WithValidName_ReturnsJob()
        {
            // <fr>FR-01</fr> <nfr>nfr-performance;nfr-observability</nfr>
            var jobName = "Meter Reading";

            var jobRepoMock = new JobRepositoryMockBuilder()
                                  .WithAddAsync()
                                  .Build();

            var eventPublisherMock = new EventPublisherMockBuilder()
                                        .WithPublishJobCreatedAsync()
                                        .Build();

            var metricsPublisherMock = new Mock<IMetricsPublisher>();
            metricsPublisherMock.Setup(m => m.IncrementJobsCreated(It.IsAny<string>()));

            var service = new JobService(
                jobRepoMock,
                _loggerMock,
                eventPublisherMock,
                metricsPublisherMock.Object);

            var result = await service.CreateJobAsync(jobName);

            result.Should().NotBeNull();
            result.Name.Should().Be(jobName);

            Mock.Get(eventPublisherMock).Verify(e => e.PublishJobCreatedAsync(result), Times.Once);
            metricsPublisherMock.Verify(m => m.IncrementJobsCreated(jobName), Times.Once);
        }

        [Fact]
        public async Task CreateJobAsync_WithEmptyName_ThrowsArgumentException()
        {
            var jobRepoMock = new JobRepositoryMockBuilder().Build();
            var eventPublisherMock = new EventPublisherMockBuilder().Build();

            var metricsPublisherMock = new Mock<IMetricsPublisher>();
            metricsPublisherMock.Setup(m => m.IncrementJobsFailed(It.IsAny<string>()));

            var service = new JobService(
                jobRepoMock,
                _loggerMock,
                eventPublisherMock,
                metricsPublisherMock.Object);

            Func<Task> act = async () => await service.CreateJobAsync("");

            await act.Should().ThrowAsync<ArgumentException>();
            metricsPublisherMock.Verify(m => m.IncrementJobsFailed(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetAllJobsAsync_ReturnsJobs()
        {
            var jobs = new List<Job> { new("Billing Run") };
            var jobRepoMock = new JobRepositoryMockBuilder()
                                  .WithListAsync(jobs)
                                  .Build();

            var eventPublisherMock = new EventPublisherMockBuilder().Build();
            var service = new JobService(
                jobRepoMock,
                _loggerMock,
                eventPublisherMock,
                Mock.Of<IMetricsPublisher>());

            var result = await service.GetAllJobsAsync();

            result.Should().NotBeNullOrEmpty()
                  .And.HaveCount(1)
                  .And.ContainSingle(j => j.Name == "Billing Run");
        }

        [Fact]
        public async Task GetJobByIdAsync_ReturnsJob()
        {
            var job = new Job("Meter Reading") { Id = Guid.NewGuid() };
            var jobRepoMock = new JobRepositoryMockBuilder()
                                  .WithGetById(1, job)
                                  .Build();

            var eventPublisherMock = new EventPublisherMockBuilder().Build();
            var service = new JobService(
                            jobRepoMock,
                            _loggerMock,
                            eventPublisherMock,
                            Mock.Of<IMetricsPublisher>());

            var result = await service.GetJobByIdAsync(1);

            result.Should().NotBeNull();
            result!.Id.Should().Be(job.Id);
            result.Name.Should().Be("Meter Reading");
        }
    }
}
