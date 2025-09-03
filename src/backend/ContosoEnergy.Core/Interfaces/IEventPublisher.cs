using ContosoEnergy.Core.Entities;

namespace ContosoEnergy.Core.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishJobCreatedAsync(Job job);
    }
}
