namespace Avn.Api.Controllers;

public class ProjectsController : BaseController
{
    private readonly IProjectsService _projectService;
    public ProjectsController(IProjectsService projectService) => _projectService = projectService;

    [HttpPost]
    public async Task<IActionResult> AddAsync(Guid userId, [FromBody] CreateProjectDto item)
    {
        item.UserId = userId;
        return Ok(await _projectService.CreateAsync(item));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAsync(Guid userId, [FromBody] UpdateProjectDto item)
    {
        item.UserId = userId;
        return Ok(await _projectService.UpdateAsync(item));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(Guid userId)
        => Ok(await _projectService.GetAllAsync(userId));
}
