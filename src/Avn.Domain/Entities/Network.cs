namespace Avn.Domain.Entities;

[Table(nameof(Network), Schema = nameof(EntitySchema.Base))]
public class Network : IEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    public NetworkType NetworkType { get; set; }
    public decimal GasFee { get; set; }

    public bool IsActive { get; set; }


    public ICollection<Event> Events { get; set; }
}
