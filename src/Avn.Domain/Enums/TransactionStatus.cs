namespace Avn.Domain.Enums;

public enum TransactionStatus : byte
{
    Pending = 1,
    TimedOut = 2,
    RejectByUser = 3,
    RejectByNetwork = 4,
    ConfirmedByNetwork = 5
}