namespace Avn.Services.Interfaces;

public interface IProjectsService : IScopedDependency
{
    /// <summary>
    /// Get all user's project
    /// </summary>
    /// <param name="userid">userId which automatic binded to apis</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<IEnumerable<Project>>> GetAllAsync(Guid userid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add new project for users
    /// </summary>
    /// <param name="model">data that provided by Users</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Project>> CreateAsync(Project model, CancellationToken cancellationToken = default);

    /// <summary>
    /// update existing project by user
    /// </summary>
    /// <param name="model"> new data that provided by users</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Project>> UpdateAsync(Project model, CancellationToken cancellationToken = default);
}
