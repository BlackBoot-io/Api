namespace Avn.Services.Implementations;
public class VerificationService : IVerificationService
{
    private readonly IAppUnitOfWork _uow;
    public VerificationService(IAppUnitOfWork uow) => _uow = uow;

    public Task<IActionResponse<bool>> SendOtpAsync(UserDto user, TemplateType type, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResponse<bool>> SendOtpAsync(Guid userId, TemplateType type, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IActionResponse<bool>> VerifyAsync(Guid userId, string uniqueCode, CancellationToken cancellationToken = default)
    {
        var verification = await _uow.VerificationCodeRepo
            .Queryable()
            .FirstOrDefaultAsync(X => X.UserId == userId &&
                                 X.UniqueCode == uniqueCode &&
                                 !X.IsUsed &&
                                 X.ExpirationTime > DateTime.Now, cancellationToken);

        if (verification is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidPrameter);

        verification.IsUsed = true;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        return new ActionResponse<bool> { IsSuccess = true, Data = true };
    }
}