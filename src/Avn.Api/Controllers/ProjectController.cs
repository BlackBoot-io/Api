using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Avn.Api.Controllers
{
    public class ProjectController : BaseController
    {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService) => _projectService = projectService;

        [HttpPost]
        public async Task<IActionResult> AddAsync(Guid userId, [FromBody] Project item)
        {
            item.UserId = userId;
            return Ok(await _projectService.CreateAsync(item));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(Guid userId, [FromBody] Project item)
        {
            item.UserId = userId;
            return Ok(await _projectService.UpdateAsync(item));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(Guid userId)
            => Ok(await _projectService.GetAllAsync(userId));
    }
}
