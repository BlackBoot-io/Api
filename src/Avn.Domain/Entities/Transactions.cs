﻿using System.Text.Json.Serialization;

namespace Avn.Domain.Entities;

[Table(nameof(Transaction), Schema = nameof(EntitySchema.Payment))]
public class Transaction : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [JsonIgnore]
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey(nameof(PaymentMethodId))]
    public PaymentMethod PaymentMethod { get; set; }
    public int PaymentMethodId { get; set; }

    /// <summary>
    /// UsdtAmount
    /// </summary>
    public int UsdtAmount { get; set; }

    /// <summary>
    /// Crypto Amount
    /// </summary>
    [Column(TypeName = ColumnType.Decimal219)]
    public decimal CryptoAmount { get; set; }

    [Column(TypeName = ColumnType.Decimal219)]
    public decimal DiscountUsdtAmount { get; set; }

    [Column(TypeName = ColumnType.Decimal219)]
    public decimal TotalEndFee { get; set; }

    public DateTime Date { get; set; }

    public TransactionStatus Status { get; set; }

    public DateTime? ConfirmDate { get; set; }

    [MaxLength(256)]
    public string WalletAddress { get; set; }

    [MaxLength(256)]
    public string TxId { get; set; }
}