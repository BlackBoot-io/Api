namespace Avn.Domain.Entities;

[Table(nameof(Subscription), Schema = nameof(EntitySchema.User))]
public class Subscription : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public Guid? UserId { get; set; }

    [ForeignKey(nameof(PricingId))]
    public Pricing Pricing { get; set; }
    public int PricingId { get; set; }


    [ForeignKey(nameof(TransactionId))]
    public Transaction Transaction { get; set; }
    public int TransactionId { get; set; }

    public DateTime From { get; set; }

    public DateTime To { get; set; }
}
