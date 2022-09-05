#nullable disable
namespace Avn.Shared.Core;

public record JwtSettings
{
    public string Key { set; get; }
    public string EncryptionKey { set; get; }
    public string Issuer { set; get; }
    public string Audience { set; get; }
    public int AccessTokenExpirationMinutes { set; get; }
    public int RefreshTokenExpirationMinutes { set; get; }
}
