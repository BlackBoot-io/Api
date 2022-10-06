namespace Avn.Api.Controllers;

public class ProjectsController : BaseController
{
    private readonly IProjectsService _projectService;
    public ProjectsController(IProjectsService projectService) => _projectService = projectService;

    /// <summary>
    /// Create a Project and get api-key to integration
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] CreateProjectDto item, CancellationToken cancellationToken)
        => Ok(await _projectService.CreateAsync(item with { UserId = CurrentUserId }, cancellationToken));


    /// <summary>
    /// update name,ip,website of a project
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateProjectDto item, CancellationToken cancellationToken)
        => Ok(await _projectService.UpdateAsync(item with { UserId = CurrentUserId }, cancellationToken));


    /// <summary>
    /// Get all user's project with userId
    /// </summary>
    /// <param name="userId">Primary key of a user</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        => Ok(await _projectService.GetAllAsync(CurrentUserId, cancellationToken));

    /// <summary>
    /// Change State of a project
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ChangeStateAsync([FromBody] UpdateProjectDto project, CancellationToken cancellationToken)
        => Ok(await _projectService.ChangeStateAsync(CurrentUserId, project.Id, cancellationToken));
}
