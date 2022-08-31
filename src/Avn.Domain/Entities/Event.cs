namespace Avn.Domain.Entities;

[Table(nameof(Event), Schema = nameof(EntitySchema.Base))]
public class Event : IEntity
{
    public Event() => Code = Guid.NewGuid();

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public Guid UserId { get; set; }

    public Guid? ProjectId { get; set; }
    [ForeignKey(nameof(ProjectId))]
    public Project Project { get; set; }

    [ForeignKey(nameof(NetworkId))]
    public Network Network { get; set; }
    public int NetworkId { get; set; }

    [MaxLength(50)]
    [Required]
    public string Name { get; set; }

    public DeliveryType DeliveryType { get; set; }
    public EventStatus EventStatus { get; set; } = EventStatus.Pending;

    [MaxLength(100)]
    [Required]
    public string EventUri { get; set; }

    [MaxLength(50)]
    [Required]
    public string Location { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime ExpireDate { get; set; }

    public bool IsVirtual { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsActive { get; set; }

    [Required]
    public Guid Code { get; set; }

    [Required]
    public decimal GasFee { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    public DateTime InsertDate { get; set; }
    public ICollection<Token> Tokens { get; set; }

}
