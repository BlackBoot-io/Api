using Microsoft.AspNetCore.Authentication;

namespace Avn.Api.Core.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string AuthenticationScheme = "ApiKey";
    public const string HeaderName = "api-key";
}