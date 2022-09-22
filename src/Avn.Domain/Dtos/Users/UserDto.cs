namespace Avn.Domain.Dtos;

public record UserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public bool EmailIsApproved { get; set; }
    public string FullName { get; set; } = default!;
    public UserType UserType { get; set; }
    public string OrganizationName { get; set; } = default!;
    public string WalletAddress { get; set; }
}
