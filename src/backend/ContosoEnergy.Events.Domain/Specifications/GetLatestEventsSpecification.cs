using Ardalis.Specification;
using System.Linq;

namespace ContosoEnergy.Events.Domain.Specifications
{
    public class GetLatestEventsSpecification : Specification<EnergyEvent>
    {
        public GetLatestEventsSpecification(int? customerId, int limit)
        {
            Query.OrderByDescending(e => e.Timestamp);

            if (customerId.HasValue)
            {
                Query.Where(e => e.CustomerId == customerId.Value);
            }

            if (limit > 0)
            {
                Query.Take(limit);
            }
        }
    }
}
