using Avn.Domain.Dtos.Events;

namespace Avn.Services.Interfaces;

public interface IEventsService : IScopedDependency
{
    Task<IActionResponse<IEnumerable<EventDto>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IActionResponse<Guid>> CreateAsync(CreateEventDto item, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> ConfirmAsync(int eventId, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> RejectAsync(int eventId, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> DeactiveAsync(Guid code, CancellationToken cancellationToken = default);
}

