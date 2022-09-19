namespace Avn.Domain.Dtos;

public record CreateDropDto
{
    public Guid? ProjectId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Count { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public DropCategoryType CategotyType { get; set; }
    public int NetworkId { get; set; }
    public string Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsVirtual { get; set; }
    public bool IsPrivate { get; set; }
    public byte[] File { get; set; }
}
