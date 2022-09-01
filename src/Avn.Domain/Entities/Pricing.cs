namespace Avn.Domain.Entities;

[Table(nameof(Pricing), Schema = nameof(EntitySchema.Base))]
public class Pricing
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public string AvailableNetworks { get; set; }

    [Required]
    public int Price { get; set; }

    [Required]
    public int RequestsPerSecond { get; set; }

    public bool IsActive { get; set; }
}
