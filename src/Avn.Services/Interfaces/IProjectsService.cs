namespace Avn.Services.Interfaces;

public interface IProjectsService : IScopedDependency
{
    /// <summary>
    /// Get all user's project
    /// </summary>
    /// <param name="userid">userId which automatic binded to apis</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<IEnumerable<object>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add new project for users
    /// </summary>
    /// <param name="model">data that provided by Users</param> 
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Guid>> CreateAsync(CreateProjectDto item, CancellationToken cancellationToken = default);

    /// <summary>
    /// update existing project by user
    /// </summary>
    /// <param name="model"> new data that provided by users</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Guid>> UpdateAsync(UpdateProjectDto item, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disable/Enable a project by user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="projectId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Guid>> ChangeStateAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
}
