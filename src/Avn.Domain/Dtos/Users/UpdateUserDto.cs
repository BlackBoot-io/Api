namespace Avn.Domain.Dtos;

public class UpdateUserDto
{
    public string FullName { get; set; } = default!;
    public UserType Type { get; set; }
    public string OrganizationName { get; set; } = default!;
    public string WalletAddress { get; set; }
}
