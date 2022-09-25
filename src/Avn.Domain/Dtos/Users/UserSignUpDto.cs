namespace Avn.Domain.Dtos;
public record UserSignUpDto
{
    public string Email { get; set; }
    public string FullName { get; set; } = default!;
    public string OrganizationName { get; set; } = default!;
    public string WalletAddress { get; set; }
    public string Password { get; set; }
    public UserType Type { get; set; }
}