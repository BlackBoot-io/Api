#nullable disable
namespace Avn.Services.Implementations;

public class UsersService : IUsersService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IVerificationService _verificationService;

    public UsersService(IAppUnitOfWork uow, IVerificationService verificationService)
    {
        _uow = uow;
        _verificationService = verificationService;
    }

    /// <summary>
    /// Get Current UserData
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo.GetAll().FirstOrDefaultAsync(X => X.UserId == userId, cancellationToken);
        if (user is null)
            return new ActionResponse<UserDto>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidUser);

        return new ActionResponse<UserDto>(new UserDto
        {
            Email = user.Email,
            FullName = user.FullName,
            UserType = user.Type,
            OrganizationName = user.OrganizationName,
            WalletAddress = user.WalletAddress,
            EmailIsApproved = user.EmailIsApproved,
        });
    }

    /// <summary>
    /// Add User Into Database
    /// </summary>
    /// <param name="user">user Model</param>
    /// <param name="cancellationToken"></param>
    /// <returns>User Id </returns>
    public async Task<IActionResponse<Guid>> SignUpAsync(User user, CancellationToken cancellationToken = default)
    {
        await _uow.UserRepo.AddAsync(user, cancellationToken);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError);

        await _verificationService.SendOtpAsync(new UserDto
        {
            Email = user.Email,
            FullName = user.FullName,
            UserType = user.Type,
            OrganizationName = user.OrganizationName,
            WalletAddress = user.WalletAddress,
            EmailIsApproved = user.EmailIsApproved
        }, VerificationType.EmailVerification, cancellationToken);
        return new ActionResponse<Guid>(user.UserId);
    }

    public async Task<IActionResponse<Guid>> UpdateProfileAsync(Guid userId, UserDto userDto, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo.FindAsync(userId, cancellationToken);
        if (user is null)
            return new ActionResponse<Guid>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidUser);

        user.WalletAddress = userDto.WalletAddress;
        user.FullName = userDto.FullName;
        user.OrganizationName = userDto.OrganizationName;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError);

        return new ActionResponse<Guid>(user.UserId);
    }


    /// <summary>
    /// Active email from uniqueCode
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="uniqueCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> ActivateEmailAsync(Guid userId, string uniqueCode, CancellationToken cancellationToken = default)
    {
        var verificationResult = await _verificationService.VerifyAsync(userId, uniqueCode, cancellationToken);
        if (!verificationResult.IsSuccess)
            return verificationResult;

        var result = await _uow.UserRepo.FindAsync(userId, cancellationToken);
        if (result is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidPrameter);

        result.EmailIsApproved = true;
        var dbResult = await _uow.SaveChangesAsync();
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError);

        return new ActionResponse<bool>(true);
    }
}