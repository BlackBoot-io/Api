namespace Avn.Services.Interfaces;

public interface IVerificationService : IScopedDependency
{
    Task<IActionResponse<bool>> VerifyAsync(Guid userId, string uniqueCode, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> SendOtpAsync(User user, VerificationType type, CancellationToken cancellationToken = default);
}
