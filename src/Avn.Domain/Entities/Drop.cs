﻿namespace Avn.Domain.Entities;

[Table(nameof(Drop), Schema = nameof(EntitySchema.User))]
public class Drop : IEntity
{
    public Drop() => Code = Guid.NewGuid();

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey(nameof(ProjectId))]
    public Project Project { get; set; }
    public Guid? ProjectId { get; set; }

    [ForeignKey(nameof(NetworkId))]
    public Network Network { get; set; }
    public int NetworkId { get; set; }

    [ForeignKey(nameof(AttachmentId))]
    public Attachment Attachment { get; set; }
    public int AttachmentId { get; set; }

    [MaxLength(50)]
    [Required]
    public string Name { get; set; }

    public int Count { get; set; }

    public DeliveryType DeliveryType { get; set; }

    public DropCategoryType CategoryType { get; set; }

    public DropStatus DropStatus { get; set; } = DropStatus.Pending;

    [Required]
    [MaxLength(100)]
    public string DropUri { get; set; }

    [Required]
    [MaxLength(100)]
    public string ContentId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Location { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime ExpireDate { get; set; }

    public bool IsVirtual { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsActive { get; set; }

    [Required]
    public Guid Code { get; set; }

    [Required]
    [Column(TypeName = ColumnType.Decimal219)]
    public decimal Wages { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [MaxLength(500)]
    public string ReviewMessage { get; set; }
    
    public DateTime InsertDate { get; set; }
    public ICollection<Token> Tokens { get; set; }

}
