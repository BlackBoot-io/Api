namespace Avn.Domain.Dtos;

public class UserTokenDto
{
    public string AccessToken { get; set; }
    public DateTimeOffset AccessTokenExpireTime { get; set; }

    public string RefreshToken { get; set; }
    public DateTimeOffset RefreshTokenExpireTime { get; set; }

    public UserDto User { get; set; }
}

