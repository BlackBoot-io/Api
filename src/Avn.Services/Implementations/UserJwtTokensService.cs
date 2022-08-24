#nullable disable
using Avn.Data.UnitofWork;
using Avn.Domain.Entities;
using Avn.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BlackBoot.Services.Implementations;

public class UserJwtTokensService : IUserJwtTokensService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IAppUnitOfWork _uow;
    public UserJwtTokensService(IAppUnitOfWork uow, IConfiguration configuration)
    {
        _uow = uow;
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
    }

    public async Task<IActionResponse> AddUserTokenAsync(Guid userId, string accessToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        var model = new UserJwtToken
        {
            UserId = userId,
            AccessTokenHash = HashGenerator.Hash(accessToken),
            AccessTokenExpiresTime = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            RefreshTokenHash = HashGenerator.Hash(refreshToken),
            RefreshTokenExpiresTime = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpirationMinutes)
        };
        await DeleteTokensWithSameRefreshTokenAsync(userId, refreshToken, cancellationToken);
        await _uow.UserJwtTokenRepo.AddAsync(model, cancellationToken);

        var dbResult = await _uow.SaveChangesAsync();
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse(ActionResponseStatusCode.ServerError);

        return new ActionResponse();
    }
    public async Task<IActionResponse> RevokeUserTokensAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(refreshToken))
            await DeleteTokensWithSameRefreshTokenAsync(userId, refreshToken, cancellationToken);
        await DeleteExpiredTokensAsync(cancellationToken);


        var dbResult = await _uow.SaveChangesAsync();
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse(ActionResponseStatusCode.ServerError);

        return new ActionResponse();
    }
    public async Task<IActionResponse<bool>> VerifyTokenAsync(Guid userId, string accessToken, CancellationToken cancellationToken = default)
    {
        var hashesAccessToken = HashGenerator.Hash(accessToken);
        var token = await _uow.UserJwtTokenRepo.GetAll().AsNoTracking().Where(x => x.UserId == userId && x.AccessTokenHash == hashesAccessToken).FirstOrDefaultAsync(cancellationToken);
        return new ActionResponse<bool>(token != null && token.AccessTokenExpiresTime >= DateTime.Now);
    }
    public async Task<IActionResponse<UserJwtToken>> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return new ActionResponse<UserJwtToken>(ActionResponseStatusCode.NotFound);

        return new ActionResponse<UserJwtToken>(await _uow.UserJwtTokenRepo.GetAll().Where(x => x.RefreshTokenHash == HashGenerator.Hash(refreshToken)).FirstOrDefaultAsync(cancellationToken));
    }

    private async Task DeleteTokensWithSameRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;
        var hashedRefreshToken = HashGenerator.Hash(refreshToken);
        var tokens = await _uow.UserJwtTokenRepo.GetAll().Where(t => t.UserId == userId && t.RefreshTokenHash == hashedRefreshToken).ToListAsync(cancellationToken);
        _uow.UserJwtTokenRepo.RemoveRange(tokens);
    }
    private async Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var tokens = await _uow.UserJwtTokenRepo.GetAll().Where(x => x.RefreshTokenExpiresTime < DateTime.Now).ToListAsync(cancellationToken);
        _uow.UserJwtTokenRepo.RemoveRange(tokens);
    }
}
