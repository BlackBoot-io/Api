namespace Avn.Domain.Dtos;

public record UpdateDropDto
{
    public Guid Code { get; set; }
    public string Name { get; set; }

}
