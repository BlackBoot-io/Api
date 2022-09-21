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
    public async Task<IActionResult> AddAsync([FromBody] CreateProjectDto item)
    {
        item.UserId = CurrentUserId;
        return Ok(await _projectService.CreateAsync(item));
    }

    /// <summary>
    /// update name,ip,website of a project
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateProjectDto item)
    {
        item.UserId = CurrentUserId;
        return Ok(await _projectService.UpdateAsync(item));
    }

    /// <summary>
    /// Get all user's project with userId
    /// </summary>
    /// <param name="userId">Primary key of a user</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
        => Ok(await _projectService.GetAllAsync(CurrentUserId));
}
