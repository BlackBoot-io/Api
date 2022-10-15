namespace Avn.Services.Dtos;

public class ApiKeyDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; }
}
