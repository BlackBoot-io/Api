namespace Avn.Domain.Dtos;

public record DropDto
{
    public int Id { get; set; }

    public Guid Code { get; set; }

    public Guid? ProjectId { get; set; }

    public string Project { get; set; }

    public Guid UserId { get; set; }

    public string User { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DeliveryType DeliveryType { get; set; }

    public int NetworkId { get; set; }

    public string Network { get; set; }

    public string DropUri { get; set; }

    public string ContentId { get; set; }
    public string Location { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime ExpireDate { get; set; }

    public bool IsVirtual { get; set; }

    public bool IsPrivate { get; set; }
}
