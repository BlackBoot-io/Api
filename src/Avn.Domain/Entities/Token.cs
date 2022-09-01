namespace Avn.Domain.Entities;

[Table(nameof(Token), Schema = nameof(EntitySchema.User))]
public class Token : IEntity
{
    public Token() => Id = Guid.NewGuid();

    [Key]
    public Guid Id { get; set; }
   
    [ForeignKey(nameof(DropId))]
    public virtual Drop Drop { get; set; }
    public int DropId { get; set; }

    public int ContractTokenId { get; set; }

    public bool IsMinted { get; set; }
    public bool IsBurned { get; set; }

    [Required]
    [MaxLength(150)]
    public string OwerWalletAddress { get; set; }

    [Required]
    [MaxLength(15)]
    public string UniqueCode { get; set; }

    public DateTime InsertDate { get; set; }
}
