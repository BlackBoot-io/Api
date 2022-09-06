namespace Avn.Services.Implementations;
public class VerificationService : IVerificationService
{
    public Task<IActionResponse<bool>> SendOtpAsync(User user, VerificationType type, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResponse<bool>> SendOtpAsync(Guid userId, VerificationType type, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResponse<bool>> VerifyAsync(Guid userId, string uniqueCode, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
