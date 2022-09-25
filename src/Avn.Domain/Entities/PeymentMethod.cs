namespace Avn.Domain.Entities;

[Table(nameof(PaymentMethod), Schema = nameof(EntitySchema.Base))]

public class PaymentMethod : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Coin { get; set; }

    public byte Discount { get; set; }

    public bool IsActive { get; set; }

}
