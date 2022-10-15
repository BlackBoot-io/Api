namespace Avn.Services.Dtos;

public class CreateSubscriptionDto
{
    public Guid? UserId { get; set; }

    public int PricingId { get; set; }
    public int? TransactionId { get; set; }

    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
