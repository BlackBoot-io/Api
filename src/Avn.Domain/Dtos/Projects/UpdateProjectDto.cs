namespace Avn.Domain.Dtos;

public record UpdateProjectDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public string SourceIp { get; init; }
    public string Website { get; init; }
}
