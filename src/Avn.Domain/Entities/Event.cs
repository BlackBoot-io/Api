namespace Avn.Domain.Entities;

[Table(nameof(Event), Schema = nameof(EntitySchema.Base))]
public class Event : IEntity
{

    public Event() => Code = Guid.NewGuid();


    [Key]
    public int Id { get; set; }

    public Guid Code { get; set; }

    public Guid? ProjectId { get; set; }
    [ForeignKey(nameof(ProjectId))]
    public virtual Project Project { get; set; }

    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(50)]
    public string Description { get; set; }

    public TemplateType TemplateType { get; set; }
    public DeliveryType DeliveryType { get; set; }


    public int NetworkId { get; set; }
    [ForeignKey(nameof(NetworkId))]
    public virtual Network Network { get; set; }

    [MaxLength(100)]
    public string EventUri { get; set; }

    [MaxLength(50)]
    public string Location { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime ExpireDate { get; set; }

    public bool IsVirtual { get; set; }
    public bool IsPrivate { get; set; }


    public ICollection<Token> Tokens { get; set; }

}
