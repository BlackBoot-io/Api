using Avn.Shared.Utilities;

namespace Avn.Services.Implementations;
public class VerificationsService : IVerificationsService
{
    private readonly IAppUnitOfWork _uow;
    private readonly Lazy<INotificationsService> _notificationService;
    public VerificationsService(IAppUnitOfWork uow, Lazy<INotificationsService> notificationService)
    {
        _uow = uow;
        _notificationService = notificationService;
    }

    /// <summary>
    /// send otp to users by channels (verificationType)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

        _uow.VerificationCodeRepo.Add(verification);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, dbResult.Message);

        await _notificationService.Value.SendAsync(userId,
                                                   new()
                                                   {
                                                       { "UniqueCode", verification.UniqueCode },
                                                   }, type);
        return new ActionResponse<bool>(true);
    }

    /// <summary>
    /// Verify uniqueCode
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="uniqueCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> VerifyAsync(Guid userId, string uniqueCode, TemplateType type, CancellationToken cancellationToken = default)
    {
        var verification = await _uow.VerificationCodeRepo
                                
                                .FirstOrDefaultAsync(X => X.UserId == userId &&
                                                     X.UniqueCode == uniqueCode &&
                                                     !X.IsUsed &&
                                                     X.Type == type &&
                                                     X.ExpirationTime > DateTime.Now, cancellationToken);

        if (verification is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidPrameter);

        verification.IsUsed = true;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, dbResult.Message);

        return new ActionResponse<bool>(true);
    }
}