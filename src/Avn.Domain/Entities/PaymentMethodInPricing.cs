namespace Avn.Domain.Entities;

[Table(nameof(PaymentMethodInPricing), Schema = nameof(EntitySchema.Base))]
public class PaymentMethodInPricing
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(PaymentMethodId))]
    public PaymentMethod PaymentMethod { get; set; }
    public int PaymentMethodId { get; set; }

    [ForeignKey(nameof(PricingId))]
    public Pricing Pricing { get; set; }
    public int PricingId { get; set; }
}