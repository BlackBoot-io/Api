namespace Avn.Domain.Entities;

[Table(nameof(Project), Schema = nameof(EntitySchema.Base))]
public class Project : IEntity
{
    public Project() => Id = Guid.NewGuid();

    [Key]
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(50)]
    public string SourceIp { get; set; }


    [MaxLength(50)]
    public string Website { get; set; }

    public string ApiKey { get; set; }


    public ICollection<Event> Events { get; set; }

}
