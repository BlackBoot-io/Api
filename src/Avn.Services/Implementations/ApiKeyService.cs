namespace Avn.Services.Implementations;

public class ApiKeyService : IApiKeyService
{
    private readonly IAppUnitOfWork _uow;

    public ApiKeyService(IAppUnitOfWork uow) => _uow = uow;

    /// <summary>
    /// Check if the api key is valid
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>collection of user information </returns>
    public async Task<IActionResponse<ApiKeyDto>> VerifyAsync(Guid apiKey, CancellationToken cancellationToken = default)
    {
        var project = await _uow.ProjectRepo.AsNoTracking().Include(x => x.User).Select(row => new ApiKeyDto
        {
            UserId = row.UserId,
            FullName = row.User.FullName,
            Email = row.User.Email,
            ProjectId = row.Id,
            ProjectName = row.Name

        }).FirstOrDefaultAsync(x => x.ProjectId == apiKey, cancellationToken);
        if (project is null)
            return new ActionResponse<ApiKeyDto>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        return new ActionResponse<ApiKeyDto>(project);
    }
}