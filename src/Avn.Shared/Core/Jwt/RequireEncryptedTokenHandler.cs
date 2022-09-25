using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Avn.Shared.Core;

public class RequireEncryptedTokenHandler : JwtSecurityTokenHandler
{
    public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentNullException(nameof(token));

        if (validationParameters is null)
            throw new ArgumentNullException(nameof(validationParameters));

        if (token.Length > MaximumTokenSizeInBytes)
            throw new ArgumentException(
                $"IDX10209: token has length: '{token.Length}' which is larger than the MaximumTokenSizeInBytes: '{MaximumTokenSizeInBytes}'.");

        var strArray = token.Split(new[] { '.' }, 6);
        if (strArray.Length == 5)
            return base.ValidateToken(token, validationParameters, out validatedToken);

        throw new SecurityTokenDecryptionFailedException();
    }
}
