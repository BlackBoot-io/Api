#nullable disable
using Avn.Domain.Entities;
using Avn.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BlackBoot.Services.Implementations;

public class UserJwtTokenService : IUserJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ApplicationDBContext _context;
    private readonly DbSet<UserJwtToken> _userJwtToken;
    public UserJwtTokenService(ApplicationDBContext context, IConfiguration configuration)
    {
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        _context = context;
        _userJwtToken = context.Set<UserJwtToken>();
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
        await _userJwtToken.AddAsync(model, cancellationToken);

        var dbResult = await _context.SaveChangesAsync();
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse(ActionResponseStatusCode.ServerError);
        
        return new ActionResponse();
    }
    public async Task<IActionResponse> RevokeUserTokensAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(refreshToken))
            await DeleteTokensWithSameRefreshTokenAsync(userId, refreshToken, cancellationToken);
        await DeleteExpiredTokensAsync(cancellationToken);


        var dbResult = await _context.SaveChangesAsync();
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse(ActionResponseStatusCode.ServerError);

        return new ActionResponse();
    }
    public async Task<IActionResponse<bool>> VerifyTokenAsync(Guid userId, string accessToken, CancellationToken cancellationToken = default)
    {
        var hashesAccessToken = HashGenerator.Hash(accessToken);
        var token = await _userJwtToken.AsNoTracking().Where(x => x.UserId == userId && x.AccessTokenHash == hashesAccessToken).FirstOrDefaultAsync(cancellationToken);
        return new ActionResponse<bool>(token != null && token.AccessTokenExpiresTime >= DateTime.Now);
    }
    public async Task<IActionResponse<UserJwtToken>> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return new ActionResponse<UserJwtToken>(ActionResponseStatusCode.NotFound);

        return new ActionResponse<UserJwtToken>(await _userJwtToken.Where(x => x.RefreshTokenHash == HashGenerator.Hash(refreshToken)).FirstOrDefaultAsync(cancellationToken));
    }

    private async Task DeleteTokensWithSameRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;
        var hashedRefreshToken = HashGenerator.Hash(refreshToken);
        var tokens = await _userJwtToken.Where(t => t.UserId == userId && t.RefreshTokenHash == hashedRefreshToken).ToListAsync(cancellationToken);
        _userJwtToken.RemoveRange(tokens);
    }
    private async Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var tokens = await _userJwtToken.Where(x => x.RefreshTokenExpiresTime < DateTime.Now).ToListAsync(cancellationToken);
        _userJwtToken.RemoveRange(tokens);
    }
}
