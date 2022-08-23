using Avn.Domain.Dtos.Events;

namespace Avn.Services.Interfaces;

public interface IEventServices : IScopedDependency
{

    Task<IActionResponse<EventDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IActionResponse<object>> CreateAsync(CreateEventDto item, CancellationToken cancellationToken = default);
    Task<IActionResponse<object>> UpdateAsync(UpdateEventDto item, CancellationToken cancellation = default);
}

