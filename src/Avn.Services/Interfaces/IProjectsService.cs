using Avn.Domain.Entities; 

namespace Avn.Services.Interfaces
{
    public interface IProjectsService
    {
        Task<IActionResponse<IEnumerable<Project>>> GetAllAsync(Guid userid, CancellationToken cancellationToken = default);

        Task<IActionResponse<Project>> CreateAsync(Project item, CancellationToken cancellationToken = default);

        Task<IActionResponse<Project>> UpdateAsync(Project item, CancellationToken cancellationToken = default);

    }
}
