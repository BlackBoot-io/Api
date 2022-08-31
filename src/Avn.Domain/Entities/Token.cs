namespace Avn.Domain.Entities;

[Table(nameof(Token), Schema = nameof(EntitySchema.Base))]
public class Token : IEntity
{
    public Token() => Id = Guid.NewGuid();
    [Key]
    public Guid Id { get; set; }
   
    [ForeignKey(nameof(EventId))]
    public virtual Event Event { get; set; }
    public int EventId { get; set; }

    public int ContractTokenId { get; set; }

    public bool IsMinted { get; set; }
    public bool IsBurned { get; set; }

    [Required]
    [MaxLength(150)]
    [Column(TypeName = "varchar")]
    public string OwerWalletAddress { get; set; }

    [Required]
    [MaxLength(15)]
    [Column(TypeName = "varchar")]
    public string UniqueCode { get; set; }

    public DateTime InsertDate { get; set; }
}
