namespace Avn.Domain.Entities;

[Table(nameof(Token), Schema = nameof(EntitySchema.Base))]
public class Token : IEntity
{
    public Token() => Id = Guid.NewGuid();
    [Key]
    public Guid Id { get; set; }

    public int EventId { get; set; }
    [ForeignKey(nameof(EventId))]
    public virtual Event Event { get; set; }
    public int ContractTokenId { get; set; }

    public bool Mint { get; set; }
    public bool Burn { get; set; }

    public string OwerWalletAddress { get; set; }
    public string UniqueCode { get; set; }
}
