namespace Avn.Domain.Entities;

[Table(nameof(Network), Schema = nameof(EntitySchema.Base))]
public class Network : IEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(50)]
    [Required]
    public string Name { get; set; }

    [Required]
    public NetworkType NetworkType { get; set; }

    [Required]
    public decimal GasFee { get; set; }

    [Required]
    public decimal Wages { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Event> Events { get; set; }
}
