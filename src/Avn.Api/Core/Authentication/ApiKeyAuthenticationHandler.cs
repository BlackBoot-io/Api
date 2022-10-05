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
                                       IApiKeyService apiKeyService) : base(options, logger, encoder, clock)
    {
        _apiKeyService = apiKeyService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var apiKey) || apiKey.Count != 1)
        {
            return AuthenticateResult.Fail("Invalid parameters");
        }

        var userInfo = await _apiKeyService.VerifyAsync(apiKey);

        if (userInfo is { IsSuccess: false })
        {
            return AuthenticateResult.Fail("Invalid parameters");
        }


        var claims = new[] {
            new Claim("","")
        };
        var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.AuthenticationScheme);
        var identities = new List<ClaimsIdentity> { identity };
        var principal = new ClaimsPrincipal(identities);
        var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.AuthenticationScheme);

        return AuthenticateResult.Success(ticket);
    }
}
