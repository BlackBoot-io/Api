// <copyright file="ApiKeyAuthenticationHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Security.Claims;
using System.Text.Encodings.Web;
using Avn.Services.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Avn.Api.Core.Authentication;

/// <summary>
/// This is ApiKeyAuthentication Handler.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly IApiKeyService apiKeyService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthenticationHandler"/> class.
    /// This is ApiKeyAuthentication Handler Constructor.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="logger">Logger Instance.</param>
    /// <param name="encoder">Url Encoder.</param>
    /// <param name="clock">System Clock.</param>
    /// <param name="apiKeyService">ApiKey Service.</param>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyService apiKeyService)
        : base(options, logger, encoder, clock) => this.apiKeyService = apiKeyService;

    /// <inheritdoc/>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var apiKey) || apiKey is not { Count: 1 } || !Guid.TryParse(apiKey[0], out var key))
            return AuthenticateResult.Fail(BusinessMessage.InvalidUser);

        var userInfo = await apiKeyService.VerifyAsync(key);

        if (userInfo is { IsSuccess: false })
            return AuthenticateResult.Fail(BusinessMessage.InvalidUser);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.Data.UserId.ToString()),
            new Claim(ClaimTypes.Email, userInfo.Data.Email),
            new Claim(ClaimTypes.Name, userInfo.Data.FullName),
            new Claim("ProjectName", userInfo.Data.ProjectName),
            new Claim("ProjectId", userInfo.Data.ProjectId.ToString()),
        };
        var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.AuthenticationScheme);
        var identities = new List<ClaimsIdentity> { identity };
        var principal = new ClaimsPrincipal(identities);
        var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.AuthenticationScheme);

        return AuthenticateResult.Success(ticket);
    }
}
