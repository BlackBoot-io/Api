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
    public int UsdtAmount { get; set; }

    [Required]
    public int DiscountForYearlySubscription { get; set; }

    [Required]
    public int RequestsPerSecond { get; set; }

    [Required]
    public int RequestsPerDay { get; set; }

    [Required]
    public int TokenPerDay { get; set; }

    public bool PublicDocumentation { get; set; }

    public bool TicketsSupport { get; set; }

    public bool PriorityTicketsSupport { get; set; }

    public bool HasTransactionWages { get; set; }

    public bool IsActive { get; set; }
    public bool IsFree { get; set; }

    public ICollection<NetworkInPricing> NetworkInPricings { get; set; }
}