namespace Avn.Domain.Dtos;

public record CreateProjectDto
{
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public string SourceIp { get; init; }
    public string Website { get; init; }
}
