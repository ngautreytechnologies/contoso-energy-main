using Microsoft.AspNetCore.Mvc;
using Ardalis.ApiEndpoints;
using Ardalis.Result;
using ContosoEnergy.Events.Domain.Entities;
using ContosoEnergy.Events.Domain.Specifications;
using ContosoEnergy.Events.Domain.Interfaces;
using ContosoEnergy.WebApi.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ContosoEnergy.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : ControllerBase
    {
        private readonly IRepository<EnergyEvent> _repository;

        public TelemetryController(IRepository<EnergyEvent> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Ingest a new telemetry event
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(EventAckDto), 202)]
        public async Task<ActionResult<EventAckDto>> IngestTelemetry([FromBody] EnergyEventDto dto)
        {
            var entity = new EnergyEvent(dto.CustomerId, dto.MeterId, dto.Voltage, dto.Current, dto.PowerFactor, dto.EnergyKWh);
            await _repository.AddAsync(entity);

            var response = new EventAckDto
            {
                EventId = entity.Id.ToString(),
                Status = "Accepted"
            };

            return Accepted(response);
        }

        /// <summary>
        /// Query telemetry events (optionally by customer)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EnergyEventDto>), 200)]
        public async Task<ActionResult<IEnumerable<EnergyEventDto>>> GetTelemetry([FromQuery] int? customerId, [FromQuery] int limit = 50)
        {
            var spec = new TelemetrySpecification(customerId, limit);
            var events = await _repository.ListAsync(spec);

            var result = new List<EnergyEventDto>();
            foreach (var e in events)
            {
                result.Add(new EnergyEventDto
                {
                    Id = e.Id,
                    CustomerId = e.CustomerId,
                    MeterId = e.MeterId,
                    Voltage = e.Voltage,
                    Current = e.Current,
                    PowerFactor = e.PowerFactor,
                    EnergyKWh = e.EnergyKWh,
                    Timestamp = e.Timestamp
                });
            }

            return Ok(result);
        }

        /// <summary>
        /// Get a specific telemetry event
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EnergyEventDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EnergyEventDto>> GetTelemetryById(Guid id)
        {
            var evt = await _repository.GetByIdAsync(id);
            if (evt == null) return NotFound();

            return Ok(new EnergyEventDto
            {
                Id = evt.Id,
                CustomerId = evt.CustomerId,
                MeterId = evt.MeterId,
                Voltage = evt.Voltage,
                Current = evt.Current,
                PowerFactor = evt.PowerFactor,
                EnergyKWh = evt.EnergyKWh,
                Timestamp = evt.Timestamp
            });
        }
    }
}
