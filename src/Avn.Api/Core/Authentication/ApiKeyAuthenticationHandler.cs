using Avn.Services.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Avn.Api.Core.Authentication;
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyService _apiKeyService;
    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
                                       ILoggerFactory logger,
                                       UrlEncoder encoder,
                                       ISystemClock clock,
                                       IApiKeyService apiKeyService) : base(options, logger, encoder, clock) => _apiKeyService = apiKeyService;


    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var apiKey) || apiKey is not { Count: 1 } || !Guid.TryParse(apiKey[0], out var key))
            return AuthenticateResult.Fail(BusinessMessage.InvalidUser);


        var userInfo = await _apiKeyService.VerifyAsync(key);
        if (userInfo is { IsSuccess: false })
            return AuthenticateResult.Fail(BusinessMessage.InvalidUser);

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier,userInfo.Data.UserId.ToString()),
            new Claim(ClaimTypes.Email,userInfo.Data.Email),
            new Claim(ClaimTypes.Name,userInfo.Data.FullName),
            new Claim("ProjectName",userInfo.Data.ProjectName),
            new Claim("ProjectId",userInfo.Data.ProjectId.ToString())
        };
        var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.AuthenticationScheme);
        var identities = new List<ClaimsIdentity> { identity };
        var principal = new ClaimsPrincipal(identities);
        var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.AuthenticationScheme);

        return AuthenticateResult.Success(ticket);
    }
}
