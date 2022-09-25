namespace Avn.Services.Implementations;

public class ProjectsService : IProjectsService
{
    private readonly IAppUnitOfWork _uow;

    public ProjectsService(IAppUnitOfWork uow) => _uow = uow;

    /// <summary>
    /// Get all user's project in order
    /// </summary>
    /// <param name="userid">userId which automatic binded to apis</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<IEnumerable<object>>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        => new ActionResponse<IEnumerable<object>>(await _uow.ProjectRepo.Queryable()
                  .Where(X => X.UserId == userId)
                  .OrderBy(X => X.InsertDate)
                  .AsNoTracking()
                  .Select(s => new
                  {
                      s.Id,
                      s.Name,
                      s.SourceIp,
                      s.Website,
                      s.IsActive
                  }).ToListAsync(cancellationToken));

    /// <summary>
    /// Create a new project
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<Guid>> CreateAsync(CreateProjectDto item, CancellationToken cancellationToken = default)
    {
        var model = new Project
        {
            UserId = item.UserId,
            Name = item.Name,
            SourceIp = item.SourceIp,
            Website = item.Website,
            InsertDate = DateTime.Now,
            IsActive = true
        };
        await _uow.ProjectRepo.AddAsync(model, cancellationToken);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError, BusinessMessage.AddProjectOperationFail);

        return new ActionResponse<Guid>(model.Id);
    }
    /// <summary>
    /// update existing project by user
    /// </summary>
    /// <param name="model"> new data that provided by users</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<Guid>> UpdateAsync(UpdateProjectDto item, CancellationToken cancellationToken = default)
    {
        var model = await _uow.ProjectRepo.Queryable().FirstOrDefaultAsync(x => x.Id == item.Id && x.UserId == item.UserId, cancellationToken);
        if (model is null)
            return new ActionResponse<Guid>(ActionResponseStatusCode.NotFound, BusinessMessage.RecordNotFound);

        model.Name = item.Name;
        model.Website = item.Website;
        model.SourceIp = item.SourceIp;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError, BusinessMessage.AddProjectOperationFail);

        return new ActionResponse<Guid>(model.Id);
    }

    /// <summary>
    /// Disable/Enable a project by user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="projectId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<Guid>> ChangeStateAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default)
    {
        var model = await _uow.ProjectRepo.Queryable().FirstOrDefaultAsync(x => x.Id == projectId && x.UserId == userId, cancellationToken);
        if (model is null)
            return new ActionResponse<Guid>(ActionResponseStatusCode.NotFound, BusinessMessage.RecordNotFound);

        model.IsActive = !model.IsActive;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError, BusinessMessage.AddProjectOperationFail);

        return new ActionResponse<Guid>(model.Id);
    }
}