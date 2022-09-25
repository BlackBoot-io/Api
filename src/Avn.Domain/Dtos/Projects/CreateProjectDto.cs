namespace Avn.Domain.Dtos;

public class CreateProjectDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string SourceIp { get; set; }
    public string Website { get; set; }
}
