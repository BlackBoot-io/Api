namespace Avn.Domain.Dtos;

public record UserDto
{
 
    public string Email { get; set; }
    public bool EmailIsApproved { get; set; }
    public string FullName { get; set; } = default!;
    public UserType UserType { get; set; }
    public string OrganizationName { get; set; } = default!;
    public string WalletAddress { get; set; }
}

public record UserSignUpDto : UserDto
{
    public string Password { get; set; }
}