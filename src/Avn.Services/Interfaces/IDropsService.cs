namespace Avn.Services.Interfaces;

public interface IDropsService : IScopedDependency
{
    /// <summary>
    /// Create a drop with Ui/Api
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Guid>> CreateAsync(CreateDropDto item, CancellationToken cancellationToken = default);

    Task<IActionResponse<IEnumerable<DropDto>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> ConfirmAsync(int DropId, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> RejectAsync(int DropId, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> DeactiveAsync(Guid code, CancellationToken cancellationToken = default);
}