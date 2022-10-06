namespace Avn.Domain.Entities;

[Table(nameof(NetworkInPricing), Schema = nameof(EntitySchema.Base))]
public class NetworkInPricing : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(NetworkId))]
    public Network Network { get; set; }
    public int NetworkId { get; set; }

    [ForeignKey(nameof(PricingId))]
    public Pricing Pricing { get; set; }
    public int PricingId { get; set; }

}

