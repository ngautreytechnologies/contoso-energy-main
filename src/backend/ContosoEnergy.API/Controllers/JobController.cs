using ContosoEnergy.Core.Services;
using ContosoEnergy.Core.Interfaces;
using ContosoEnergy.Core.Entities; 
using Microsoft.AspNetCore.Mvc;

namespace ContosoEnergy.API.Controllers
{
    /// <summary>
    /// API Controller for managing Jobs.
    /// Exposes endpoints to create, retrieve, and list jobs.
    /// </summary>
    /// <remarks>
    /// Constructor injects the JobService.
    /// </remarks>
    /// <param name="jobService">Injected application service for Jobs.</param>
    [ApiController]
    [Route("api/[controller]")] // Route: api/jobs
    public class JobsController(IJobService jobService) : ControllerBase
    {
        // Application service for handling job operations
        private readonly IJobService _jobService = jobService;

        /// <summary>
        /// GET api/jobs
        /// Retrieves all jobs.
        /// </summary>
        /// <returns>List of Job entities.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return Ok(jobs); // Returns HTTP 200 with job list
        }

        /// <summary>
        /// GET api/jobs/{id}
        /// Retrieves a specific job by its Id.
        /// </summary>
        /// <param name="id">Job primary key.</param>
        /// <returns>Job entity or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null) return NotFound(); // HTTP 404 if job does not exist
            return Ok(job); // HTTP 200 with job entity
        }

        /// <summary>
        /// POST api/jobs
        /// Creates a new job.
        /// </summary>
        /// <param name="name">Job name/title passed in request body.</param>
        /// <returns>Created Job entity with HTTP 201.</returns>
        [HttpPost]
        public async Task<ActionResult<Job>> CreateJob([FromBody] string name)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Name cannot be empty");

            // Create job via service
            var job = await _jobService.CreateJobAsync(name);

            // Returns HTTP 201 Created with route to newly created resource
            return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
        }
    }
}
