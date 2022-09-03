namespace Avn.Domain.Dtos;
public record ProjectDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; }
    public string SourceIp { get; set; }
    public string Website { get; set; }
    public string ApiKey { get; set; }
}