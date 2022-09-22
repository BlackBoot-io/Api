using Avn.Shared.Utilities;

namespace Avn.Services.Implementations;
public class VerificationService : IVerificationService
{
    private readonly IAppUnitOfWork _uow;
    private readonly Lazy<INotificationService> _notificationService;
    public VerificationService(IAppUnitOfWork uow, Lazy<INotificationService> notificationService)
    {
        _uow = uow;
        _notificationService = notificationService;
    }

    public async Task<IActionResponse<bool>> SendOtpAsync(Guid userId, TemplateType type, CancellationToken cancellationToken = default)
    {
        VerificationCode verification = new()
        {
            ExpirationTime = DateTime.UtcNow.AddHours(24),
            InsertDateMi = DateTime.UtcNow,
            IsUsed = false,
            Type = type,
            UniqueCode = RandomStringGenerator.Generate(25),
            UserId = userId,
        };

        await _uow.VerificationCodeRepo.AddAsync(verification, cancellationToken);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, BusinessMessage.ServerError);

        Dictionary<string, string> extraData = new()
        {
             

        };

        await _notificationService.Value.SendAsync(userId, extraData, type);
        return new ActionResponse<bool>(ActionResponseStatusCode.Success);
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