#nullable disable
using Avn.Data.UnitofWork;

namespace Avn.Services.Implementations;

public class UsersService : IUsersService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IJwtTokensService _jwtTokensService;
    private readonly IVerificationService _verificationService;

    public UsersService(IAppUnitOfWork uow, IJwtTokensService jwtTokensService, IVerificationService verificationService)
    {
        _uow = uow;
        _jwtTokensService = jwtTokensService;
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
            return new ActionResponse<UserDto>(ActionResponseStatusCode.BadRequest, AppResource.InvalidUser);

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

        await _verificationService.SendOtpAsync(user, VerificationType.EmailVerification, cancellationToken);
        return new ActionResponse<Guid>(user.UserId);
    }

    public async Task<IActionResponse<Guid>> UpdateProfileAsync(Guid userId, UserDto userDto, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo.FindAsync(userId, cancellationToken);
        if (user == null)
            return new ActionResponse<Guid>(ActionResponseStatusCode.BadRequest, AppResource.InvalidUser);

        user.WalletAddress = userDto.WalletAddress;
        user.FullName = userDto.FullName;
        user.OrganizationName = userDto.OrganizationName;

        _uow.UserRepo.Update(user);

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError);

        return new ActionResponse<Guid>
        {
            IsSuccess = true,
            Data = user.UserId
        };
    }

    /// <summary>
    /// check username and password then create jwt token
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<UserTokenDto>> LoginAsync(UserLoginDto item, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo.GetAll()
                                      .FirstOrDefaultAsync(X => X.Email == item.Email, cancellationToken);

        if (user is null || user.Password != HashGenerator.Hash(item.Password, user.PasswordSalt)) return new ActionResponse<UserTokenDto> { Message = AppResource.InvalidUsernameOrPassword };
        if (!user.IsActive) return new ActionResponse<UserTokenDto> { Message = AppResource.AccountIsDeActive };

        return await _jwtTokensService.GenerateUserTokenAsync(user, cancellationToken: cancellationToken);
    }

    public async Task<IActionResponse> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
         => await _jwtTokensService.RevokeUserTokensAsync(refreshToken, cancellationToken);

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
            return new ActionResponse<bool>(ActionResponseStatusCode.BadRequest, AppResource.InvalidPrameter);

        result.EmailIsApproved = true;
        var dbResult = await _uow.SaveChangesAsync();
        if (dbResult.ToSaveChangeResult())
            return new ActionResponse<bool>(ActionResponseStatusCode.Success, true);

        return new ActionResponse<bool>(ActionResponseStatusCode.ServerError);
    }


    public async Task<IActionResponse<UserTokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshTokenModel = await _jwtTokensService.GetRefreshTokenAsync(refreshToken, cancellationToken);
        if (refreshTokenModel is null)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, AppResource.InvalidUser);

        var user = await _uow.UserRepo.GetAll()
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(X => X.UserId == refreshTokenModel.UserId, cancellationToken);
        if (user is null)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, AppResource.InvalidUser);

        return await _jwtTokensService.GenerateUserTokenAsync(user, refreshToken, cancellationToken: cancellationToken);
    }

}