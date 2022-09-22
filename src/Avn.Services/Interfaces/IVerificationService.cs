namespace Avn.Services.Interfaces;

public interface IVerificationService : IScopedDependency
{
    /// <summary>
    /// Verify uniqueCode
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="uniqueCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<bool>> VerifyAsync(Guid userId, string uniqueCode, CancellationToken cancellationToken = default);
   
    /// <summary>
    /// send otp to users by channels (verificationType)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<bool>> SendOtpAsync(Guid userId, TemplateType type, CancellationToken cancellationToken = default);
}
