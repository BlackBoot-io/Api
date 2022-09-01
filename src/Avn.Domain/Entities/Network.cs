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
    [Column(TypeName = "decimal(21,9)")]
    public decimal GasFee { get; set; }

    [Required]
    [Column(TypeName = "decimal(21,9)")]
    public decimal Wages { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Event> Events { get; set; }
}
