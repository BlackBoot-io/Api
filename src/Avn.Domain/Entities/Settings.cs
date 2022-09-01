namespace Avn.Domain.Entities;

[Table(nameof(Settings), Schema = nameof(EntitySchema.Base))]
public class Settings
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public BlockchainNetwork NetworkForPayment { get; set; }

    [Required]
    public int DiscountForYearlySubscription { get; set; }

}
