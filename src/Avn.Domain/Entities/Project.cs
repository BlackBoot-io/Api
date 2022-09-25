namespace Avn.Domain.Entities;

[Table(nameof(Project), Schema = nameof(EntitySchema.User))]
public class Project : IEntity
{
    public Project() => Id = Guid.NewGuid();

    /// <summary>
    /// Use this field as an Api-Key
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public Guid UserId { get; set; }

    [MaxLength(50)]
    [Required]
    public string Name { get; set; }

    [Required]
    [MaxLength(15)]
    public string SourceIp { get; set; }

    [MaxLength(50)]
    public string Website { get; set; }

    public bool IsActive { get; set; }

    public DateTime InsertDate { get; set; }
    public ICollection<Drop> Drops { get; set; }
}
