#nullable disable
using Avn.Shared.Utilities;
using System.Linq;

namespace Avn.Services.Implementations;

public class UsersService : IUsersService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IVerificationsService _verificationService;
    private readonly ISubscriptionsService _subscriptionsService;

    public UsersService(IAppUnitOfWork uow,
                        IVerificationsService verificationService,
                        ISubscriptionsService subscriptionsService)
    {
        _uow = uow;
        _verificationService = verificationService;
        _subscriptionsService = subscriptionsService;
    }

    /// <summary>
    /// Get Current UserData
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo.Queryable()
                        .Where(X => X.UserId == userId)
                        .Select(X => new UserDto
                        {
                            Email = X.Email,
                            EmailIsApproved = X.EmailIsApproved,
                            Type = X.Type.ToString(),
                            OrganizationName = X.OrganizationName,
                            IsActive = X.IsActive,
                            WalletAddress = X.WalletAddress,
                            FullName = X.FullName
                        })
                        .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
            return new ActionResponse<UserDto>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidUser);

        return new ActionResponse<UserDto>(user);
    }

    /// <summary>
    /// Add User Into Database
    /// </summary>
    /// <param name="user">user Model</param>
    /// <param name="cancellationToken"></param>
    /// <returns>User Id </returns>
    public async Task<IActionResponse<Guid>> SignUpAsync(UserSignUpDto user, CancellationToken cancellationToken = default)
    {
        using var transaction = await _uow.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var passwordSalt = RandomStringGenerator.Generate(10);
            User model = new()
            {
                Email = user.Email,
                FullName = user.FullName,
                IsActive = true,
                OrganizationName = user.OrganizationName,
                Password = HashGenerator.Hash(user.Password, passwordSalt),
                PasswordSalt = passwordSalt,
                Type = user.Type,
                WalletAddress = user.WalletAddress,
                EmailIsApproved = false
            };
            
            await _uow.UserRepo.AddAsync(model, cancellationToken);
            var dbResult = await _uow.SaveChangesAsync(cancellationToken);
            if (!dbResult.IsSuccess)
                return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError, dbResult.Message);
            #region Add Basic Subsciption
            var basicPricing = await _uow.PricingRepo.Queryable().FirstOrDefaultAsync(x => x.IsFree, cancellationToken);
            if (basicPricing is null)
                return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError);
            var subscriptionResult = await _subscriptionsService.AddAsync(new()
            {
                PricingId = basicPricing.Id,
                UserId = model.UserId
            }, cancellationToken);
            if (!subscriptionResult.IsSuccess)
                return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError);
            #endregion
            await _verificationService.SendOtpAsync(model.UserId, TemplateType.EmailVerification, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return new ActionResponse<Guid>(model.UserId);
        }
        catch (DbUpdateException ex)
        {
            transaction.Rollback();
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError, BusinessMessage.DouplicateEmail);
        }
    }

    public async Task<IActionResponse<Guid>> UpdateProfileAsync(Guid userId, UpdateUserDto userDto, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo.FindAsync(userId, cancellationToken);
        if (user is null)
            return new ActionResponse<Guid>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidUser);

        user.WalletAddress = userDto.WalletAddress;
        user.FullName = userDto.FullName;
        user.OrganizationName = userDto.OrganizationName;
        user.Type = userDto.Type;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<Guid>(ActionResponseStatusCode.ServerError, dbResult.Message);

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
        var verificationResult = await _verificationService.VerifyAsync(userId, uniqueCode, TemplateType.EmailVerification, cancellationToken);
        if (!verificationResult.IsSuccess)
            return verificationResult;

        var result = await _uow.UserRepo.FindAsync(userId, cancellationToken);
        if (result is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidPrameter);

        result.EmailIsApproved = true;
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, dbResult.Message);

        return new ActionResponse<bool>(true);
    }

    /// <summary>
    /// Resend Code fot email activation
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> ResendEmailActivationCode(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetCurrentUserAsync(userId, cancellationToken);

        if (user is null || user.Data.EmailIsApproved)
            return new ActionResponse<bool>(ActionResponseStatusCode.BadRequest, BusinessMessage.UserIsActive);

        return await _verificationService.SendOtpAsync(userId, TemplateType.EmailVerification, cancellationToken);
    }

    /// <summary>
    /// enable Lockout 
    /// </summary>
    /// <param name="userId">user Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse> LockAsync(Guid userId, DateTime endDateUTC, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo.FindAsync(userId, cancellationToken);
        if (user is null)
            return new ActionResponse(ActionResponseStatusCode.BadRequest, BusinessMessage.NotFound);

        if (user is { IsLockoutEnabled: true })
            return new ActionResponse(ActionResponseStatusCode.BadRequest, BusinessMessage.NotFound);

        user.IsLockoutEnabled = true;
        user.LockoutEndDateUtc = endDateUTC;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, dbResult.Message);

        return new ActionResponse();


    }
    /// <summary>
    /// disable Lockout
    /// </summary>
    /// <param name="userId">user Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse> UnLockAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo.FindAsync(userId, cancellationToken);
        if (user is null)
            return new ActionResponse(ActionResponseStatusCode.BadRequest, BusinessMessage.NotFound);

        if (user is { IsLockoutEnabled: false })
            return new ActionResponse(ActionResponseStatusCode.BadRequest, BusinessMessage.NotFound);

        user.IsLockoutEnabled = false;
        user.LockoutEndDateUtc = null;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, dbResult.Message);

        return new ActionResponse();
    }

    public async Task<IActionResponse<bool>> CheckLockoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo.FindAsync(userId, cancellationToken);
        if (user is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.BadRequest, BusinessMessage.NotFound);

        if (user is  { IsLockoutEnabled: false } )
            return new ActionResponse<bool>(false);


        if (user.LockoutEndDateUtc < DateTime.UtcNow)
        {
            await UnLockAsync(userId, cancellationToken);
            return new ActionResponse<bool>(false);
        }
        else
            return new ActionResponse<bool>(true);


    }
}