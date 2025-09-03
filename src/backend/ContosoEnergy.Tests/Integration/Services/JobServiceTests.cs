using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using ContosoEnergy.Core.Entities;

namespace ContosoEnergy.Tests.Integration.Services
{
    public class JobsApiBlackBoxTests
    {
        private readonly HttpClient _client;

        public JobsApiBlackBoxTests()
        {
            // Point to the running API (can be localhost or test environment)
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001") // adjust to your API
            };
        }

        [Fact(DisplayName = "Can create a job via POST /jobs")]
        public async Task CreateJob_ShouldReturnCreatedJob()
        {
            // Arrange
            var newJob = new Job(name: "BlackBox Test Job");

            // Act
            var response = await _client.PostAsJsonAsync("/jobs", newJob);
            response.EnsureSuccessStatusCode();

            var createdJob = await response.Content.ReadFromJsonAsync<Job>();

            // Assert
            createdJob.Should().NotBeNull();
            createdJob.Id.Should().NotBeEmpty();
            createdJob.Name.Should().Be(newJob.Name);
            createdJob.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(5));
        }

        [Fact(DisplayName = "Can read a job via GET /jobs/{id}")]
        public async Task GetJob_ShouldReturnExistingJob()
        {
            // First, create a job
            var newJob = new Job(name: "BlackBox Read Test Job");
            var createResponse = await _client.PostAsJsonAsync("/jobs", newJob);
            createResponse.EnsureSuccessStatusCode();
            var createdJob = await createResponse.Content.ReadFromJsonAsync<Job>();

            // Act
            var getResponse = await _client.GetAsync($"/jobs/{createdJob.Id}");
            getResponse.EnsureSuccessStatusCode();
            var fetchedJob = await getResponse.Content.ReadFromJsonAsync<Job>();

            // Assert
            fetchedJob.Should().NotBeNull();
            fetchedJob.Id.Should().Be(createdJob.Id);
            fetchedJob.Name.Should().Be(createdJob.Name);
            fetchedJob.CreatedAt.Should().BeCloseTo(createdJob.CreatedAt, precision: TimeSpan.FromSeconds(5));
        }
    }
}
