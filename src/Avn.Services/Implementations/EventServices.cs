using Avn.Domain.Dtos.Events;

namespace Avn.Services.Interfaces;

public class EventServices : IEventServices
{
    public Task<IActionResponse<EventDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResponse<object>> CreateAsync(CreateEventDto item, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResponse<object>> UpdateAsync(UpdateEventDto item, CancellationToken cancellation = default)
    {
        throw new NotImplementedException();
    }
}
