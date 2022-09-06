using Avn.Data.UnitofWork;

namespace Avn.Services.Implementations;
public class ProjectsService : IProjectsService
{
    private readonly IAppUnitOfWork _uow;

    public ProjectsService(IAppUnitOfWork uow) => _uow = uow;

    /// <summary>
    /// Get all user's project
    /// </summary>
    /// <param name="userid">userId which automatic binded to apis</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<IEnumerable<Project>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
       => new ActionResponse<IEnumerable<Project>>(await _uow.ProjectRepo.GetAll()
           .Where(X => X.UserId == userId)
           .AsNoTracking()
           .ToListAsync(cancellationToken));

    /// <summary>
    /// Create a new project
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<Project>> CreateAsync(Project model, CancellationToken cancellationToken = default)
    {
        model.ApiKey = Guid.NewGuid().ToString();
        await _uow.ProjectRepo.AddAsync(model, cancellationToken);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Project>(ActionResponseStatusCode.ServerError, BusinessMessage.AddProjectOperationFail);
        return new ActionResponse<Project>(model);
    }


    public async Task<IActionResponse<Project>> UpdateAsync(Project item, CancellationToken cancellationToken = default)
    {
        var project = await _uow.ProjectRepo.GetAll().FirstOrDefaultAsync(x => x.Id == item.Id);
        if (project == null)
            return new ActionResponse<Project>(ActionResponseStatusCode.NotFound, BusinessMessage.RecordNotFound);

        project.Name = item.Name;
        project.Website = item.Website;
        project.SourceIp = item.SourceIp;
        project.ApiKey = item.ApiKey;

        var dbResult = await _uow.SaveChangesAsync();

        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Project>(ActionResponseStatusCode.ServerError, BusinessMessage.AddProjectOperationFail);

        return new ActionResponse<Project>(new Project
        {
            Name = project.Name,
            SourceIp = project.SourceIp,
            UserId = project.UserId,
            Website = project.Website,
            ApiKey = project.ApiKey,
        });
    }
}
