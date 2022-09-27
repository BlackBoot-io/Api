using System.Text.Json.Serialization;

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
    public NetworkType Type { get; set; }

    [Required]
    [Column(TypeName = ColumnType.Decimal219)]
    public decimal GasFee { get; set; }

    [Required]
    [Column(TypeName = ColumnType.Decimal219)]
    public decimal Wages { get; set; }

    [Required]
    public string SmartContractAddress { get; set; }

    public bool IsActive { get; set; }

    public bool IsDefault { get; set; }

    [JsonIgnore]
    public ICollection<Drop> Drops { get; set; }

    [JsonIgnore]
    public ICollection<NetworkInPricing> NetworkInPricings { get; set; }
}
